using Intranet.DTOs;
using Intranet.Models;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Services;

public partial class UsuarioService
{
    public async Task<ServiceResult<TraspasarEquipoResultadoDto>> TraspasarEquipoAsync(TraspasarEquipoDto dto)
    {
        if (dto.IdJefeAnterior <= 0 || dto.IdJefeNuevo <= 0 || dto.IdJefeAnterior == dto.IdJefeNuevo)
            return ServiceResult<TraspasarEquipoResultadoDto>.Fallo("Selecciona dos jefes distintos y válidos.");

        await using var tx = await _context.Database.BeginTransactionAsync();
        // Operación administrativa breve: evita cambios de jerarquía o aprobaciones
        // durante la lectura y el traspaso. Las consultas normales siguen disponibles.
        await _context.Database.ExecuteSqlRawAsync(
            "LOCK TABLE \"Usuarios\", \"SolicitudesVacaciones\" IN SHARE ROW EXCLUSIVE MODE");

        var usuarios = await _context.Usuarios.ToListAsync();
        var anterior = usuarios.FirstOrDefault(u => u.IdUsuario == dto.IdJefeAnterior);
        var nuevo = usuarios.FirstOrDefault(u => u.IdUsuario == dto.IdJefeNuevo);
        if (anterior == null || !anterior.EsJefe)
            return ServiceResult<TraspasarEquipoResultadoDto>.Fallo("El jefe anterior no existe o no está marcado como jefe.");
        if (nuevo == null || !nuevo.EsJefe || !nuevo.Estado)
            return ServiceResult<TraspasarEquipoResultadoDto>.Fallo("El nuevo jefe debe estar activo y marcado como jefe.");

        var equipo = usuarios.Where(u => u.IdJefeDirecto == anterior.IdUsuario).ToList();
        if (equipo.Count == 0)
            return ServiceResult<TraspasarEquipoResultadoDto>.Fallo("El jefe anterior no tiene empleados asignados.");

        // Si se promueve a un integrante del equipo, hereda el superior del jefe
        // anterior para no quedar a cargo de sí mismo ni seguir bajo el jefe saliente.
        var jerarquia = usuarios.ToDictionary(u => u.IdUsuario, u => u.IdJefeDirecto);
        foreach (var empleado in equipo)
            jerarquia[empleado.IdUsuario] = empleado.IdUsuario == nuevo.IdUsuario
                ? anterior.IdJefeDirecto : nuevo.IdUsuario;

        foreach (var empleado in equipo)
        {
            var visitados = new HashSet<long>();
            long? actual = empleado.IdUsuario;
            while (actual.HasValue && jerarquia.TryGetValue(actual.Value, out var superior))
            {
                if (!visitados.Add(actual.Value))
                    return ServiceResult<TraspasarEquipoResultadoDto>.Fallo("El traspaso produciría un ciclo entre jefes. Revisa la jerarquía del nuevo jefe.");
                actual = superior;
            }
        }

        var ids = equipo.Select(u => u.IdUsuario).ToArray();
        var pendientes = await _context.SolicitudesVacaciones
            .Where(s => ids.Contains(s.IdUsuario) && s.Estado == EstadoSolicitudVacacion.PendienteJefe)
            .ToListAsync();
        // El nuevo jefe no puede aprobar sus propias solicitudes pendientes.
        if (pendientes.Any(s => s.IdUsuario == nuevo.IdUsuario) &&
            (!anterior.IdJefeDirecto.HasValue || !usuarios.Any(u =>
                u.IdUsuario == anterior.IdJefeDirecto && u.Estado && u.EsJefe)))
            return ServiceResult<TraspasarEquipoResultadoDto>.Fallo(
                "El nuevo jefe tiene solicitudes pendientes: asigna primero un superior activo al jefe anterior para poder trasladarlas.");

        var ahora = DateTime.UtcNow;
        foreach (var empleado in equipo)
        {
            empleado.IdJefeDirecto = jerarquia[empleado.IdUsuario];
            empleado.FechaActualizacion = ahora;
        }
        foreach (var solicitud in pendientes)
            solicitud.IdJefeAprobador = solicitud.IdUsuario == nuevo.IdUsuario
                ? anterior.IdJefeDirecto!.Value : nuevo.IdUsuario;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        return ServiceResult<TraspasarEquipoResultadoDto>.Ok(new()
        {
            IdJefeAnterior = anterior.IdUsuario,
            IdJefeNuevo = nuevo.IdUsuario,
            EmpleadosTrasladados = equipo.Count,
            SolicitudesPendientesTrasladadas = pendientes.Count
        });
    }
}
