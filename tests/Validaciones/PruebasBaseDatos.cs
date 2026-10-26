using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Intranet.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Intranet.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

internal static class PruebasBaseDatos
{
    public static async Task InspeccionarIndicesAsync()
    {
        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        await using var db = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
        await db.OpenAsync();
        await using var comando = new NpgsqlCommand("SELECT indexname, indexdef FROM pg_indexes WHERE schemaname = current_schema() AND tablename = 'Usuarios' ORDER BY indexname", db);
        await using var filas = await comando.ExecuteReaderAsync();
        while (await filas.ReadAsync()) Console.WriteLine(filas.GetString(0) + ": " + filas.GetString(1));
    }

    public static async Task<int> EjecutarAsync()
    {
        var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
        var cadena = config.GetConnectionString("DefaultConnection") ?? throw new Exception("Falta configuración de base de datos.");
        var schema = "validaciones_" + Guid.NewGuid().ToString("N");
        await using var admin = new NpgsqlConnection(new NpgsqlConnectionStringBuilder(cadena) { Pooling = false }.ConnectionString);
        await admin.OpenAsync();
        await using (var crear = new NpgsqlCommand($"CREATE SCHEMA \"{schema}\"", admin)) await crear.ExecuteNonQueryAsync();
        await admin.CloseAsync();
        var opciones = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(new NpgsqlConnectionStringBuilder(cadena) { SearchPath = schema, Pooling = false }.ConnectionString).Options;
        AppDbContext Db() => new(opciones);
        var total = 0;
        void Verificar(bool condicion, string caso) { if (!condicion) throw new Exception(caso); total++; }
        var correo = new CorreoPrueba();
        UsuarioService Usuarios(AppDbContext db) => new(db, correo, config, NullLogger<UsuarioService>.Instance);
        VacacionService Vacaciones(AppDbContext db) => new(db, correo, config, NullLogger<VacacionService>.Instance);
        try
        {
            await using (var db = Db())
            {
                await db.Database.ExecuteSqlRawAsync(db.Database.GenerateCreateScript());
                db.Areas.Add(new Area { IdArea = 90001, Nombre = "PRUEBAS", Estado = true });
                db.Cargos.Add(new Cargo { IdCargo = 90001, IdArea = 90001, Nombre = "PRUEBAS", Estado = true });
                // Roles existentes del seed, o crear si no están definidos allí.
                if (!await db.Roles.AnyAsync(r => r.Nombre == "ADMIN")) db.Roles.Add(new Rol { IdRol = 90001, Nombre = "ADMIN" });
                if (!await db.Roles.AnyAsync(r => r.Nombre == "RRHH")) db.Roles.Add(new Rol { IdRol = 90002, Nombre = "RRHH" });
                await db.SaveChangesAsync();
                var idAdmin = await db.Roles.Where(r => r.Nombre == "ADMIN").Select(r => r.IdRol).FirstAsync();
                var idRrhh = await db.Roles.Where(r => r.Nombre == "RRHH").Select(r => r.IdRol).FirstAsync();
                db.Usuarios.AddRange(Usuario(90001, idAdmin), Usuario(90002, idRrhh), Usuario(90003, null), Usuario(90004, null));
                await db.SaveChangesAsync();
            }
            await using (var db = Db())
            {
                var res = await Usuarios(db).ActualizarEstadoAsync(90002, false, 90001);
                Verificar(!res.Exito, "No desactivar último RRHH");
            }
            await using (var db = Db())
            {
                var res = await Usuarios(db).ActualizarEstadoAsync(90001, false, 90002);
                Verificar(!res.Exito, "No desactivar último ADMIN");
            }
            await using (var db = Db())
            {
                var controller = new RolesController(db) { ControllerContext = new ControllerContext {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, "90001"), new Claim(ClaimTypes.Role, "ADMIN") }, "prueba")) }
                } };
                Verificar(await controller.QuitarRolAUsuario(90002) is BadRequestObjectResult, "No retirar último rol RRHH");
            }
            await using (var db = Db())
            {
                Verificar(!(await Usuarios(db).ActualizarEstadoAsync(90003, false, 90003)).Exito, "No autodesactivar cuenta");
            }
            await using (var db = Db())
            {
                var a = await db.Usuarios.FindAsync(90003L); a!.IdJefeDirecto = 90004;
                await db.SaveChangesAsync();
                Verificar(!(await Usuarios(db).ActualizarUsuarioAsync(90004, new() { IdJefeDirecto = 90003 })).Exito, "Ciclo individual rechazado");
            }
            await using (var db = Db())
            {
                Verificar(!(await Usuarios(db).ActualizarUsuarioAsync(90003, new() { IdCargo = 999999 })).Exito, "Cargo inexistente rechazado");
            }
            await using (var db = Db())
            {
                var id = await db.Set<ContactoEmergencia>().Where(c => c.Usuario!.IdUsuario == 90003).Select(c => c.IdContacto).FirstAsync();
                Verificar(!(await Usuarios(db).ActualizarUsuarioAsync(90003, new() { ContactosEmergenciaAEliminar = new() { id } })).Exito, "No eliminar último contacto");
            }
            await using (var db = Db())
            {
                Verificar(await db.Set<ContactoEmergencia>().AnyAsync(c => c.Usuario!.IdUsuario == 90003), "Error deja datos intactos");
                var duplicado = Usuario(90005, null); duplicado.Cedula = "0000090003";
                db.Usuarios.Add(duplicado);
                var rechazado = false;
                try { await db.SaveChangesAsync(); } catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: "23505" }) { rechazado = true; }
                Verificar(rechazado, "Índice único de cédula");
            }
            // Dos descuentos compiten por un saldo de 15: solo uno de 10 debe persistir.
            async Task<bool> Descontar(int desplazamiento)
            {
                await using var db = Db();
                return (await Vacaciones(db).RegistrarDescuentoAsync(new() { IdUsuario = 90003, FechaInicio = DateTime.UtcNow.Date.AddDays(desplazamiento), FechaFin = DateTime.UtcNow.Date.AddDays(desplazamiento + 9), Motivo = "Prueba de concurrencia" }, 90002)).Exito;
            }
            var resultados = await Task.WhenAll(Descontar(10), Descontar(30));
            Verificar(resultados.Count(x => x) == 1, "Descuentos simultáneos respetan saldo");
            await using (var db = Db())
            {
                Verificar((await Vacaciones(db).ObtenerSaldoAsync(90003)).Data!.DiasDisponibles == 5, "Saldo final coherente");
                Verificar(!(await Vacaciones(db).ResponderComoJefeAsync(1, 90004, new())).Exito, "Decisión omitida no cambia solicitudes");
                Verificar(!(await Vacaciones(db).ResponderComoJefeAsync(1, 90004, new() { Aprobar = false })).Exito, "Rechazo requiere motivo");
            }
            // Desactivar un jefe sigue permitido aunque conserve empleados: decisión del producto.
            await using (var db = Db())
                Verificar((await Usuarios(db).ActualizarEstadoAsync(90004, false, 90002)).Exito, "Desactivar jefe no exige traspaso");
            Console.WriteLine($"Pruebas PostgreSQL aisladas: {total} correctas.");
            return total;
        }
        finally
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(schema, "^validaciones_[a-f0-9]{32}$")) throw new Exception("Esquema de limpieza no válido.");
            await admin.OpenAsync();
            await using var borrar = new NpgsqlCommand($"DROP SCHEMA \"{schema}\" CASCADE", admin);
            await borrar.ExecuteNonQueryAsync();
        }
    }

    private static Usuario Usuario(long id, long? rol) => new()
    {
        IdUsuario = id, Cedula = id.ToString("D10"), UsuarioNombre = id.ToString(), Nombre = "Ana", Apellido = "Pérez",
        CorreoEmpresa = $"empresa{id}@example.test", CorreoPersonal = $"personal{id}@example.test", ContrasenaHash = "prueba",
        IdRol = rol, IdCargo = 90001, EsJefe = true, FechaNacimiento = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), FechaIngreso = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        ContactosEmergencia = new List<ContactoEmergencia> { new() { Nombre = "Luis", Parentesco = "HERMANO", Telefono = "0991234567" } },
        DatosBancarios = new List<DatoBancario> { new() { IdBanco = 1, TipoCuenta = "AHORROS", NumeroCuenta = "123456" } }
    };

    private sealed class CorreoPrueba : IEmailService
    {
        public Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent) => Task.FromResult(true);
        public Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent, IEnumerable<EmailAdjunto>? adjuntos) => Task.FromResult(true);
    }
}
