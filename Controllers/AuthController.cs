using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Intranet.Data;
using Intranet.DTOs;

namespace Intranet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Cargo)
            .FirstOrDefaultAsync(u => u.Cedula == dto.Usuario || u.CorreoEmpresa == dto.Usuario);

        if (usuario == null || !usuario.Estado)
            return Unauthorized(new { mensaje = "Credenciales inválidas o usuario inactivo." });

        if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash))
            return Unauthorized(new { mensaje = "Credenciales inválidas." });

        if (usuario.DebeCambiarContrasena)
        {
            return Ok(new
            {
                debeCambiarContrasena = true,
                idUsuario = usuario.IdUsuario,
                mensaje = "Debe cambiar su contraseña antes de ingresar por primera vez."
            });
        }

        string token = GenerarJwtToken(usuario);

        return Ok(new
        {
            debeCambiarContrasena = false,
            token,
            usuario = new
            {
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Apellido,
                usuario.CorreoEmpresa,
                Rol = usuario.Rol.Nombre,
                Cargo = usuario.Cargo?.Nombre
            }
        });
    }

    [HttpPost("cambiar-contrasena")]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);

        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado." });

        if (!BCrypt.Net.BCrypt.Verify(dto.ContrasenaActual, usuario.ContrasenaHash))
            return BadRequest(new { mensaje = "La contraseña actual es incorrecta." });

        usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
        usuario.DebeCambiarContrasena = false;
        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Contraseña actualizada exitosamente. Ya puede iniciar sesión." });
    }

    private string GenerarJwtToken(Models.Usuario usuario)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
            new Claim(ClaimTypes.Email, usuario.CorreoEmpresa),
            new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"]!)),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}