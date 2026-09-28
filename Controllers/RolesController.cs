using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Intranet.Helpers;
using Microsoft.AspNetCore.Authorization;

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
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
            return NotFound(new { mensaje = "Rol no encontrado." });

        var nombre = TextoHelper.AMayusculasObligatorio(dto.Nombre);

        if (await _context.Roles.AnyAsync(r => r.Nombre == nombre && r.IdRol != id))
            return BadRequest(new { mensaje = "Ya existe otro rol con ese nombre." });

        rol.Nombre = nombre;
        rol.Estado = dto.Estado;

        await _context.SaveChangesAsync();
        return Ok(rol);
    }

    // DELETE: api/Roles/5 (Baja lógica)
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> EliminarRol(long id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
            return NotFound(new { mensaje = "Rol no encontrado." });

        rol.Estado = false;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Rol desactivado correctamente." });
    }

    // POST: api/Roles/asignar-usuario
    [HttpPost("asignar-usuario")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AsignarRolAUsuario([FromBody] AsignarRolUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
        if (usuario == null)
            return NotFound(new { mensaje = "El usuario indicado no existe." });

        var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.IdRol && r.Estado);
        if (!rolExiste)
            return BadRequest(new { mensaje = "El rol seleccionado no existe o está inactivo." });

        usuario.IdRol = dto.IdRol;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = $"Rol asignado correctamente al usuario {usuario.Nombre} {usuario.Apellido}." });
    }
}