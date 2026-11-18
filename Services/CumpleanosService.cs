using System.Net;
using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Intranet.Services;

public class CumpleanosService(AppDbContext db, IEmailService email, ILogger<CumpleanosService> logger)
{
    // Ecuador continental (UTC-5).
    internal static DateOnly HoyEcuador(DateTime utc) => DateOnly.FromDateTime(utc.AddHours(-5));

    internal static DateOnly Aniversario(DateTime nacimiento, int anio)
    {
        var dia = Math.Min(nacimiento.Day, DateTime.DaysInMonth(anio, nacimiento.Month));
        return new DateOnly(anio, nacimiento.Month, dia);
    }

    public async Task<IReadOnlyList<CumpleanosProximoDto>> ObtenerProximosAsync(int dias, CancellationToken ct)
    {
        var hoy = HoyEcuador(DateTime.UtcNow);
        var limite = hoy.AddDays(dias);
        var candidatos = await db.Usuarios
            .Where(u => u.Estado && u.FechaNacimiento != null)
            .Select(u => new
            {
                u.IdUsuario, u.Nombre, u.Apellido, u.UrlImagenPerfil,
                Nacimiento = u.FechaNacimiento!.Value,
                YaFelicitado = db.FelicitacionesCumpleanos.Any(f => f.IdUsuario == u.IdUsuario && f.Anio == hoy.Year)
            })
            .ToListAsync(ct);

        return candidatos.Select(u =>
        {
            var fecha = Aniversario(u.Nacimiento, hoy.Year);
            if (fecha < hoy)
                fecha = Aniversario(u.Nacimiento, hoy.Year + 1);
            return new CumpleanosProximoDto(u.IdUsuario, $"{u.Nombre} {u.Apellido}", u.UrlImagenPerfil,
                fecha, fecha.Year - u.Nacimiento.Year, u.YaFelicitado);
        })
        .Where(u => u.FechaCumpleanos <= limite)
        .OrderBy(u => u.FechaCumpleanos)
        .ThenBy(u => u.NombreCompleto)
        .ToList();
    }

    public async Task<ServiceResult<bool>> FelicitarAsync(long idUsuario, long? idUsuarioRrhh, CancellationToken ct)
    {
        var usuario = await db.Usuarios.Where(u => u.IdUsuario == idUsuario && u.Estado)
            .Select(u => new { u.Nombre, u.Apellido, u.CorreoPersonal, u.CorreoEmpresa })
            .SingleOrDefaultAsync(ct);
        if (usuario is null)
            return ServiceResult<bool>.Fallo("El colaborador no existe o está inactivo.");

        var anio = HoyEcuador(DateTime.UtcNow).Year;
        if (await db.FelicitacionesCumpleanos.AnyAsync(f => f.IdUsuario == idUsuario && f.Anio == anio, ct))
            return ServiceResult<bool>.Fallo("El colaborador ya fue felicitado este año.");

        // Se conserva la preferencia de correo del proceso automático.
        var correo = idUsuarioRrhh.HasValue ? usuario.CorreoPersonal : usuario.CorreoEmpresa;
        if (string.IsNullOrWhiteSpace(correo))
            correo = idUsuarioRrhh.HasValue ? usuario.CorreoEmpresa : usuario.CorreoPersonal;
        if (string.IsNullOrWhiteSpace(correo))
            return ServiceResult<bool>.Fallo("El colaborador no tiene un correo registrado.");

        await using var transaccion = await db.Database.BeginTransactionAsync(ct);
        // La clave única coordina envíos simultáneos, incluso entre instancias.
        // La fila solo queda registrada cuando Brevo acepta el correo y se confirma la transacción.
        var felicitacion = new FelicitacionCumpleanos
        {
            IdUsuario = idUsuario, Anio = anio, FechaEnvio = DateTime.UtcNow, IdUsuarioRRHH = idUsuarioRrhh
        };
        db.FelicitacionesCumpleanos.Add(felicitacion);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "PK_FelicitacionCumpleanos" })
        {
            db.Entry(felicitacion).State = EntityState.Detached;
            return ServiceResult<bool>.Fallo("El colaborador ya fue felicitado este año.");
        }

        try
        {
            var enviado = await email.SendEmailAsync(correo, $"{usuario.Nombre} {usuario.Apellido}",
                "¡Feliz cumpleaños de parte del equipo Happy Pay! 🎉",
                PlantillasCorreo.Cumpleanos(WebUtility.HtmlEncode(usuario.Nombre)));
            if (!enviado)
                return ServiceResult<bool>.Fallo("El servicio de correo no pudo enviar la felicitación.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al enviar la felicitación al usuario {IdUsuario}.", idUsuario);
            return ServiceResult<bool>.Fallo("El servicio de correo no pudo enviar la felicitación.");
        }
        finally
        {
            db.Entry(felicitacion).State = EntityState.Detached;
        }

        // Una desconexión del navegador no debe cancelar el registro de un correo ya enviado.
        await transaccion.CommitAsync(CancellationToken.None);
        return ServiceResult<bool>.Ok(true);
    }
}
