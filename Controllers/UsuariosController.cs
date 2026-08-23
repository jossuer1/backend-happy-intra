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

    public UsuariosController(AppDbContext context, IUsuarioService usuarioService)
    {
        _context = context;
        _usuarioService = usuarioService;
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
                u.Estado
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

        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Cargo)
            .Include(u => u.Ciudad)
            .Include(u => u.Familiares)
            .Include(u => u.ContactosEmergencia)
            .Include(u => u.DatosBancarios)
            .Include(u => u.Titulos)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        return Ok(usuario);
    }

    // Endpoint conveniente para que el usuario logueado obtenga su propio perfil sin pasar su ID
    [HttpGet("mi-perfil")]
    public async Task<IActionResult> GetMiPerfil()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await GetUsuarioPorId(currentUserId);
    }
}