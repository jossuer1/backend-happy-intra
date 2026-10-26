using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Intranet.Services;
using Intranet.Helpers;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere token JWT válido para cualquier endpoint
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IUsuarioService _usuarioService;
    private readonly ISolicitudPerfilService _solicitudPerfilService;
    private readonly IImagenPerfilService _imagenPerfilService;

    private static readonly string[] TiposImagenPermitidos =
        { "image/jpeg", "image/png", "image/webp" };
    private const long TamanoMaximoImagenBytes = 5 * 1024 * 1024; // 5 MB

    public UsuariosController(
        AppDbContext context,
        IUsuarioService usuarioService,
        ISolicitudPerfilService solicitudPerfilService,
        IImagenPerfilService imagenPerfilService)
    {
        _context = context;
        _usuarioService = usuarioService;
        _solicitudPerfilService = solicitudPerfilService;
        _imagenPerfilService = imagenPerfilService;
    }

    // Lista para los selectores de reemplazo. Incluye jefes inactivos como origen;
    // el destino siempre se valida como activo en el servicio.
    [HttpGet("jefes")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerJefes()
    {
        return Ok(await _context.Usuarios.AsNoTracking()
            .Where(u => u.EsJefe)
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new
            {
                u.IdUsuario, u.Nombre, u.Apellido, u.Estado,
                Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
                Area = u.Cargo != null && u.Cargo.Area != null ? u.Cargo.Area.Nombre : null,
                CantidadEmpleados = u.Subordinados.Count(),
                CantidadSolicitudesPendientes = _context.SolicitudesVacaciones.Count(s =>
                    s.Usuario.IdJefeDirecto == u.IdUsuario &&
                    s.Estado == EstadoSolicitudVacacion.PendienteJefe)
            }).ToListAsync());
    }

    [HttpGet("jefes/{idJefe:long}/empleados")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ObtenerEmpleadosDeJefe(long idJefe)
    {
        if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == idJefe && u.EsJefe))
            return NotFound(new { mensaje = "Jefe no encontrado." });
        return Ok(await _context.Usuarios.AsNoTracking()
            .Where(u => u.IdJefeDirecto == idJefe)
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new
            {
                u.IdUsuario, u.Nombre, u.Apellido, u.Estado, u.TieneVacaciones,
                Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
                Area = u.Cargo != null && u.Cargo.Area != null ? u.Cargo.Area.Nombre : null
            }).ToListAsync());
    }

    [HttpPost("traspasar-equipo")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> TraspasarEquipo([FromBody] TraspasarEquipoDto dto)
    {
        var resultado = await _usuarioService.TraspasarEquipoAsync(dto);
        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });
        return Ok(resultado.Data);
    }

    [HttpGet("mis-empleados")]
    public async Task<IActionResult> ObtenerMisEmpleados()
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idJefe))
            return Forbid();
        if (!await _context.Usuarios.AnyAsync(u => u.IdUsuario == idJefe && u.Estado && u.EsJefe))
            return Forbid();

        // El jefe viene exclusivamente del token. No se exponen datos del perfil personal.
        return Ok(await _context.Usuarios.AsNoTracking()
            .Where(u => u.IdJefeDirecto == idJefe)
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new
            {
                u.IdUsuario, u.Nombre, u.Apellido, u.Estado, u.TieneVacaciones,
                Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
                Area = u.Cargo != null && u.Cargo.Area != null ? u.Cargo.Area.Nombre : null
            }).ToListAsync());
    }

    // Solo RRHH puede registrar nuevos colaboradores
    [HttpPost]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
    {
        var resultado = await _usuarioService.CrearUsuarioAsync(dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return CreatedAtAction(nameof(GetUsuarioPorId), new { id = resultado.Data!.IdUsuario }, resultado.Data);
    }

    // Solo RRHH puede listar a todos los empleados
    [HttpGet]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> GetTodosLosUsuarios()
    {
        var ahora = DateTime.UtcNow;
        var usuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Cargo)
            .Select(u => new
            {
                u.IdUsuario,
                u.Cedula,
                u.Nombre,
                u.Apellido,
                u.CorreoEmpresa,
                Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
                Area = u.Cargo != null && u.Cargo.Area != null ? u.Cargo.Area.Nombre : null,
                Rol = u.Rol != null ? u.Rol.Nombre : null,
                Ciudad = u.Ciudad != null ? u.Ciudad.Nombre : null,
                Provincia = u.Ciudad != null ? u.Ciudad.Provincia.Nombre : null,
                Region = u.Ciudad != null ? u.Ciudad.Provincia.Region.Nombre : null,
                u.Sectorial,
                u.Nacionalidad,
                u.Estado,
                u.TieneVacaciones,
                // La fecha límite es tentativa: no cierra la ventana (ver Usuario.PuedeActualizarHasta).
                u.PuedeActualizarPerfil,
                u.PuedeActualizarHasta,
                // Atrasado = ventana abierta y ya pasó la fecha de referencia. Se calcula, no se guarda.
                Atrasado = u.PuedeActualizarPerfil &&
                    u.PuedeActualizarHasta != null && u.PuedeActualizarHasta <= ahora,
                TieneSolicitudPendiente = u.SolicitudesCambioPerfil
                    .Any(sp => sp.Estado == EstadoSolicitudCambioPerfil.Pendiente),
                u.EsJefe
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    // Un usuario puede ver su perfil; RRHH puede consultar el perfil de cualquiera
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuarioPorId(long id)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        // Si no es RRHH y está intentando ver el ID de otro usuario, se bloquea
        if (!esRrhh && currentUserId != id)
        {
            return Forbid(); // Retorna 403 Forbidden
        }
        var perfil = await _context.Usuarios
        .Where(u => u.IdUsuario == id)
        .Select(u => new PerfilDto
        {
            IdUsuario = u.IdUsuario,
            Cedula = u.Cedula,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            CorreoEmpresa = u.CorreoEmpresa,
            CorreoPersonal = u.CorreoPersonal,
            CelularPersonal = u.CelularPersonal,
            CelularEmpresa = u.CelularEmpresa,
            Direccion = u.Direccion,
            UrlImagenPerfil = u.UrlImagenPerfil,
            FechaNacimiento = u.FechaNacimiento,
            FechaIngreso = u.FechaIngreso,
            TieneVacaciones = u.TieneVacaciones,
            DiasVacacionesAsignados = u.DiasVacacionesAsignados,
            Estado = u.Estado,
            Rol = u.Rol != null ? u.Rol.Nombre : null,
            PuedeActualizarPerfil = u.PuedeActualizarPerfil,
            PuedeActualizarHasta = u.PuedeActualizarHasta,
            TieneSolicitudPendiente = u.SolicitudesCambioPerfil
                .Any(sp => sp.Estado == EstadoSolicitudCambioPerfil.Pendiente),

            // --- Condición laboral ---
            CargoIess = u.CargoIess,
            Sectorial = u.Sectorial,
            Nacionalidad = u.Nacionalidad,
            Jornada = u.Jornada,
            TipoContrato = u.TipoContrato,
            FechaFinContrato = u.FechaFinContrato,
            RecibeComisiones = u.RecibeComisiones,
            AcumulaDecimos = u.AcumulaDecimos,
            EsJefe = u.EsJefe,

            // --- ASIGNAR LOS IDs DE FORMA DIRECTA ---
            IdCargo = u.IdCargo,
            IdCiudad = u.IdCiudad,
            IdProvincia = u.Ciudad != null ? u.Ciudad.IdProvincia : (long?)null,
            IdRegion = u.Ciudad != null ? u.Ciudad.Provincia.IdRegion : (long?)null,
            IdGenero = u.IdGenero,
            IdEstadoCivil = u.IdEstadoCivil,
            IdEtnia = u.IdEtnia,
            IdTipoSangre = u.IdTipoSangre,
            IdJefeDirecto = u.IdJefeDirecto,

            Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
            Departamento = u.Cargo != null && u.Cargo.Area != null ? u.Cargo.Area.Nombre : null,
            Ciudad = u.Ciudad != null ? u.Ciudad.Nombre : null,
            Provincia = u.Ciudad != null ? u.Ciudad.Provincia.Nombre : null,
            Region = u.Ciudad != null ? u.Ciudad.Provincia.Region.Nombre : null,
            Genero = u.Genero != null ? u.Genero.Nombre : null,
            EstadoCivil = u.EstadoCivil != null ? u.EstadoCivil.Nombre : null,
            Etnia = u.Etnia != null ? u.Etnia.Nombre : null,
            TipoSangre = u.TipoSangre != null ? u.TipoSangre.Nombre : null,
            JefeDirecto = u.JefeDirecto != null ? u.JefeDirecto.Nombre + " " + u.JefeDirecto.Apellido : null,

            Familiares = u.Familiares.Select(f => new FamiliarDto
            {
                IdFamiliar = f.IdFamiliar,
                Nombre = f.Nombre,
                Apellido = f.Apellido,
                Parentesco = f.Parentesco,
                Cedula = f.Cedula,
                FechaNacimiento = f.FechaNacimiento,
                FechaUnion = f.FechaUnion
            }).ToList(),

            ContactosEmergencia = u.ContactosEmergencia.Select(c => new ContactoEmergenciaDto
            {
                IdContacto = c.IdContacto,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Parentesco = c.Parentesco,
                Telefono = c.Telefono,
                Direccion = c.Direccion
            }).ToList(),

            DatosBancarios = u.DatosBancarios.Select(b => new DatoBancarioDto
            {
                IdDatoBancario = b.IdDatoBancario,
                IdBanco = b.IdBanco,
                Banco = b.Banco != null ? b.Banco.Nombre : "",
                TipoCuenta = b.TipoCuenta,
                NumeroCuenta = b.NumeroCuenta
            }).ToList(),

            Titulos = u.Titulos.Select(t => new TituloDto
            {
                IdTitulo = t.IdTitulo,
                NombreTitulo = t.NombreTitulo,
                Institucion = t.Institucion,
                FechaObtencion = t.FechaObtencion,
                Culminado = t.Culminado
            }).ToList(),

            PeriodosIess = u.PeriodosIess
                .OrderBy(per => per.FechaIngreso)
                .Select(per => new PeriodoIessDto
                {
                    IdPeriodoIess = per.IdPeriodoIess,
                    FechaIngreso = per.FechaIngreso,
                    FechaSalida = per.FechaSalida,
                    CargoIess = per.CargoIess
                }).ToList()
        })
        .FirstOrDefaultAsync();

        if (perfil == null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        return Ok(perfil);
    }

    // Actualización completa del perfil de un colaborador: datos generales, y alta/edición/baja
    // de familiares, contactos de emergencia, títulos y datos bancarios. Exclusivo RRHH.
    [HttpPut("{id}")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarUsuarioDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _usuarioService.ActualizarUsuarioAsync(id, dto);

        if (!resultado.Exito)
        {
            if (resultado.Mensaje == "Usuario no encontrado.")
                return NotFound(new { mensaje = resultado.Mensaje });

            return BadRequest(new { mensaje = resultado.Mensaje });
        }

        return Ok(new { mensaje = "Usuario actualizado exitosamente." });
    }

    // Endpoint conveniente para que el usuario logueado obtenga su propio perfil sin pasar su ID
    [HttpGet("mi-perfil")]
    public async Task<IActionResult> GetMiPerfil()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await GetUsuarioPorId(currentUserId);
    }

    // Solo RRHH: activar/desactivar el beneficio de vacaciones de un usuario existente
    // y, opcionalmente, ajustar los días asignados.
    [HttpPatch("{id}/vacaciones")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ActualizarVacaciones(long id, [FromBody] ActualizarVacacionesUsuarioDto dto)
    {
        var resultado = await _usuarioService.ActualizarVacacionesAsync(id, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // Solo RRHH: activa o desactiva la cuenta de un usuario. Un usuario
    // desactivado no puede iniciar sesión ni cambiar su contraseña.
    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ActualizarEstado(long id, [FromBody] ActualizarEstadoUsuarioDto dto)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idActor)) return Forbid();
        var resultado = await _usuarioService.ActualizarEstadoAsync(id, dto.Activar, idActor);

        if (!resultado.Exito)
            return resultado.Mensaje == "Usuario no encontrado."
                ? NotFound(new { mensaje = resultado.Mensaje })
                : BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new
        {
            mensaje = dto.Activar
                ? "Usuario activado correctamente."
                : "Usuario desactivado correctamente."
        });
    }

    // Solo RRHH: habilita (o revoca) la ventana en la que el propio empleado puede
    // proponer cambios a su perfil (PUT /api/Usuarios/mi-perfil). Al habilitar hay que
    // indicar la fecha límite; el empleado recibe un correo con esa fecha.
    [HttpPatch("{id}/permiso-actualizacion")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> ActualizarPermisoPerfil(long id, [FromBody] PermisoActualizarPerfilDto dto)
    {
        var resultado = await _usuarioService.ActualizarPermisoPerfilAsync(id, dto.Habilitar, dto.FechaLimite);

        if (!resultado.Exito)
        {
            if (resultado.Mensaje == "Usuario no encontrado.")
                return NotFound(new { mensaje = resultado.Mensaje });

            return BadRequest(new { mensaje = resultado.Mensaje });
        }

        return Ok(new
        {
            mensaje = dto.Habilitar
                ? "Se habilitó la edición de perfil para este usuario."
                : "Se deshabilitó la edición de perfil para este usuario."
        });
    }

    // Solo RRHH: habilita la ventana a todos los usuarios activos (TodosActivos = true)
    // o a una lista (IdsUsuarios), con una misma fecha límite. Se omiten quienes ya
    // tienen una solicitud pendiente de revisión.
    [HttpPost("habilitar-actualizacion")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> HabilitarActualizacionMasiva([FromBody] HabilitarActualizacionMasivaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _usuarioService.HabilitarActualizacionMasivaAsync(dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // Solo RRHH: recordatorio por correo a quienes tienen la ventana abierta y aún no
    // respondieron (TodosSinEnviar = true, o una lista en IdsUsuarios). Opcionalmente
    // amplía la fecha de referencia. No cambia nada más: es solo un aviso.
    [HttpPost("recordatorio-actualizacion")]
    [Authorize(Roles = "RRHH")]
    public async Task<IActionResult> EnviarRecordatorioActualizacion([FromBody] RecordatorioActualizacionDto dto)
    {
        var resultado = await _usuarioService.EnviarRecordatorioActualizacionAsync(dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(resultado.Data);
    }

    // El empleado logueado propone cambios a su información (contacto, estado civil,
    // familiares, contactos de emergencia y títulos). NO se aplican al instante: quedan
    // como solicitud pendiente hasta que RRHH las apruebe o rechace. Solo funciona
    // mientras RRHH tenga habilitada su ventana de edición (la fecha límite es solo
    // una referencia: pasada la fecha puede seguir enviando).
    [HttpPut("mi-perfil")]
    public async Task<IActionResult> ActualizarMiPerfil([FromBody] ActualizarPerfilPropioDto dto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _solicitudPerfilService.EnviarAsync(currentUserId, dto);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Accepted(new
        {
            idSolicitud = resultado.Data,
            mensaje = "Tus cambios fueron enviados a RRHH para su revisión."
        });
    }

    // El empleado revisó su información y no tiene nada que cambiar: lo confirma, se
    // cierra su ventana y queda registrado (estado SIN_CAMBIOS) para que RRHH sepa que respondió.
    [HttpPost("mi-perfil/confirmar-sin-cambios")]
    public async Task<IActionResult> ConfirmarSinCambios()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var resultado = await _solicitudPerfilService.ConfirmarSinCambiosAsync(currentUserId);

        if (!resultado.Exito)
            return BadRequest(new { mensaje = resultado.Mensaje });

        return Ok(new
        {
            idSolicitud = resultado.Data,
            mensaje = "Registramos que tu información está al día. ¡Gracias!"
        });
    }

    // Sube/reemplaza la foto de perfil en Cloudinary y guarda la URL resultante.
    // Mismo criterio de acceso que GetUsuarioPorId: el propio usuario puede
    // actualizar su foto, o RRHH puede actualizar la de cualquiera.
    [HttpPost("{id}/foto")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> ActualizarFotoPerfil(long id, [FromForm] IFormFile foto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        if (!esRrhh && currentUserId != id)
            return Forbid();

        var errorImagen = await ValidacionImagen.ValidarAsync(foto);
        if (errorImagen != null) return BadRequest(new { mensaje = errorImagen });
        if (foto == null || foto.Length == 0)
            return BadRequest(new { mensaje = "No se recibió ningún archivo." });

        if (!TiposImagenPermitidos.Contains(foto.ContentType))
            return BadRequest(new { mensaje = "Formato no permitido. Usa JPG, PNG o WEBP." });

        if (foto.Length > TamanoMaximoImagenBytes)
            return BadRequest(new { mensaje = "La imagen no puede superar los 5MB." });

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        string urlImagen;
        try
        {
            urlImagen = await _imagenPerfilService.SubirImagenAsync(foto, id);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(502, new { mensaje = ex.Message });
        }

        usuario.UrlImagenPerfil = urlImagen;
        await _context.SaveChangesAsync();

        return Ok(new { urlImagenPerfil = urlImagen });
    }
}
