using Intranet.Data;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

public class CumpleanosBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<CumpleanosBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DateOnly? ultimaFechaRevisada = null;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var ahora = DateTime.UtcNow;
                var hoy = CumpleanosService.HoyEcuador(ahora);
                // 13:00 UTC corresponde a las 08:00 en Ecuador continental.
                if (ahora.Hour >= 13 && ultimaFechaRevisada != hoy)
                {
                    await EnviarFelicitacionesDelDiaAsync(hoy, stoppingToken);
                    ultimaFechaRevisada = hoy;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al revisar los cumpleaños del día.");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }

    private async Task EnviarFelicitacionesDelDiaAsync(DateOnly hoy, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var incluirBisiestos = hoy.Month == 2 && hoy.Day == 28 && !DateTime.IsLeapYear(hoy.Year);
        var pendientes = await db.Usuarios
            .Where(u => u.Estado && u.FechaNacimiento != null
                && u.FechaNacimiento.Value.Month == hoy.Month
                && (u.FechaNacimiento.Value.Day == hoy.Day || (incluirBisiestos && u.FechaNacimiento.Value.Day == 29))
                && !db.FelicitacionesCumpleanos.Any(f => f.IdUsuario == u.IdUsuario && f.Anio == hoy.Year))
            .Select(u => u.IdUsuario)
            .ToListAsync(ct);

        foreach (var id in pendientes)
        {
            try
            {
                using var scopeEnvio = scopeFactory.CreateScope();
                var servicio = scopeEnvio.ServiceProvider.GetRequiredService<CumpleanosService>();
                var resultado = await servicio.FelicitarAsync(id, null, ct);
                if (!resultado.Exito)
                    logger.LogWarning("Felicitación automática al usuario {IdUsuario}: {Mensaje}", id, resultado.Mensaje);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                logger.LogError(ex, "Error al procesar la felicitación automática del usuario {IdUsuario}.", id);
            }
        }
    }
}
