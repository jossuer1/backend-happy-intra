using Intranet.Data;
using Intranet.DTOs;
using Intranet.Models;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

public class DashboardRrhhService(AppDbContext db)
{
    public async Task<ResumenRrhhDto> ObtenerResumenAsync(CancellationToken ct)
    {
        var empleados = await db.Usuarios.CountAsync(u => u.Estado, ct);
        var areas = await db.Areas.CountAsync(ct);
        // El flujo actual resuelve vacaciones con el jefe. Si existen solicitudes
        // en espera de RRHH, solo se cuenta ese estado explícito.
        var vacaciones = await db.SolicitudesVacaciones.CountAsync(s => s.Estado == EstadoSolicitudVacacion.PendienteRrhh, ct);
        var perfiles = await db.SolicitudesCambioPerfil.CountAsync(s => s.Estado == EstadoSolicitudCambioPerfil.Pendiente, ct);
        return new ResumenRrhhDto(empleados, areas, vacaciones + perfiles);
    }
}
