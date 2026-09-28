using Microsoft.EntityFrameworkCore;
using Intranet.Data;

namespace Intranet.Services;

// Revisa, una vez al día, si algún colaborador activo cumple años hoy y le
// manda un correo de felicitación de parte del equipo de Happy Pay.
//
// No hay tabla de "correos ya enviados": se apoya en que, mientras el
// proceso siga corriendo, solo se dispara una vez por fecha (se guarda en
// memoria la última fecha en la que ya se envió). Si la API se reinicia
// más de una vez el mismo día después de la hora de envío, podría volver a
// mandar el correo de ese día; para una intranet interna es un riesgo
// aceptable, pero si se necesita garantía total, lo ideal sería registrar
// el envío en una tabla en base de datos.
public class CumpleanosBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CumpleanosBackgroundService> _logger;

    // Hora del día (UTC) a la que se revisan los cumpleaños. Ecuador está en
    // UTC-5 todo el año (sin horario de verano), así que 13:00 UTC = 8:00 AM
    // hora de Ecuador. Ajustar aquí si la empresa opera en otra zona horaria.
    private static readonly TimeSpan HoraEnvioUtc = TimeSpan.FromHours(13);

    public CumpleanosBackgroundService(IServiceScopeFactory scopeFactory, ILogger<CumpleanosBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DateTime? ultimaFechaEnviada = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var ahora = DateTime.UtcNow;

                if (ahora.TimeOfDay >= HoraEnvioUtc && ultimaFechaEnviada != ahora.Date)
                {
                    await EnviarFelicitacionesDelDiaAsync(ahora, stoppingToken);
                    ultimaFechaEnviada = ahora.Date;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revisando/enviando correos de cumpleaños.");
            }

            // Revisa cada 30 minutos; no necesita más precisión para un correo diario.
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task EnviarFelicitacionesDelDiaAsync(DateTime hoy, CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var cumpleaneros = await context.Usuarios
            .Where(u => u.Estado
                && u.FechaNacimiento != null
                && u.FechaNacimiento.Value.Month == hoy.Month
                && u.FechaNacimiento.Value.Day == hoy.Day)
            .ToListAsync(stoppingToken);

        foreach (var usuario in cumpleaneros)
        {
            var correoDestino = !string.IsNullOrWhiteSpace(usuario.CorreoEmpresa)
                ? usuario.CorreoEmpresa
                : usuario.CorreoPersonal;

            if (string.IsNullOrWhiteSpace(correoDestino))
                continue;

            try
            {
                await emailService.SendEmailAsync(
                    correoDestino,
                    $"{usuario.Nombre} {usuario.Apellido}",
                    "¡Feliz cumpleaños de parte del equipo Happy Pay! 🎉",
                    PlantillasCorreo.Cumpleanos(usuario.Nombre)
                );

                _logger.LogInformation("Correo de cumpleaños enviado a {Correo} (usuario {IdUsuario}).", correoDestino, usuario.IdUsuario);
            }
            catch (Exception ex)
            {
                // Si falla el correo de una persona, no debe frenar el envío al resto.
                _logger.LogError(ex, "No se pudo enviar el correo de cumpleaños al usuario {IdUsuario}.", usuario.IdUsuario);
            }
        }
    }
}
