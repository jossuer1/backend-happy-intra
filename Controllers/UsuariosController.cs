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
                DiasVacacionesAsignados = u.DiasVacacionesAsignados,
                Estado = u.Estado,
                Rol = u.Rol != null ? u.Rol.Nombre : null,
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

    // Endpoint conveniente para que el usuario logueado obtenga su propio perfil sin pasar su ID
    [HttpGet("mi-perfil")]
    public async Task<IActionResult> GetMiPerfil()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await GetUsuarioPorId(currentUserId);
    }
}