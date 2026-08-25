using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Services;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere token JWT válido para cualquier endpoint
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IUsuarioService _usuarioService;
    private readonly IImagenPerfilService _imagenPerfilService;

    private static readonly string[] TiposImagenPermitidos =
        { "image/jpeg", "image/png", "image/webp" };
    private const long TamanoMaximoImagenBytes = 5 * 1024 * 1024; // 5 MB

    public UsuariosController(
        AppDbContext context,
        IUsuarioService usuarioService,
        IImagenPerfilService imagenPerfilService)
    {
        _context = context;
        _usuarioService = usuarioService;
        _imagenPerfilService = imagenPerfilService;
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
        var usuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Cargo)
            .Include(u => u.Ciudad)
            .Select(u => new
            {
                u.IdUsuario,
                u.Cedula,
                u.Nombre,
                u.Apellido,
                u.CorreoEmpresa,
                Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
                Rol = u.Rol != null ? u.Rol.Nombre : null,
                u.Estado,
                u.TieneVacaciones
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

            // --- ASIGNAR LOS IDs DE FORMA DIRECTA ---
            IdCargo = u.IdCargo,
            IdCiudad = u.IdCiudad,
            IdGenero = u.IdGenero,
            IdEstadoCivil = u.IdEstadoCivil,
            IdEtnia = u.IdEtnia,

            Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
            Departamento = u.Cargo != null && u.Cargo.Area != null ? u.Cargo.Area.Nombre : null,
            Ciudad = u.Ciudad != null ? u.Ciudad.Nombre : null,
            Genero = u.Genero != null ? u.Genero.Nombre : null,
            EstadoCivil = u.EstadoCivil != null ? u.EstadoCivil.Nombre : null,
            Etnia = u.Etnia != null ? u.Etnia.Nombre : null,

            Familiares = u.Familiares.Select(f => new FamiliarDto
            {
                IdFamiliar = f.IdFamiliar,
                Nombre = f.Nombre,
                Apellido = f.Apellido,
                Parentesco = f.Parentesco,
                FechaNacimiento = f.FechaNacimiento
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
                FechaObtencion = t.FechaObtencion
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

    // Sube/reemplaza la foto de perfil en Cloudinary y guarda la URL resultante.
    // Mismo criterio de acceso que GetUsuarioPorId: el propio usuario puede
    // actualizar su foto, o RRHH puede actualizar la de cualquiera.
    [HttpPost("{id}/foto")]
    [RequestSizeLimit(TamanoMaximoImagenBytes)]
    public async Task<IActionResult> ActualizarFotoPerfil(long id, [FromForm] IFormFile foto)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esRrhh = User.IsInRole("RRHH");

        if (!esRrhh && currentUserId != id)
            return Forbid();

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