using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Intranet.Helpers;
using Intranet.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
// Lectura disponible para cualquier usuario autenticado (p.ej. combos de la app);
// alta/edición/baja de roles y asignación de roles a usuarios es exclusiva de ADMIN.
[Authorize]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Roles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolReadDto>>> GetRoles()
    {
        var roles = await _context.Roles
            .Where(r => r.Estado)
            .Select(r => new RolReadDto
            {
                IdRol = r.IdRol,
                Nombre = r.Nombre,
                Estado = r.Estado
            })
            .ToListAsync();

        return Ok(roles);
    }

    // GET: api/Roles/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRolPorId(long id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
            return NotFound(new { mensaje = "Rol no encontrado." });

        return Ok(new RolReadDto
        {
            IdRol = rol.IdRol,
            Nombre = rol.Nombre,
            Estado = rol.Estado
        });
    }

    // POST: api/Roles
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CrearRol([FromBody] RolCrearDto dto)
    {
        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        var existeRol = await _context.Roles.AnyAsync(r => r.Nombre == nombre);
        if (existeRol)
            return BadRequest(new { mensaje = "El rol ingresado ya existe." });

        var rol = new Rol
        {
            Nombre = nombre,
            Estado = true
        };

        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRolPorId), new { id = rol.IdRol }, rol);
    }

    // PUT: api/Roles/5
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ActualizarRol(long id, [FromBody] RolActualizarDto dto)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        await SeguridadCuentas.BloquearAsync(_context);
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
            return NotFound(new { mensaje = "Rol no encontrado." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (rol.Nombre is "ADMIN" or "RRHH" && (nombre != rol.Nombre || !dto.Estado))
            return BadRequest(new { mensaje = "Los roles ADMIN y RRHH son roles del sistema: no se pueden renombrar ni desactivar." });
        if (await _context.Roles.AnyAsync(r => r.Nombre == nombre && r.IdRol != id))
            return BadRequest(new { mensaje = "Ya existe otro rol con ese nombre." });

        rol.Nombre = nombre;
        rol.Estado = dto.Estado;

        await _context.SaveChangesAsync();
        await _context.Usuarios.Where(u => u.IdRol == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.VersionAcceso, u => u.VersionAcceso + 1));
        await tx.CommitAsync();
        return Ok(rol);
    }

    // DELETE: api/Roles/5 (Baja lógica)
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> EliminarRol(long id)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        await SeguridadCuentas.BloquearAsync(_context);
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
            return NotFound(new { mensaje = "Rol no encontrado." });

        if (rol.Nombre is "ADMIN" or "RRHH")
            return BadRequest(new { mensaje = "Los roles ADMIN y RRHH son roles del sistema y no se pueden desactivar." });
        rol.Estado = false;
        await _context.SaveChangesAsync();
        await _context.Usuarios.Where(u => u.IdRol == id)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.VersionAcceso, u => u.VersionAcceso + 1));
        await tx.CommitAsync();

        return Ok(new { mensaje = "Rol desactivado correctamente." });
    }

    // POST: api/Roles/asignar-usuario
    [HttpPost("asignar-usuario")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AsignarRolAUsuario([FromBody] AsignarRolUsuarioDto dto)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        await SeguridadCuentas.BloquearAsync(_context);
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return NotFound(new { mensaje = "El usuario indicado no existe." });

        var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.IdRol && r.Estado);
        if (!rolExiste)
            return BadRequest(new { mensaje = "El rol seleccionado no existe o está inactivo." });

        // Evita que un ADMIN se quite (o cambie) su propio acceso por accidente.
        if (usuario.IdUsuario == IdUsuarioActual())
            return BadRequest(new { mensaje = "No puedes cambiar tu propio rol." });

        if (usuario.Estado && usuario.IdRol != dto.IdRol)
        {
            var error = await SeguridadCuentas.ValidarRetiroAsync(_context, usuario.IdUsuario, usuario.IdRol);
            if (error != null) return BadRequest(new { mensaje = error });
        }
        usuario.IdRol = dto.IdRol;
        usuario.VersionAcceso++;
        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Ok(new { mensaje = $"Rol asignado correctamente al usuario {usuario.Nombre} {usuario.Apellido}. Sus sesiones anteriores fueron revocadas; debe iniciar sesión nuevamente." });
    }

    // POST: api/Roles/quitar-usuario/5  (deja al usuario sin rol)
    [HttpPost("quitar-usuario/{idUsuario}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> QuitarRolAUsuario(long idUsuario)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        await SeguridadCuentas.BloquearAsync(_context);
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return NotFound(new { mensaje = "El usuario indicado no existe." });

        if (usuario.IdUsuario == IdUsuarioActual())
            return BadRequest(new { mensaje = "No puedes quitarte tu propio rol." });

        if (usuario.Estado)
        {
            var error = await SeguridadCuentas.ValidarRetiroAsync(_context, usuario.IdUsuario, usuario.IdRol);
            if (error != null) return BadRequest(new { mensaje = error });
        }
        usuario.IdRol = null;
        usuario.VersionAcceso++;
        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return Ok(new { mensaje = $"Se quitó el rol al usuario {usuario.Nombre} {usuario.Apellido}." });
    }

    // GET: api/Roles/usuarios?buscar=juan
    // Lista de usuarios (con su rol actual) para que el ADMIN elija a quién asignarle un rol.
    // GET /api/Usuarios es solo de RRHH, por eso este endpoint aparte.
    [HttpGet("usuarios")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<IEnumerable<UsuarioRolDto>>> GetUsuariosConRol([FromQuery] string? buscar = null)
    {
        var query = _context.Usuarios.AsNoTracking().AsQueryable();

        var texto = buscar?.Trim().ToLower();
        if (!string.IsNullOrEmpty(texto))
        {
            query = query.Where(u =>
                u.Nombre.ToLower().Contains(texto) ||
                u.Apellido.ToLower().Contains(texto) ||
                u.Cedula.Contains(texto) ||
                u.CorreoEmpresa.ToLower().Contains(texto));
        }

        var usuarios = await query
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new UsuarioRolDto
            {
                IdUsuario = u.IdUsuario,
                Cedula = u.Cedula,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                CorreoEmpresa = u.CorreoEmpresa,
                Cargo = u.Cargo != null ? u.Cargo.Nombre : null,
                IdRol = u.IdRol,
                Rol = u.Rol != null ? u.Rol.Nombre : null,
                Estado = u.Estado
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    private long? IdUsuarioActual()
        => long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
