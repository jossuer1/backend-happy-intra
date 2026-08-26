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


    // Actualización completa de un usuario. Exclusivo RRHH (la restricción de rol
    // se aplica en el controller, pero el service asume que quien llama ya está autorizado).
    // Permite editar los datos generales y, además, dar de alta, editar o dar de baja
    // sus familiares, contactos de emergencia, títulos y datos bancarios en la misma llamada.
    public async Task<ServiceResult<bool>> ActualizarUsuarioAsync(long id, ActualizarUsuarioDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Familiares)
            .Include(u => u.ContactosEmergencia)
            .Include(u => u.Titulos)
            .Include(u => u.DatosBancarios)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.IdUsuario == id);

        if (usuario == null)
            return ServiceResult<bool>.Fallo("Usuario no encontrado.");

        // --- Unicidad si cambian cédula o correos ---
        if (dto.Cedula != null && dto.Cedula != usuario.Cedula &&
            await _context.Usuarios.AnyAsync(u => u.Cedula == dto.Cedula && u.IdUsuario != id))
            return ServiceResult<bool>.Fallo("La cédula ingresada ya está registrada por otro usuario.");

        if (dto.CorreoEmpresa != null && dto.CorreoEmpresa != usuario.CorreoEmpresa &&
            await _context.Usuarios.AnyAsync(u => u.CorreoEmpresa == dto.CorreoEmpresa && u.IdUsuario != id))
            return ServiceResult<bool>.Fallo("El correo empresarial ya está registrado por otro usuario.");

        if (dto.CorreoPersonal != null && dto.CorreoPersonal != usuario.CorreoPersonal &&
            await _context.Usuarios.AnyAsync(u => u.CorreoPersonal == dto.CorreoPersonal && u.IdUsuario != id))
            return ServiceResult<bool>.Fallo("El correo personal ya está registrado por otro usuario.");

        if (dto.TieneVacaciones == true && dto.DiasVacacionesAsignados.HasValue && dto.DiasVacacionesAsignados.Value < 0)
            return ServiceResult<bool>.Fallo("Los días de vacaciones asignados no pueden ser negativos.");

        // --- Campos escalares (solo se tocan los que vienen informados) ---
        if (dto.Nombre != null) usuario.Nombre = dto.Nombre;
        if (dto.Apellido != null) usuario.Apellido = dto.Apellido;
        if (dto.Cedula != null) usuario.Cedula = dto.Cedula;
        if (dto.CorreoEmpresa != null) usuario.CorreoEmpresa = dto.CorreoEmpresa;
        if (dto.CorreoPersonal != null) usuario.CorreoPersonal = dto.CorreoPersonal;
        if (dto.FechaNacimiento.HasValue) usuario.FechaNacimiento = dto.FechaNacimiento.Value;
        if (dto.IdGenero.HasValue) usuario.IdGenero = dto.IdGenero.Value;
        if (dto.IdEstadoCivil.HasValue) usuario.IdEstadoCivil = dto.IdEstadoCivil.Value;
        if (dto.IdEtnia.HasValue) usuario.IdEtnia = dto.IdEtnia.Value;
        if (dto.IdCargo.HasValue) usuario.IdCargo = dto.IdCargo.Value;
        if (dto.IdCiudad.HasValue) usuario.IdCiudad = dto.IdCiudad.Value;
        if (dto.FechaIngreso.HasValue) usuario.FechaIngreso = dto.FechaIngreso.Value;
        if (dto.CelularEmpresa != null) usuario.CelularEmpresa = dto.CelularEmpresa;
        if (dto.CelularPersonal != null) usuario.CelularPersonal = dto.CelularPersonal;
        if (dto.Direccion != null) usuario.Direccion = dto.Direccion;

        // El beneficio de vacaciones sigue la misma regla que en creación:
        // si se desactiva, los días asignados quedan en 0 sin importar lo enviado.
        if (dto.TieneVacaciones.HasValue)
        {
            usuario.TieneVacaciones = dto.TieneVacaciones.Value;
            usuario.DiasVacacionesAsignados = dto.TieneVacaciones.Value
                ? (dto.DiasVacacionesAsignados ?? usuario.DiasVacacionesAsignados)
                : 0;
        }
        else if (dto.DiasVacacionesAsignados.HasValue && usuario.TieneVacaciones)
        {
            usuario.DiasVacacionesAsignados = dto.DiasVacacionesAsignados.Value;
        }

        // --- Familiares ---
        if (dto.FamiliaresAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.Familiares.Where(f => dto.FamiliaresAEliminar.Contains(f.IdFamiliar)).ToList();
            foreach (var f in aEliminar)
            {
                usuario.Familiares.Remove(f);
                _context.Remove(f);
            }
        }

        if (dto.Familiares is { Count: > 0 })
        {
            foreach (var f in dto.Familiares)
            {
                if (f.IdFamiliar is long idF && idF > 0)
                {
                    var existente = usuario.Familiares.FirstOrDefault(x => x.IdFamiliar == idF);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El familiar con id {idF} no pertenece a este usuario.");

                    existente.Nombre = f.Nombre;
                    existente.Apellido = f.Apellido;
                    existente.Parentesco = f.Parentesco;
                    existente.FechaNacimiento = f.FechaNacimiento;
                }
                else
                {
                    usuario.Familiares.Add(new Familiar
                    {
                        Nombre = f.Nombre,
                        Apellido = f.Apellido,
                        Parentesco = f.Parentesco,
                        FechaNacimiento = f.FechaNacimiento,
                        Estado = true
                    });
                }
            }
        }

        // --- Contactos de emergencia ---
        if (dto.ContactosEmergenciaAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.ContactosEmergencia.Where(c => dto.ContactosEmergenciaAEliminar.Contains(c.IdContacto)).ToList();
            foreach (var c in aEliminar)
            {
                usuario.ContactosEmergencia.Remove(c);
                _context.Remove(c);
            }
        }

        if (dto.ContactosEmergencia is { Count: > 0 })
        {
            foreach (var c in dto.ContactosEmergencia)
            {
                if (c.IdContacto is long idC && idC > 0)
                {
                    var existente = usuario.ContactosEmergencia.FirstOrDefault(x => x.IdContacto == idC);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El contacto de emergencia con id {idC} no pertenece a este usuario.");

                    existente.Nombre = c.Nombre;
                    existente.Apellido = c.Apellido;
                    existente.Parentesco = c.Parentesco;
                    existente.Telefono = c.Telefono;
                    existente.Direccion = c.Direccion;
                }
                else
                {
                    usuario.ContactosEmergencia.Add(new ContactoEmergencia
                    {
                        Nombre = c.Nombre,
                        Apellido = c.Apellido,
                        Parentesco = c.Parentesco,
                        Telefono = c.Telefono,
                        Direccion = c.Direccion,
                        Estado = true
                    });
                }
            }
        }

        // --- Títulos ---
        if (dto.TitulosAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.Titulos.Where(t => dto.TitulosAEliminar.Contains(t.IdTitulo)).ToList();
            foreach (var t in aEliminar)
            {
                usuario.Titulos.Remove(t);
                _context.Remove(t);
            }
        }

        if (dto.Titulos is { Count: > 0 })
        {
            foreach (var t in dto.Titulos)
            {
                if (t.IdTitulo is long idT && idT > 0)
                {
                    var existente = usuario.Titulos.FirstOrDefault(x => x.IdTitulo == idT);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El título con id {idT} no pertenece a este usuario.");

                    existente.NombreTitulo = t.NombreTitulo;
                    existente.Institucion = t.Institucion;
                    existente.FechaObtencion = t.FechaObtencion;
                }
                else
                {
                    usuario.Titulos.Add(new Titulo
                    {
                        NombreTitulo = t.NombreTitulo,
                        Institucion = t.Institucion,
                        FechaObtencion = t.FechaObtencion,
                        Estado = true
                    });
                }
            }
        }

        // --- Datos bancarios ---
        if (dto.DatosBancariosAEliminar is { Count: > 0 })
        {
            var aEliminar = usuario.DatosBancarios.Where(b => dto.DatosBancariosAEliminar.Contains(b.IdDatoBancario)).ToList();
            foreach (var b in aEliminar)
            {
                usuario.DatosBancarios.Remove(b);
                _context.Remove(b);
            }
        }

        if (dto.DatosBancarios is { Count: > 0 })
        {
            foreach (var b in dto.DatosBancarios)
            {
                if (b.IdDatoBancario is long idB && idB > 0)
                {
                    var existente = usuario.DatosBancarios.FirstOrDefault(x => x.IdDatoBancario == idB);
                    if (existente == null)
                        return ServiceResult<bool>.Fallo($"El dato bancario con id {idB} no pertenece a este usuario.");

                    existente.IdBanco = b.IdBanco;
                    existente.TipoCuenta = b.TipoCuenta;
                    existente.NumeroCuenta = b.NumeroCuenta;
                }
                else
                {
                    usuario.DatosBancarios.Add(new DatoBancario
                    {
                        IdBanco = b.IdBanco,
                        TipoCuenta = b.TipoCuenta,
                        NumeroCuenta = b.NumeroCuenta,
                        Estado = true
                    });
                }
            }
        }

        usuario.FechaActualizacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
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