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

        if (await _context.Usuarios.AnyAsync(u => u.CorreoPersonal == dto.CorreoPersonal))
            return ServiceResult<UsuarioCreadoDto>.Fallo("El correo personal ya está registrado.");

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
            CorreoPersonal = dto.CorreoPersonal,
            ContrasenaHash = contrasenaHash,
            CelularPersonal = dto.CelularPersonal,
            CelularEmpresa = dto.CelularEmpresa,
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

            // Si el usuario no tiene el beneficio de vacaciones, los días asignados quedan en 0
            // sin importar lo que se haya mandado en el DTO.
            TieneVacaciones = dto.TieneVacaciones,
            DiasVacacionesAsignados = dto.TieneVacaciones ? (dto.DiasVacacionesAsignados ?? 15) : 0,

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
            usuario.CorreoPersonal,
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
            CorreoPersonal = usuario.CorreoPersonal,
            ClaveTemporal = claveTemporal,
            DebeCambiarContrasena = usuario.DebeCambiarContrasena
        };

        return ServiceResult<UsuarioCreadoDto>.Ok(resultado);
    }

    public async Task<ServiceResult<VacacionesUsuarioActualizadoDto>> ActualizarVacacionesAsync(long idUsuario, ActualizarVacacionesUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario == null)
            return ServiceResult<VacacionesUsuarioActualizadoDto>.Fallo("Usuario no encontrado.");

        if (dto.TieneVacaciones && dto.DiasVacacionesAsignados.HasValue && dto.DiasVacacionesAsignados.Value < 0)
            return ServiceResult<VacacionesUsuarioActualizadoDto>.Fallo("Los días asignados no pueden ser negativos.");

        usuario.TieneVacaciones = dto.TieneVacaciones;

        usuario.DiasVacacionesAsignados = dto.TieneVacaciones
            ? (dto.DiasVacacionesAsignados ?? (usuario.DiasVacacionesAsignados > 0 ? usuario.DiasVacacionesAsignados : 15))
            : 0;

        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<VacacionesUsuarioActualizadoDto>.Ok(new VacacionesUsuarioActualizadoDto
        {
            IdUsuario = usuario.IdUsuario,
            TieneVacaciones = usuario.TieneVacaciones,
            DiasVacacionesAsignados = usuario.DiasVacacionesAsignados
        });
    }


    public async Task ActualizarUsuarioAsync(long id, ActualizarUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            throw new KeyNotFoundException("Usuario no encontrado.");
        }
        if (dto.Nombre != null) usuario.Nombre = dto.Nombre;
        if (dto.Apellido != null) usuario.Apellido = dto.Apellido;
        if (dto.Cedula != null) usuario.Cedula = dto.Cedula;
        if (dto.CorreoEmpresa != null) usuario.CorreoEmpresa = dto.CorreoEmpresa;
        if (dto.CorreoPersonal != null) usuario.CorreoPersonal = dto.CorreoPersonal;
        if (dto.FechaNacimiento.HasValue) usuario.FechaNacimiento = dto.FechaNacimiento.Value;
        if (dto.IdGenero.HasValue) usuario.IdGenero = dto.IdGenero.Value;
        if (dto.IdEstadoCivil.HasValue) usuario.IdEstadoCivil = dto.IdEstadoCivil.Value;
        if (dto.IdCargo.HasValue) usuario.IdCargo = dto.IdCargo.Value;
        if (dto.IdCiudad.HasValue) usuario.IdCiudad = dto.IdCiudad.Value;
        if (dto.FechaIngreso.HasValue) usuario.FechaIngreso = dto.FechaIngreso.Value;
        if (dto.CelularEmpresa != null) usuario.CelularEmpresa = dto.CelularEmpresa;
        if (dto.CelularPersonal != null) usuario.CelularPersonal = dto.CelularPersonal;
        if (dto.Direccion != null) usuario.Direccion = dto.Direccion;
        if (dto.TieneVacaciones.HasValue) usuario.TieneVacaciones = dto.TieneVacaciones.Value;
        if (dto.DiasVacacionesAsignados.HasValue) usuario.DiasVacacionesAsignados = dto.DiasVacacionesAsignados.Value;

        await _context.SaveChangesAsync();
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