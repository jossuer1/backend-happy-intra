// Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;

namespace Intranet.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<ServiceResult<AuthResultDto>> LoginAsync(LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Cargo)
            .FirstOrDefaultAsync(u => u.Cedula == dto.Usuario || u.CorreoEmpresa == dto.Usuario);

        if (usuario == null || !usuario.Estado)
            return ServiceResult<AuthResultDto>.Fallo("Credenciales inválidas o usuario inactivo.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash))
            return ServiceResult<AuthResultDto>.Fallo("Credenciales inválidas.");

        if (usuario.DebeCambiarContrasena)
        {
            return ServiceResult<AuthResultDto>.Ok(new AuthResultDto
            {
                DebeCambiarContrasena = true,
                IdUsuario = usuario.IdUsuario
            });
        }

        string token = GenerarJwtToken(usuario);

        return ServiceResult<AuthResultDto>.Ok(new AuthResultDto
        {
            DebeCambiarContrasena = false,
            IdUsuario = usuario.IdUsuario,
            Token = token,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            CorreoEmpresa = usuario.CorreoEmpresa,
            Rol = usuario.Rol?.Nombre,
            Cargo = usuario.Cargo?.Nombre
        });
    }

    public async Task<ServiceResult<string>> CambiarContrasenaAsync(CambiarContrasenaDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);

        if (usuario == null)
            return ServiceResult<string>.Fallo("Usuario no encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(dto.ContrasenaActual, usuario.ContrasenaHash))
            return ServiceResult<string>.Fallo("La contraseña actual es incorrecta.");

        usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
        usuario.DebeCambiarContrasena = false;
        usuario.FechaActualizacion = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<string>.Ok("Contraseña actualizada exitosamente. Ya puede iniciar sesión.");
    }

    private string GenerarJwtToken(Usuario usuario)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
            new(ClaimTypes.Email, usuario.CorreoEmpresa)
        };

        if (usuario.Rol != null)
            claims.Add(new Claim(ClaimTypes.Role, usuario.Rol.Nombre));

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