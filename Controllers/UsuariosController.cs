using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDto dto)
    {
        string usuarioNombre = dto.Cedula.Trim();

        if (await _context.Usuarios.AnyAsync(u => u.Cedula == dto.Cedula))
            return BadRequest(new { mensaje = "La cédula/usuario ingresado ya se encuentra registrado." });

        if (await _context.Usuarios.AnyAsync(u => u.CorreoEmpresa == dto.CorreoEmpresa))
            return BadRequest(new { mensaje = "El correo empresarial ya está registrado." });

        // 1. Obtener el ID del rol predeterminado "Usuario" o "Empleado"
        var rolBase = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "Usuario" || r.Nombre == "Empleado");

        if (rolBase == null)
            return BadRequest(new { mensaje = "No se encontró el rol base 'Usuario' en el catálogo de roles." });

        // 2. Generar clave aleatoria e hashear
        string claveTemporal = GenerarContrasenaAleatoria(10);
        string contrasenaHash = BCrypt.Net.BCrypt.HashPassword(claveTemporal);

        // 3. Instanciar entidad asignando el IdRol automático
        var usuario = new Usuario
        {
            Cedula = dto.Cedula,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            UsuarioNombre = usuarioNombre,
            CorreoEmpresa = dto.CorreoEmpresa,
            ContrasenaHash = contrasenaHash,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion,
            UrlImagenPerfil = dto.UrlImagenPerfil,
            FechaNacimiento = dto.FechaNacimiento,
            FechaIngreso = dto.FechaIngreso,

            IdRol = rolBase.IdRol, // <- Rol por defecto asignado automáticamente

            IdCargo = dto.IdCargo,
            IdCiudad = dto.IdCiudad,
            IdEstadoCivil = dto.IdEstadoCivil,
            IdEtnia = dto.IdEtnia,
            IdGenero = dto.IdGenero,
            DebeCambiarContrasena = true,
            DiasVacacionesAsignados = 15,
            Estado = true,

            Familiares = dto.Familiares?.Select(f => new Familiar
            {
                Nombre = f.Nombre,
                Apellido = f.Apellido,
                Parentesco = f.Parentesco,
                FechaNacimiento = f.FechaNacimiento,
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
                Institucion = t.Institucion
            }).ToList() ?? new List<Titulo>()
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUsuarioPorId), new { id = usuario.IdUsuario }, new
        {
            usuario.IdUsuario,
            usuario.Nombre,
            usuario.Apellido,
            UsuarioAcceso = usuario.UsuarioNombre,
            usuario.CorreoEmpresa,
            RolAsignado = rolBase.Nombre,
            ClaveTemporal = claveTemporal,
            usuario.DebeCambiarContrasena
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuarioPorId(long id)
    {
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

    // Método auxiliar para generar la clave temporal
    private static string GenerarContrasenaAleatoria(int longitud = 10)
    {
        const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%&*";
        var bytes = new byte[longitud];
        RandomNumberGenerator.Fill(bytes);

        var resultado = new StringBuilder(longitud);
        foreach (byte b in bytes)
        {
            resultado.Append(caracteres[b % caracteres.Length]);
        }
        return resultado.ToString();
    }
}