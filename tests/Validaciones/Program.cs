using System.Security.Claims;
using Intranet.Models;
using Intranet.Services;
using Intranet.Helpers;
using Intranet.DTOs;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

if (args.Contains("--indices")) { await PruebasBaseDatos.InspeccionarIndicesAsync(); return; }
var comprobaciones = 0;
void Verificar(bool condicion, string caso)
{
    if (!condicion) throw new Exception(caso);
    comprobaciones++;
}
ClaimsPrincipal Token(string? version = "0", string? rol = null)
{
    var claims = new List<Claim>();
    if (version != null) claims.Add(new("version_acceso", version));
    if (rol != null) claims.Add(new(ClaimTypes.Role, rol));
    return new(new ClaimsIdentity(claims, "test"));
}
var usuario = new Usuario { Estado = true };
Verificar(AccesoToken.EsVigente(Token(), usuario), "Cuenta activa sin rol");
Verificar(AccesoToken.EsVigente(Token(null), usuario), "Token anterior a la migración");
Verificar(!AccesoToken.EsVigente(Token("inválida"), usuario), "Versión malformada");
Verificar(!AccesoToken.EsVigente(Token(), null), "Cuenta eliminada");
usuario.Estado = false;
Verificar(!AccesoToken.EsVigente(Token(), usuario), "Cuenta desactivada");
usuario.VersionAcceso++;
usuario.Estado = true;
Verificar(!AccesoToken.EsVigente(Token(), usuario), "Reactivar no revive tokens");
Verificar(AccesoToken.EsVigente(Token("1"), usuario), "Token nuevo tras reactivar");
usuario.IdRol = 1;
usuario.Rol = new Rol { IdRol = 1, Nombre = "RRHH" };
Verificar(AccesoToken.EsVigente(Token("1", "RRHH"), usuario), "Rol vigente");
Verificar(!AccesoToken.EsVigente(Token("1", "ADMIN"), usuario), "Rol diferente");
usuario.Rol.Estado = false;
Verificar(!AccesoToken.EsVigente(Token("1", "RRHH"), usuario), "Rol desactivado");
usuario.Rol.Estado = true;
usuario.IdRol = null;
usuario.VersionAcceso++;
Verificar(!AccesoToken.EsVigente(Token("1", "RRHH"), usuario), "Rol retirado");
usuario.IdRol = 1;
Verificar(!AccesoToken.EsVigente(Token("1", "RRHH"), usuario), "Reasignar rol no revive token");
var inicio = new DateTime(2026, 11, 5, 0, 0, 0, DateTimeKind.Utc);
var nueva = PlantillasCorreo.AvisoSolicitudVacaciones("Jefe", "Ana <img>", 5,
    inicio, inicio.AddDays(2), 3, "Motivo <script>", "https://example.com", true);
Verificar(nueva.Contains("05/11/2026") && nueva.Contains("07/11/2026"), "Fechas sin desplazamiento de zona");
Verificar(nueva.Contains("Ana &lt;img&gt;") && !nueva.Contains("<script>"), "Datos escapados en correo");
var rechazo = PlantillasCorreo.AvisoSolicitudVacaciones("Ana", "Ana", 5,
    inicio, inicio, 1, null, "https://example.com", false);
Verificar(rechazo.Contains("rechazó") && rechazo.Contains("No se descontaron"), "Rechazo sin observación");
bool DtoValido(object dto) => Validator.TryValidateObject(dto, new ValidationContext(dto), new List<ValidationResult>(), true);
Verificar(!DtoValido(new RespuestaSolicitudVacacionDto()), "Omitir Aprobar debe fallar");
Verificar(DtoValido(new RespuestaSolicitudVacacionDto { Aprobar = false, Observacion = "Sin cobertura" }), "False explícito debe aceptarse");
Verificar(!DtoValido(new SolicitudVacacionCrearDto { Motivo = "Descanso" }), "Fechas omitidas deben fallar");
Verificar(!DtoValido(new VacacionDescuentoCrearDto { IdUsuario = 1, Motivo = "Descanso" }), "Descuento sin fechas debe fallar");
Verificar(!DtoValido(new VacacionAjusteCrearDto { IdUsuario = 1, Dias = -2, Motivo = "Ajuste" }), "Ajuste negativo debe fallar");
Verificar(!DtoValido(new VacacionAjusteCrearDto { IdUsuario = 1, Dias = 366, Motivo = "Ajuste" }), "Ajuste mayor a 365 debe fallar");
Verificar(!ValidacionesExpediente.CelularValido("1234567890"), "Celular sin prefijo 09");
Verificar(ValidacionesExpediente.CelularValido("0991234567"), "Celular válido");
Verificar(!ValidacionesExpediente.TelefonoValido("099-1234567"), "Teléfono no numérico");
Verificar(ValidacionesExpediente.NombreValido("María-José"), "Nombre acentuado y compuesto");
Verificar(!ValidacionesExpediente.NombreValido("Ana <img>"), "Nombre con marcado HTML");
var expediente = new Usuario {
    Nombre = "Ana", Apellido = "Pérez", CorreoPersonal = "ana@example.com", CorreoEmpresa = "ana@empresa.test",
    IdCargo = 1, FechaNacimiento = inicio.AddYears(-26), FechaIngreso = inicio.AddYears(-5),
    DatosBancarios = new List<DatoBancario> { new() { IdBanco = 1, TipoCuenta = "AHORROS", NumeroCuenta = "123456" } },
    ContactosEmergencia = new List<ContactoEmergencia> { new() { Nombre = "Luis", Parentesco = "HERMANO", Telefono = "0991234567" } }
};
Verificar(ValidacionesExpediente.Validar(expediente) == null, "Expediente válido");
expediente.FechaNacimiento = ValidacionesExpediente.Hoy.AddDays(1);
Verificar(ValidacionesExpediente.Validar(expediente) != null, "Nacimiento futuro");
expediente.FechaNacimiento = inicio.AddYears(-26);
expediente.ContactosEmergencia.Clear();
Verificar(ValidacionesExpediente.Validar(expediente) != null, "Último contacto eliminado");
expediente.ContactosEmergencia.Add(new() { Nombre = "Luis", Parentesco = "HERMANO", Telefono = "0991234567" });
expediente.DatosBancarios.Clear();
Verificar(ValidacionesExpediente.Validar(expediente) != null, "Última cuenta eliminada");
Verificar(ValidacionImagen.DetectarTipo("<html>mal</html>"u8) == null, "Imagen con contenido HTML");
var archivoFalso = new FormFile(new MemoryStream("texto falso"u8.ToArray()), 0, 11, "foto", "foto.png") { Headers = new HeaderDictionary(), ContentType = "image/png" };
Verificar(await ValidacionImagen.ValidarAsync(archivoFalso) != null, "Extensión y MIME falsificados");
var archivoGrande = new FormFile(new MemoryStream(), 0, ValidacionImagen.MaximoBytes + 1, "foto", "foto.png") { Headers = new HeaderDictionary(), ContentType = "image/png" };
Verificar(await ValidacionImagen.ValidarAsync(archivoGrande) != null, "Archivo mayor de 5 MB");
Verificar(new PasswordSeguraAttribute().IsValid("Segura123!") && !new PasswordSeguraAttribute().IsValid("Segura123 "), "Espacio no cuenta como carácter especial");
if (args.Contains("--database")) comprobaciones += await PruebasBaseDatos.EjecutarAsync();
Console.WriteLine($"{comprobaciones} comprobaciones correctas.");
