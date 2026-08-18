// Services/UsuarioService.cs
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Microsoft.Extensions.Configuration;

namespace Intranet.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public UsuarioService(AppDbContext context, IEmailService emailService, IConfiguration config)
    {
        _context = context;
        _emailService = emailService;
        _config = config;
    }

    public async Task<ServiceResult<UsuarioCreadoDto>> CrearUsuarioAsync(CrearUsuarioDto dto)
    {
        // 1. Validaciones de negocio
        if (await _context.Usuarios.AnyAsync(u => u.Cedula == dto.Cedula))
            return ServiceResult<UsuarioCreadoDto>.Fallo("La cédula/usuario ingresado ya se encuentra registrado.");

        if (await _context.Usuarios.AnyAsync(u => u.CorreoEmpresa == dto.CorreoEmpresa))
            return ServiceResult<UsuarioCreadoDto>.Fallo("El correo empresarial ya está registrado.");

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
            ContrasenaHash = contrasenaHash,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion,
            UrlImagenPerfil = dto.UrlImagenPerfil,
            FechaNacimiento = dto.FechaNacimiento,
            FechaIngreso = dto.FechaIngreso,

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

        // 4. Guardar
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var urlIntranet = _config["Intranet:UrlBase"] ?? "https://intranet.happypay.com";

        await _emailService.SendEmailAsync(
            usuario.CorreoEmpresa,
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
            ClaveTemporal = claveTemporal,
            DebeCambiarContrasena = usuario.DebeCambiarContrasena
        };

        return ServiceResult<UsuarioCreadoDto>.Ok(resultado);
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