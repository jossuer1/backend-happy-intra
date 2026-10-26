// Services/UsuarioService.cs
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Intranet.Helpers;
using Microsoft.Extensions.Configuration;

namespace Intranet.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(AppDbContext context, IEmailService emailService, IConfiguration config,
        ILogger<UsuarioService> logger)
    {
        _context = context;
        _emailService = emailService;
        _config = config;
        _logger = logger;
    }

    public async Task<ServiceResult<UsuarioCreadoDto>> CrearUsuarioAsync(CrearUsuarioDto dto)
    {
        // 0. Normalización: cédula/celulares solo se recortan; nombres y textos "de orden"
        //    se guardan en MAYÚSCULAS; los correos en minúsculas (para no duplicar cuentas
        //    por diferencias de mayúsculas/minúsculas).
        dto.Cedula = TextoHelper.SoloTrim(dto.Cedula)!;
        dto.Nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);
        dto.Apellido = TextoHelper.AMayusculasObligatorio(dto.Apellido);
        dto.CorreoEmpresa = TextoHelper.AEmailNormalizado(dto.CorreoEmpresa);
        dto.CorreoPersonal = TextoHelper.AEmailNormalizado(dto.CorreoPersonal);
        dto.CelularPersonal = TextoHelper.SoloTrim(dto.CelularPersonal);
        dto.CelularEmpresa = TextoHelper.SoloTrim(dto.CelularEmpresa);
        dto.Direccion = TextoHelper.AMayusculas(dto.Direccion);

        if (dto.Familiares != null)
            foreach (var f in dto.Familiares)
            {
                f.Nombre = TextoHelper.AMayusculasObligatorio(f.Nombre);
                f.Apellido = TextoHelper.AMayusculas(f.Apellido);
                f.Parentesco = TextoHelper.AMayusculas(f.Parentesco);
                f.Cedula = NormalizarCedulaOpcional(f.Cedula);
            }

        if (dto.ContactosEmergencia != null)
            foreach (var c in dto.ContactosEmergencia)
            {
                c.Nombre = TextoHelper.AMayusculasObligatorio(c.Nombre);
                c.Apellido = TextoHelper.AMayusculas(c.Apellido);
                c.Parentesco = TextoHelper.AMayusculas(c.Parentesco);
                c.Telefono = TextoHelper.SoloTrim(c.Telefono);
                c.Direccion = TextoHelper.AMayusculas(c.Direccion);
            }

        if (dto.DatosBancarios != null)
            foreach (var b in dto.DatosBancarios)
            {
                b.TipoCuenta = TextoHelper.AMayusculasObligatorio(b.TipoCuenta);
                b.NumeroCuenta = TextoHelper.SoloTrim(b.NumeroCuenta)!;
            }

        if (dto.Titulos != null)
            foreach (var t in dto.Titulos)
            {
                t.NombreTitulo = TextoHelper.AMayusculasObligatorio(t.NombreTitulo);
                t.Institucion = TextoHelper.AMayusculasObligatorio(t.Institucion);
            }

        dto.CargoIess = string.IsNullOrWhiteSpace(dto.CargoIess) ? null : TextoHelper.AMayusculas(dto.CargoIess);
        dto.Sectorial = string.IsNullOrWhiteSpace(dto.Sectorial) ? null : TextoHelper.AMayusculas(dto.Sectorial);

        if (dto.PeriodosIess != null)
            foreach (var per in dto.PeriodosIess)
                per.CargoIess = string.IsNullOrWhiteSpace(per.CargoIess) ? null : TextoHelper.AMayusculas(per.CargoIess);

        // 1. Validaciones de negocio
        if (await _context.Usuarios.AnyAsync(u => u.Cedula == dto.Cedula))
            return ServiceResult<UsuarioCreadoDto>.Fallo("La cédula/usuario ingresado ya se encuentra registrado.");

        if (await _context.Usuarios.AnyAsync(u => u.CorreoEmpresa == dto.CorreoEmpresa))
            return ServiceResult<UsuarioCreadoDto>.Fallo("El correo empresarial ya está registrado.");

        if (await _context.Usuarios.AnyAsync(u => u.CorreoPersonal == dto.CorreoPersonal))
            return ServiceResult<UsuarioCreadoDto>.Fallo("El correo personal ya está registrado.");

        if (dto.IdTipoSangre.HasValue &&
            !await _context.TiposSangre.AnyAsync(t => t.IdTipoSangre == dto.IdTipoSangre.Value && t.Estado))
            return ServiceResult<UsuarioCreadoDto>.Fallo("El tipo de sangre indicado no es válido.");

        if (dto.IdJefeDirecto.HasValue &&
            !await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.IdJefeDirecto.Value && u.Estado && u.EsJefe))
            return ServiceResult<UsuarioCreadoDto>.Fallo("El jefe directo indicado no existe, está inactivo o no está habilitado como jefe.");

        // Jornada, tipo de contrato y fecha de fin de contrato
        var (jornada, errorJornada) = ResolverOpcion(dto.Jornada, Jornadas.Todas, "La jornada indicada no es válida");
        if (errorJornada != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorJornada);

        var (tipoContrato, errorTipo) = ResolverOpcion(dto.TipoContrato, TiposContrato.Todos, "El tipo de contrato indicado no es válido");
        if (errorTipo != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorTipo);

        var errorContrato = ValidarFechaFinContrato(tipoContrato, dto.FechaFinContrato, dto.FechaIngreso);
        if (errorContrato != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorContrato);

        var (nacionalidad, errorNacionalidad) = ResolverOpcion(dto.Nacionalidad, Nacionalidades.Todas, "La nacionalidad indicada no es válida");
        if (errorNacionalidad != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorNacionalidad);

        var errorPeriodos = ValidarPeriodosIess(dto.PeriodosIess?.Select(per => (per.FechaIngreso!.Value, per.FechaSalida)));
        if (errorPeriodos != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorPeriodos);

        // Familiares: parentesco dentro de la lista fija y reglas del cónyuge
        var errorFamiliares = NormalizarYValidarFamiliares(dto.Familiares);
        if (errorFamiliares != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorFamiliares);

        if (dto.Familiares != null && dto.Familiares.Count(f => f.Parentesco == Parentescos.Conyuge) > 1)
            return ServiceResult<UsuarioCreadoDto>.Fallo("Un empleado solo puede tener un cónyuge registrado.");

        var errorCedulasFam = ValidarCedulasFamiliares(dto.Familiares?.Select(f => f.Cedula) ?? Enumerable.Empty<string?>(), dto.Cedula);
        if (errorCedulasFam != null)
            return ServiceResult<UsuarioCreadoDto>.Fallo(errorCedulasFam);

        // 2. Generar y hashear contraseña temporal
        string claveTemporal = GenerarContrasenaAleatoria(10);
        string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(claveTemporal);

        // 3. Mapear DTO -> entidad (sin IdRol: queda null hasta que se asigne manualmente)
        var usuario = new Usuario
        {
            Cedula = dto.Cedula,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            UsuarioNombre = dto.Cedula.Trim(),
            CorreoEmpresa = dto.CorreoEmpresa,
            CorreoPersonal = dto.CorreoPersonal,
            ContrasenaHash = contrasenaHash,
            CelularPersonal = dto.CelularPersonal,
            CelularEmpresa = dto.CelularEmpresa,
            Direccion = dto.Direccion,
            UrlImagenPerfil = dto.UrlImagenPerfil,
            FechaNacimiento = dto.FechaNacimiento,
            FechaIngreso = dto.FechaIngreso,

            IdCargo = dto.IdCargo,
            IdCiudad = dto.IdCiudad,
            IdEstadoCivil = dto.IdEstadoCivil,
            IdEtnia = dto.IdEtnia,
            IdGenero = dto.IdGenero,
            IdTipoSangre = dto.IdTipoSangre,
            IdJefeDirecto = dto.IdJefeDirecto,
            DebeCambiarContrasena = true,
            TieneVacaciones = dto.TieneVacaciones,
            DiasVacacionesAsignados = dto.TieneVacaciones ? (dto.DiasVacacionesAsignados ?? 15) : 0,

            CargoIess = dto.CargoIess,
            Sectorial = dto.Sectorial,
            Nacionalidad = nacionalidad ?? Nacionalidades.Ecuatoriana,
            Jornada = jornada,
            TipoContrato = tipoContrato,
            FechaFinContrato = ComoUtc(dto.FechaFinContrato),
            RecibeComisiones = dto.RecibeComisiones,
            AcumulaDecimos = dto.AcumulaDecimos,
            EsJefe = dto.EsJefe,

            Estado = true,

            Familiares = dto.Familiares?.Select(f => new Familiar
            {
                Nombre = f.Nombre,
                Apellido = f.Apellido,
                Parentesco = f.Parentesco,
                Cedula = f.Cedula,
                FechaNacimiento = f.FechaNacimiento,
                FechaUnion = ComoUtc(f.FechaUnion),
                Estado = true
            }).ToList() ?? new List<Familiar>(),

            ContactosEmergencia = dto.ContactosEmergencia?.Select(c => new ContactoEmergencia
            {
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Parentesco = c.Parentesco,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                Estado = true
            }).ToList() ?? new List<ContactoEmergencia>(),

            DatosBancarios = dto.DatosBancarios?.Select(b => new DatoBancario
            {
                IdBanco = b.IdBanco,
                TipoCuenta = b.TipoCuenta,
                NumeroCuenta = b.NumeroCuenta
            }).ToList() ?? new List<DatoBancario>(),

            Titulos = dto.Titulos?.Select(t => new Titulo
            {
                NombreTitulo = t.NombreTitulo,
                Institucion = t.Institucion,
                Culminado = t.Culminado,
                // Si sigue estudiando no hay fecha de obtención.
                FechaObtencion = t.Culminado ? ComoUtc(t.FechaObtencion) : null
            }).ToList() ?? new List<Titulo>(),

            PeriodosIess = dto.PeriodosIess?.Select(per => new PeriodoIess
            {
                FechaIngreso = ComoUtc(per.FechaIngreso)!.Value,
                FechaSalida = ComoUtc(per.FechaSalida),
                CargoIess = per.CargoIess
            }).ToList() ?? new List<PeriodoIess>()
        };

        // 4. Guardar
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var urlIntranet = _config["Intranet:UrlBase"] ?? "https://intranet.happypay.com";

        await _emailService.SendEmailAsync(
            usuario.CorreoPersonal,
            $"{usuario.Nombre} {usuario.Apellido}",
            "¡Bienvenido a Happy Pay! Aquí están tus credenciales",
            PlantillasCorreo.BienvenidaConCredenciales(usuario.Nombre, usuario.UsuarioNombre, claveTemporal, urlIntranet)
        );

        // 6. Devolver resultado
        var resultado = new UsuarioCreadoDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            UsuarioAcceso = usuario.UsuarioNombre,
            CorreoEmpresa = usuario.CorreoEmpresa,
            CorreoPersonal = usuario.CorreoPersonal,
            ClaveTemporal = claveTemporal,
            DebeCambiarContrasena = usuario.DebeCambiarContrasena
        };

        return ServiceResult<UsuarioCreadoDto>.Ok(resultado);
    }

    public async Task<ServiceResult<VacacionesUsuarioActualizadoDto>> ActualizarVacacionesAsync(long idUsuario, ActualizarVacacionesUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return ServiceResult<VacacionesUsuarioActualizadoDto>.Fallo("Usuario no encontrado.");

        if (dto.TieneVacaciones && dto.DiasVacacionesAsignados.HasValue && dto.DiasVacacionesAsignados.Value < 0)
            return ServiceResult<VacacionesUsuarioActualizadoDto>.Fallo("Los días asignados no pueden ser negativos.");

        usuario.TieneVacaciones = dto.TieneVacaciones;

        usuario.DiasVacacionesAsignados = dto.TieneVacaciones
            ? (dto.DiasVacacionesAsignados ?? (usuario.DiasVacacionesAsignados > 0 ? usuario.DiasVacacionesAsignados : 15))
            : 0;

        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<VacacionesUsuarioActualizadoDto>.Ok(new VacacionesUsuarioActualizadoDto
        {
            IdUsuario = usuario.IdUsuario,
            TieneVacaciones = usuario.TieneVacaciones,
            DiasVacacionesAsignados = usuario.DiasVacacionesAsignados
        });
    }


    // Actualización completa de un usuario. Exclusivo RRHH (la restricción de rol
    // se aplica en el controller, pero el service asume que quien llama ya está autorizado).
    // Permite editar los datos generales y, además, dar de alta, editar o dar de baja
    // sus familiares, contactos de emergencia, títulos y datos bancarios en la misma llamada.
    public async Task<ServiceResult<bool>> ActualizarUsuarioAsync(long id, ActualizarUsuarioDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Familiares)
            .Include(u => u.ContactosEmergencia)
            .Include(u => u.Titulos)
            .Include(u => u.DatosBancarios)
            .Include(u => u.PeriodosIess)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario == null)
            return ServiceResult<bool>.Fallo("Usuario no encontrado.");

        // --- Normalización de lo que venga informado ---
        dto.Cedula = TextoHelper.SoloTrim(dto.Cedula);
        dto.Nombre = TextoHelper.AMayusculas(dto.Nombre);
        dto.Apellido = TextoHelper.AMayusculas(dto.Apellido);
        dto.CorreoEmpresa = dto.CorreoEmpresa != null ? TextoHelper.AEmailNormalizado(dto.CorreoEmpresa) : null;
        dto.CorreoPersonal = dto.CorreoPersonal != null ? TextoHelper.AEmailNormalizado(dto.CorreoPersonal) : null;
        dto.CelularEmpresa = TextoHelper.SoloTrim(dto.CelularEmpresa);
        dto.CelularPersonal = TextoHelper.SoloTrim(dto.CelularPersonal);
        dto.Direccion = TextoHelper.AMayusculas(dto.Direccion);

        if (dto.Familiares != null)
            foreach (var f in dto.Familiares)
            {
                f.Nombre = TextoHelper.AMayusculasObligatorio(f.Nombre);
                f.Apellido = TextoHelper.AMayusculas(f.Apellido);
                f.Parentesco = TextoHelper.AMayusculas(f.Parentesco);
                f.Cedula = NormalizarCedulaOpcional(f.Cedula);
            }

        if (dto.ContactosEmergencia != null)
            foreach (var c in dto.ContactosEmergencia)
            {
                c.Nombre = TextoHelper.AMayusculasObligatorio(c.Nombre);
                c.Apellido = TextoHelper.AMayusculas(c.Apellido);
                c.Parentesco = TextoHelper.AMayusculas(c.Parentesco);
                c.Telefono = TextoHelper.SoloTrim(c.Telefono);
                c.Direccion = TextoHelper.AMayusculas(c.Direccion);
            }

        if (dto.Titulos != null)
            foreach (var t in dto.Titulos)
            {
                t.NombreTitulo = TextoHelper.AMayusculasObligatorio(t.NombreTitulo);
                t.Institucion = TextoHelper.AMayusculas(t.Institucion);
            }

        if (dto.DatosBancarios != null)
            foreach (var b in dto.DatosBancarios)
            {
                b.TipoCuenta = TextoHelper.AMayusculasObligatorio(b.TipoCuenta);
                b.NumeroCuenta = TextoHelper.SoloTrim(b.NumeroCuenta)!;
            }

        // --- Familiares: parentesco dentro de la lista fija y reglas del cónyuge ---
        var errorFamiliares = NormalizarYValidarFamiliares(dto.Familiares);
        if (errorFamiliares != null)
            return ServiceResult<bool>.Fallo(errorFamiliares);

        // --- Jornada, tipo de contrato y fecha de fin de contrato ---
        // Se valida sobre lo que quedaría guardado: lo informado, o lo que ya tenía.
        var (jornadaNueva, errorJornada) = ResolverOpcion(dto.Jornada, Jornadas.Todas, "La jornada indicada no es válida");
        if (errorJornada != null)
            return ServiceResult<bool>.Fallo(errorJornada);

        var (tipoNuevo, errorTipo) = ResolverOpcion(dto.TipoContrato, TiposContrato.Todos, "El tipo de contrato indicado no es válido");
        if (errorTipo != null)
            return ServiceResult<bool>.Fallo(errorTipo);

        var (nacionalidadNueva, errorNacionalidad) = ResolverOpcion(dto.Nacionalidad, Nacionalidades.Todas, "La nacionalidad indicada no es válida");
        if (errorNacionalidad != null)
            return ServiceResult<bool>.Fallo(errorNacionalidad);

        var tipoFinal = tipoNuevo ?? usuario.TipoContrato;

        // Si el tipo final no admite fecha de fin (p. ej. pasa de EMERGENTE a INDEFINIDO),
        // la fecha guardada se descarta; si la admite, se conserva salvo que llegue una nueva.
        DateTime? fechaFinFinal = dto.FechaFinContrato.HasValue
            ? ComoUtc(dto.FechaFinContrato)
            : (TiposContrato.RequiereFechaFin(tipoFinal) ? usuario.FechaFinContrato : null);

        var errorContrato = ValidarFechaFinContrato(tipoFinal, fechaFinFinal, dto.FechaIngreso ?? usuario.FechaIngreso);
        if (errorContrato != null)
            return ServiceResult<bool>.Fallo(errorContrato);

        // --- Unicidad si cambian cédula o correos ---
        if (dto.Cedula != null && dto.Cedula != usuario.Cedula &&
            await _context.Usuarios.AnyAsync(u => u.Cedula == dto.Cedula && u.IdUsuario != id))
            return ServiceResult<bool>.Fallo("La cédula ingresada ya está registrada por otro usuario.");

        if (dto.CorreoEmpresa != null && dto.CorreoEmpresa != usuario.CorreoEmpresa &&
            await _context.Usuarios.AnyAsync(u => u.CorreoEmpresa == dto.CorreoEmpresa && u.IdUsuario != id))
            return ServiceResult<bool>.Fallo("El correo empresarial ya está registrado por otro usuario.");

        if (dto.CorreoPersonal != null && dto.CorreoPersonal != usuario.CorreoPersonal &&
            await _context.Usuarios.AnyAsync(u => u.CorreoPersonal == dto.CorreoPersonal && u.IdUsuario != id))
            return ServiceResult<bool>.Fallo("El correo personal ya está registrado por otro usuario.");

        if (dto.TieneVacaciones == true && dto.DiasVacacionesAsignados.HasValue && dto.DiasVacacionesAsignados.Value < 0)
            return ServiceResult<bool>.Fallo("Los días de vacaciones asignados no pueden ser negativos.");

        if (dto.IdTipoSangre.HasValue &&
            !await _context.TiposSangre.AnyAsync(t => t.IdTipoSangre == dto.IdTipoSangre.Value && t.Estado))
            return ServiceResult<bool>.Fallo("El tipo de sangre indicado no es válido.");

        if (dto.IdJefeDirecto.HasValue)
        {
            if (dto.IdJefeDirecto.Value == id)
                return ServiceResult<bool>.Fallo("Un usuario no puede ser jefe directo de sí mismo.");

            if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.IdJefeDirecto.Value && u.Estado && u.EsJefe))
                return ServiceResult<bool>.Fallo("El jefe directo indicado no existe, está inactivo o no está habilitado como jefe.");
        }

        // No se puede quitar la marca de jefe a quien todavía tiene gente a su cargo:
        // esas personas se quedarían con un jefe que ya no puede aprobar sus solicitudes.
        if (dto.EsJefe == false && usuario.EsJefe &&
            await _context.Usuarios.AnyAsync(u => u.IdJefeDirecto == id && u.Estado))
            return ServiceResult<bool>.Fallo(
                "No se puede quitar la marca de jefe: este usuario tiene empleados activos a su cargo. Reasigna primero a sus subordinados.");

        // --- Campos escalares (solo se tocan los que vienen informados) ---
        if (dto.Nombre != null) usuario.Nombre = dto.Nombre;
        if (dto.Apellido != null) usuario.Apellido = dto.Apellido;
        if (dto.Cedula != null) usuario.Cedula = dto.Cedula;
        if (dto.CorreoEmpresa != null) usuario.CorreoEmpresa = dto.CorreoEmpresa;
        if (dto.CorreoPersonal != null) usuario.CorreoPersonal = dto.CorreoPersonal;
        if (dto.FechaNacimiento.HasValue) usuario.FechaNacimiento = dto.FechaNacimiento.Value;
        if (dto.IdGenero.HasValue) usuario.IdGenero = dto.IdGenero.Value;
        if (dto.IdEstadoCivil.HasValue) usuario.IdEstadoCivil = dto.IdEstadoCivil.Value;
        if (dto.IdEtnia.HasValue) usuario.IdEtnia = dto.IdEtnia.Value;
        if (dto.IdCargo.HasValue) usuario.IdCargo = dto.IdCargo.Value;
        if (dto.IdCiudad.HasValue) usuario.IdCiudad = dto.IdCiudad.Value;
        if (dto.IdTipoSangre.HasValue) usuario.IdTipoSangre = dto.IdTipoSangre.Value;
        if (dto.IdJefeDirecto.HasValue) usuario.IdJefeDirecto = dto.IdJefeDirecto.Value;
        if (dto.FechaIngreso.HasValue) usuario.FechaIngreso = dto.FechaIngreso.Value;

        // --- Condición laboral ---
        if (dto.CargoIess != null)
            usuario.CargoIess = string.IsNullOrWhiteSpace(dto.CargoIess) ? null : TextoHelper.AMayusculas(dto.CargoIess);
        if (dto.Sectorial != null)
            usuario.Sectorial = string.IsNullOrWhiteSpace(dto.Sectorial) ? null : TextoHelper.AMayusculas(dto.Sectorial);
        if (nacionalidadNueva != null) usuario.Nacionalidad = nacionalidadNueva;
        if (jornadaNueva != null) usuario.Jornada = jornadaNueva;
        usuario.TipoContrato = tipoFinal;
        usuario.FechaFinContrato = fechaFinFinal;
        if (dto.RecibeComisiones.HasValue) usuario.RecibeComisiones = dto.RecibeComisiones.Value;
        if (dto.AcumulaDecimos.HasValue) usuario.AcumulaDecimos = dto.AcumulaDecimos.Value;
        if (dto.EsJefe.HasValue) usuario.EsJefe = dto.EsJefe.Value;
        if (dto.CelularEmpresa != null) usuario.CelularEmpresa = dto.CelularEmpresa;
        if (dto.CelularPersonal != null) usuario.CelularPersonal = dto.CelularPersonal;
        if (dto.Direccion != null) usuario.Direccion = dto.Direccion;

        // El beneficio de vacaciones sigue la misma regla que en creación:
        // si se desactiva, los días asignados quedan en 0 sin importar lo enviado.
        if (dto.TieneVacaciones.HasValue)
        {
            usuario.TieneVacaciones = dto.TieneVacaciones.Value;
            usuario.DiasVacacionesAsignados = dto.TieneVacaciones.Value
                ? (dto.DiasVacacionesAsignados ?? usuario.DiasVacacionesAsignados)
                : 0;
        }
        else if (dto.DiasVacacionesAsignados.HasValue && usuario.TieneVacaciones)
        {
            usuario.DiasVacacionesAsignados = dto.DiasVacacionesAsignados.Value;
        }

        // --- Familiares ---
        if (dto.FamiliaresAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.Familiares.Where(f => dto.FamiliaresAEliminar.Contains(f.IdFamiliar)).ToList();
            foreach (var f in aEliminar)
            {
                usuario.Familiares.Remove(f);
                _context.Remove(f);
            }
        }

        if (dto.Familiares is { Count: > 0 })
        {
            foreach (var f in dto.Familiares)
            {
                if (f.IdFamiliar is long idF && idF > 0)
                {
                    var existente = usuario.Familiares.FirstOrDefault(x => x.IdFamiliar == idF);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El familiar con id {idF} no pertenece a este usuario.");

                    existente.Nombre = f.Nombre;
                    existente.Apellido = f.Apellido;
                    existente.Parentesco = f.Parentesco;
                    existente.Cedula = f.Cedula;
                    existente.FechaNacimiento = f.FechaNacimiento;
                    existente.FechaUnion = ComoUtc(f.FechaUnion);
                }
                else
                {
                    usuario.Familiares.Add(new Familiar
                    {
                        Nombre = f.Nombre,
                        Apellido = f.Apellido,
                        Parentesco = f.Parentesco,
                        Cedula = f.Cedula,
                        FechaNacimiento = f.FechaNacimiento,
                        FechaUnion = ComoUtc(f.FechaUnion),
                        Estado = true
                    });
                }
            }
        }

        // Un empleado solo puede tener un cónyuge (se revisa sobre el resultado final).
        if (usuario.Familiares.Count(f => f.Estado && f.Parentesco == Parentescos.Conyuge) > 1)
            return ServiceResult<bool>.Fallo("Un empleado solo puede tener un cónyuge registrado.");

        var errorCedulasFamiliares = ValidarCedulasFamiliares(
            usuario.Familiares.Where(f => f.Estado).Select(f => f.Cedula), usuario.Cedula);
        if (errorCedulasFamiliares != null)
            return ServiceResult<bool>.Fallo(errorCedulasFamiliares);

        // --- Contactos de emergencia ---
        if (dto.ContactosEmergenciaAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.ContactosEmergencia.Where(c => dto.ContactosEmergenciaAEliminar.Contains(c.IdContacto)).ToList();
            foreach (var c in aEliminar)
            {
                usuario.ContactosEmergencia.Remove(c);
                _context.Remove(c);
            }
        }

        if (dto.ContactosEmergencia is { Count: > 0 })
        {
            foreach (var c in dto.ContactosEmergencia)
            {
                if (c.IdContacto is long idC && idC > 0)
                {
                    var existente = usuario.ContactosEmergencia.FirstOrDefault(x => x.IdContacto == idC);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El contacto de emergencia con id {idC} no pertenece a este usuario.");

                    existente.Nombre = c.Nombre;
                    existente.Apellido = c.Apellido;
                    existente.Parentesco = c.Parentesco;
                    existente.Telefono = c.Telefono;
                    existente.Direccion = c.Direccion;
                }
                else
                {
                    usuario.ContactosEmergencia.Add(new ContactoEmergencia
                    {
                        Nombre = c.Nombre,
                        Apellido = c.Apellido,
                        Parentesco = c.Parentesco,
                        Telefono = c.Telefono,
                        Direccion = c.Direccion,
                        Estado = true
                    });
                }
            }
        }

        // --- Títulos ---
        if (dto.TitulosAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.Titulos.Where(t => dto.TitulosAEliminar.Contains(t.IdTitulo)).ToList();
            foreach (var t in aEliminar)
            {
                usuario.Titulos.Remove(t);
                _context.Remove(t);
            }
        }

        if (dto.Titulos is { Count: > 0 })
        {
            foreach (var t in dto.Titulos)
            {
                if (t.IdTitulo is long idT && idT > 0)
                {
                    var existente = usuario.Titulos.FirstOrDefault(x => x.IdTitulo == idT);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El título con id {idT} no pertenece a este usuario.");

                    existente.NombreTitulo = t.NombreTitulo;
                    existente.Institucion = t.Institucion;
                    existente.Culminado = t.Culminado;
                    existente.FechaObtencion = t.Culminado ? ComoUtc(t.FechaObtencion) : null;
                }
                else
                {
                    usuario.Titulos.Add(new Titulo
                    {
                        NombreTitulo = t.NombreTitulo,
                        Institucion = t.Institucion,
                        Culminado = t.Culminado,
                        FechaObtencion = t.Culminado ? ComoUtc(t.FechaObtencion) : null,
                        Estado = true
                    });
                }
            }
        }

        // --- Datos bancarios ---
        if (dto.DatosBancariosAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.DatosBancarios.Where(b => dto.DatosBancariosAEliminar.Contains(b.IdDatoBancario)).ToList();
            foreach (var b in aEliminar)
            {
                usuario.DatosBancarios.Remove(b);
                _context.Remove(b);
            }
        }

        if (dto.DatosBancarios is { Count: > 0 })
        {
            foreach (var b in dto.DatosBancarios)
            {
                if (b.IdDatoBancario is long idB && idB > 0)
                {
                    var existente = usuario.DatosBancarios.FirstOrDefault(x => x.IdDatoBancario == idB);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El dato bancario con id {idB} no pertenece a este usuario.");

                    existente.IdBanco = b.IdBanco;
                    existente.TipoCuenta = b.TipoCuenta;
                    existente.NumeroCuenta = b.NumeroCuenta;
                }
                else
                {
                    usuario.DatosBancarios.Add(new DatoBancario
                    {
                        IdBanco = b.IdBanco,
                        TipoCuenta = b.TipoCuenta,
                        NumeroCuenta = b.NumeroCuenta,
                        Estado = true
                    });
                }
            }
        }

        // --- Períodos IESS (entradas y salidas) ---
        if (dto.PeriodosIessAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.PeriodosIess.Where(x => dto.PeriodosIessAEliminar.Contains(x.IdPeriodoIess)).ToList();
            foreach (var per in aEliminar)
            {
                usuario.PeriodosIess.Remove(per);
                _context.Remove(per);
            }
        }

        if (dto.PeriodosIess is { Count: > 0 })
        {
            foreach (var per in dto.PeriodosIess)
            {
                var cargoPeriodo = string.IsNullOrWhiteSpace(per.CargoIess) ? null : TextoHelper.AMayusculas(per.CargoIess);

                if (per.IdPeriodoIess is long idP && idP > 0)
                {
                    var existente = usuario.PeriodosIess.FirstOrDefault(x => x.IdPeriodoIess == idP);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El período IESS con id {idP} no pertenece a este usuario.");

                    existente.FechaIngreso = ComoUtc(per.FechaIngreso)!.Value;
                    existente.FechaSalida = ComoUtc(per.FechaSalida);
                    existente.CargoIess = cargoPeriodo;
                }
                else
                {
                    usuario.PeriodosIess.Add(new PeriodoIess
                    {
                        FechaIngreso = ComoUtc(per.FechaIngreso)!.Value,
                        FechaSalida = ComoUtc(per.FechaSalida),
                        CargoIess = cargoPeriodo
                    });
                }
            }
        }

        // Se valida sobre el resultado final: sin traslapes y con un solo período abierto (el último).
        var errorPeriodos = ValidarPeriodosIess(usuario.PeriodosIess.Select(x => (x.FechaIngreso, x.FechaSalida)));
        if (errorPeriodos != null)
            return ServiceResult<bool>.Fallo(errorPeriodos);

        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    // RRHH habilita (o revoca) la ventana de autoedición de perfil de un empleado.
    // Al habilitar, la fecha límite es obligatoria y el empleado recibe un correo
    // avisándole hasta cuándo puede actualizar su información.
    public async Task<ServiceResult<bool>> ActualizarPermisoPerfilAsync(long id, bool habilitar, DateTime? fechaLimite)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return ServiceResult<bool>.Fallo("Usuario no encontrado.");

        if (!habilitar)
        {
            usuario.PuedeActualizarPerfil = false;
            usuario.PuedeActualizarHasta = null;
            usuario.FechaActualizacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }

        var errorFecha = ValidarFechaLimite(fechaLimite);
        if (errorFecha != null)
            return ServiceResult<bool>.Fallo(errorFecha);

        if (!usuario.Estado)
            return ServiceResult<bool>.Fallo("No se puede habilitar la edición de un usuario inactivo.");

        if (await _context.SolicitudesCambioPerfil.AnyAsync(s =>
                s.IdUsuario == id && s.Estado == EstadoSolicitudCambioPerfil.Pendiente))
            return ServiceResult<bool>.Fallo(
                "Este usuario tiene una solicitud de cambios pendiente de revisión. Resuélvela antes de habilitar una nueva actualización.");

        var limite = FechasEcuador.FinDelDiaEnUtc(fechaLimite!.Value);
        usuario.PuedeActualizarPerfil = true;
        usuario.PuedeActualizarHasta = limite;
        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await EnviarAvisoVentanaAsync(
            new[] { (usuario.Nombre, usuario.Apellido, usuario.CorreoEmpresa, usuario.CorreoPersonal) }, limite);

        return ServiceResult<bool>.Ok(true);
    }

    // RRHH habilita la ventana a todos los activos o a una lista, con la misma fecha límite.
    // Se hace con una sola consulta (no 60 llamadas) y se omiten quienes ya tienen una
    // solicitud pendiente: su ventana se reabre sola cuando RRHH la resuelva.
    public async Task<ServiceResult<HabilitarActualizacionMasivaResultadoDto>> HabilitarActualizacionMasivaAsync(
        HabilitarActualizacionMasivaDto dto)
    {
        var errorFecha = ValidarFechaLimite(dto.FechaLimite);
        if (errorFecha != null)
            return ServiceResult<HabilitarActualizacionMasivaResultadoDto>.Fallo(errorFecha);

        var ids = dto.IdsUsuarios?.Distinct().ToList() ?? new List<long>();
        if (!dto.TodosActivos && ids.Count == 0)
            return ServiceResult<HabilitarActualizacionMasivaResultadoDto>.Fallo(
                "Indica TodosActivos = true o al menos un usuario en IdsUsuarios.");

        var limite = FechasEcuador.FinDelDiaEnUtc(dto.FechaLimite!.Value);

        var objetivo = _context.Usuarios.Where(u => u.Estado);
        if (!dto.TodosActivos)
            objetivo = objetivo.Where(u => ids.Contains(u.IdUsuario));

        var conPendiente = objetivo.Where(u => u.SolicitudesCambioPerfil
            .Any(s => s.Estado == EstadoSolicitudCambioPerfil.Pendiente));
        var omitidos = await conPendiente.CountAsync();

        var elegibles = objetivo.Where(u => !u.SolicitudesCambioPerfil
            .Any(s => s.Estado == EstadoSolicitudCambioPerfil.Pendiente));

        var destinatarios = await elegibles
            .Select(u => new { u.Nombre, u.Apellido, u.CorreoEmpresa, u.CorreoPersonal })
            .ToListAsync();

        var ahora = DateTime.UtcNow;
        var habilitados = await elegibles.ExecuteUpdateAsync(set => set
            .SetProperty(u => u.PuedeActualizarPerfil, true)
            .SetProperty(u => u.PuedeActualizarHasta, (DateTime?)limite)
            .SetProperty(u => u.FechaActualizacion, ahora));

        await EnviarAvisoVentanaAsync(
            destinatarios.Select(d => (d.Nombre, d.Apellido, d.CorreoEmpresa, d.CorreoPersonal)), limite);

        return ServiceResult<HabilitarActualizacionMasivaResultadoDto>.Ok(new HabilitarActualizacionMasivaResultadoDto
        {
            Habilitados = habilitados,
            OmitidosPorSolicitudPendiente = omitidos,
            FechaLimite = limite
        });
    }

    // Recordatorio por correo a quienes tienen la ventana abierta (aún no enviaron cambios ni
    // confirmaron "sin cambios"). Quien ya respondió no recibe nada. Si viene NuevaFechaLimite,
    // se actualiza la fecha de referencia de los destinatarios. No toca nada más.
    public async Task<ServiceResult<RecordatorioActualizacionResultadoDto>> EnviarRecordatorioActualizacionAsync(
        RecordatorioActualizacionDto dto)
    {
        var ids = dto.IdsUsuarios?.Distinct().ToList() ?? new List<long>();
        if (!dto.TodosSinEnviar && ids.Count == 0)
            return ServiceResult<RecordatorioActualizacionResultadoDto>.Fallo(
                "Indica TodosSinEnviar = true o al menos un usuario en IdsUsuarios.");

        DateTime? nuevoLimite = null;
        if (dto.NuevaFechaLimite.HasValue)
        {
            var errorFecha = ValidarFechaLimite(dto.NuevaFechaLimite);
            if (errorFecha != null)
                return ServiceResult<RecordatorioActualizacionResultadoDto>.Fallo(errorFecha);

            nuevoLimite = FechasEcuador.FinDelDiaEnUtc(dto.NuevaFechaLimite.Value);
        }

        var objetivo = _context.Usuarios.Where(u => u.Estado);
        if (!dto.TodosSinEnviar)
            objetivo = objetivo.Where(u => ids.Contains(u.IdUsuario));

        // Ventana abierta = todavía no respondió.
        var elegibles = objetivo.Where(u => u.PuedeActualizarPerfil);

        var destinatarios = await elegibles
            .Select(u => new { u.Nombre, u.Apellido, u.CorreoEmpresa, u.CorreoPersonal, u.PuedeActualizarHasta })
            .ToListAsync();

        if (nuevoLimite.HasValue && destinatarios.Count > 0)
        {
            var ahora = DateTime.UtcNow;
            await elegibles.ExecuteUpdateAsync(set => set
                .SetProperty(u => u.PuedeActualizarHasta, (DateTime?)nuevoLimite)
                .SetProperty(u => u.FechaActualizacion, ahora));
        }

        var urlIntranet = _config["Intranet:UrlBase"] ?? "https://intranet.happypay.com";

        await Parallel.ForEachAsync(
            destinatarios,
            new ParallelOptions { MaxDegreeOfParallelism = 5 },
            async (d, _) =>
            {
                var correo = string.IsNullOrWhiteSpace(d.CorreoEmpresa) ? d.CorreoPersonal : d.CorreoEmpresa;
                if (string.IsNullOrWhiteSpace(correo))
                    return;

                var referencia = nuevoLimite ?? d.PuedeActualizarHasta;
                var fecha = referencia.HasValue ? FechasEcuador.FechaLarga(referencia.Value) : null;

                try
                {
                    var ok = await _emailService.SendEmailAsync(
                        correo,
                        $"{d.Nombre} {d.Apellido}",
                        "Recordatorio: actualiza tu información en la intranet",
                        PlantillasCorreo.RecordatorioActualizacionPerfil(d.Nombre, fecha, urlIntranet));

                    if (!ok)
                        _logger.LogWarning("No se pudo enviar el recordatorio de actualización de perfil a {Correo}.", correo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enviando el recordatorio de actualización de perfil a {Correo}.", correo);
                }
            });

        return ServiceResult<RecordatorioActualizacionResultadoDto>.Ok(new RecordatorioActualizacionResultadoDto
        {
            Enviados = destinatarios.Count,
            Omitidos = dto.TodosSinEnviar ? 0 : Math.Max(0, ids.Count - destinatarios.Count),
            FechaLimite = nuevoLimite
        });
    }

    // La fecha límite debe existir y no ser anterior a hoy (hora de Ecuador).
    private static string? ValidarFechaLimite(DateTime? fechaLimite)
    {
        if (!fechaLimite.HasValue)
            return "La fecha límite es obligatoria al habilitar la actualización.";

        if (fechaLimite.Value.Date < FechasEcuador.Hoy())
            return "La fecha límite no puede ser anterior a hoy.";

        return null;
    }

    // Avisa por correo (hasta 5 envíos en paralelo) que ya pueden actualizar su información.
    // Un fallo de correo no deshace la habilitación: solo se registra en el log.
    private async Task EnviarAvisoVentanaAsync(
        IEnumerable<(string Nombre, string Apellido, string? CorreoEmpresa, string? CorreoPersonal)> destinatarios,
        DateTime limiteUtc)
    {
        var urlIntranet = _config["Intranet:UrlBase"] ?? "https://intranet.happypay.com";
        var fecha = FechasEcuador.FechaLarga(limiteUtc);

        await Parallel.ForEachAsync(
            destinatarios,
            new ParallelOptions { MaxDegreeOfParallelism = 5 },
            async (d, _) =>
            {
                var correo = string.IsNullOrWhiteSpace(d.CorreoEmpresa) ? d.CorreoPersonal : d.CorreoEmpresa;
                if (string.IsNullOrWhiteSpace(correo))
                    return;

                try
                {
                    var ok = await _emailService.SendEmailAsync(
                        correo,
                        $"{d.Nombre} {d.Apellido}",
                        "Actualiza tu información en la intranet",
                        PlantillasCorreo.ActualizacionPerfilHabilitada(d.Nombre, fecha, urlIntranet));

                    if (!ok)
                        _logger.LogWarning("No se pudo enviar el aviso de actualización de perfil a {Correo}.", correo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enviando el aviso de actualización de perfil a {Correo}.", correo);
                }
            });
    }

    // RRHH activa o desactiva la cuenta de un usuario. Un usuario inactivo no
    // puede iniciar sesión ni cambiar su contraseña (ver AuthService.LoginAsync
    // y CambiarContrasenaAsync). Al desactivar, además se le revoca cualquier
    // ventana de autoedición de perfil que tuviera pendiente.
    public async Task<ServiceResult<bool>> ActualizarEstadoAsync(long id, bool activar)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return ServiceResult<bool>.Fallo("Usuario no encontrado.");

        usuario.Estado = activar;
        if (!activar)
        {
            usuario.PuedeActualizarPerfil = false;
            usuario.PuedeActualizarHasta = null;
        }

        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    // Las fechas de estos campos son solo "día" y la columna es timestamptz: se marcan
    // como UTC (igual que en VacacionService) para que Npgsql no rechace un Kind sin definir.
    internal static DateTime? ComoUtc(DateTime? fecha)
        => fecha.HasValue ? DateTime.SpecifyKind(fecha.Value, DateTimeKind.Utc) : null;

    // Resuelve un valor de lista fija: (null, null) si no se informó; (valor canónico, null)
    // si es válido; (null, mensaje) si viene algo que no está en la lista.
    private static (string? Valor, string? Error) ResolverOpcion(
        string? entrada, IReadOnlyCollection<string> validas, string mensajeInvalido)
    {
        if (string.IsNullOrWhiteSpace(entrada))
            return (null, null);

        var canonico = OpcionesHelper.Normalizar(entrada, validas);
        if (canonico == null)
            return (null, $"{mensajeInvalido}. Valores permitidos: {string.Join(", ", validas)}.");

        return (canonico, null);
    }

    // EMERGENTE y PRODUCTIVO exigen fecha de fin (posterior al ingreso); el resto de
    // tipos (o sin tipo) no admiten fecha de fin. Devuelve un mensaje de error o null.
    private static string? ValidarFechaFinContrato(string? tipoContrato, DateTime? fechaFin, DateTime? fechaIngreso)
    {
        if (TiposContrato.RequiereFechaFin(tipoContrato))
        {
            if (!fechaFin.HasValue)
                return $"La fecha de fin de contrato es obligatoria para contratos {tipoContrato}.";

            if (fechaIngreso.HasValue && fechaFin.Value.Date <= fechaIngreso.Value.Date)
                return "La fecha de fin de contrato debe ser posterior a la fecha de ingreso.";

            return null;
        }

        if (fechaFin.HasValue)
            return $"La fecha de fin de contrato solo aplica a contratos {string.Join(" o ", TiposContrato.ConFechaFin)}.";

        return null;
    }

    // Lleva el parentesco de cada familiar a su valor canónico (CONYUGE o HIJO) y aplica
    // la regla de fechas: el cónyuge usa FechaUnion (su FechaNacimiento se descarta) y
    // los hijos no pueden traer FechaUnion. Devuelve un mensaje de error o null.
    internal static string? NormalizarYValidarFamiliares(IEnumerable<IFamiliarEntrada>? familiares)
    {
        if (familiares == null)
            return null;

        foreach (var familiar in familiares)
        {
            var parentesco = OpcionesHelper.Normalizar(familiar.Parentesco, Parentescos.Todos);
            if (parentesco == null)
                return $"El parentesco del familiar es obligatorio y debe ser uno de: {string.Join(", ", Parentescos.Todos)}.";

            familiar.Parentesco = parentesco;

            if (parentesco == Parentescos.Conyuge)
                familiar.FechaNacimiento = null;
            else if (familiar.FechaUnion.HasValue)
                return "La fecha de unión solo aplica al cónyuge.";
        }

        return null;
    }

    // Cédula opcional de un familiar: se recorta y, si queda vacía, se guarda como null.
    internal static string? NormalizarCedulaOpcional(string? cedula)
    {
        var limpia = TextoHelper.SoloTrim(cedula);
        return string.IsNullOrEmpty(limpia) ? null : limpia;
    }

    // Las cédulas informadas de los familiares no pueden repetirse entre sí ni coincidir
    // con la del propio empleado. Devuelve un mensaje de error o null.
    internal static string? ValidarCedulasFamiliares(IEnumerable<string?> cedulas, string? cedulaEmpleado)
    {
        var informadas = cedulas
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!.Trim())
            .ToList();

        if (informadas.Count != informadas.Distinct().Count())
            return "Hay familiares registrados con la misma cédula.";

        if (!string.IsNullOrWhiteSpace(cedulaEmpleado) && informadas.Contains(cedulaEmpleado.Trim()))
            return "La cédula de un familiar no puede ser la misma que la del empleado.";

        return null;
    }

    // Reglas de los períodos IESS de un colaborador (se evalúan ordenados por ingreso):
    // la salida no puede ser anterior al ingreso, un período no puede empezar antes de que
    // termine el anterior, y solo el último puede quedar abierto (sin fecha de salida).
    private static string? ValidarPeriodosIess(IEnumerable<(DateTime Ingreso, DateTime? Salida)>? periodos)
    {
        if (periodos == null)
            return null;

        var lista = periodos.OrderBy(x => x.Ingreso).ToList();

        for (int i = 0; i < lista.Count; i++)
        {
            var (ingreso, salida) = lista[i];

            if (salida.HasValue && salida.Value.Date < ingreso.Date)
                return $"En el período IESS que inicia el {ingreso:dd/MM/yyyy}, la fecha de salida no puede ser anterior a la de ingreso.";

            if (i < lista.Count - 1)
            {
                if (!salida.HasValue)
                    return "Solo el último período IESS puede quedar sin fecha de salida.";

                if (salida.Value.Date >= lista[i + 1].Ingreso.Date)
                    return $"Los períodos IESS no pueden traslaparse: el que inicia el {ingreso:dd/MM/yyyy} debe terminar antes del siguiente ingreso.";
            }
        }

        return null;
    }

    private static string GenerarContrasenaAleatoria(int longitud = 10)
    {
        const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%&*";
        var bytes = new byte[longitud];
        RandomNumberGenerator.Fill(bytes);

        var resultado = new StringBuilder(longitud);
        foreach (byte b in bytes)
            resultado.Append(caracteres[b % caracteres.Length]);

        return resultado.ToString();
    }
}