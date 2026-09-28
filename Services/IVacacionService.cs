using Intranet.DTOs;

namespace Intranet.Services;

public interface IVacacionService
{
    Task<ServiceResult<VacacionDto>> RegistrarDescuentoAsync(VacacionDescuentoCrearDto dto, long idRegistradoPor);
    Task<ServiceResult<VacacionDto>> RegistrarAjusteAsync(VacacionAjusteCrearDto dto, long idRegistradoPor);
    Task<ServiceResult<SaldoVacacionesDto>> ObtenerSaldoAsync(long idUsuario);
    Task<ServiceResult<List<VacacionDto>>> ObtenerHistorialAsync(long idUsuario);
    Task<ServiceResult<List<VacacionDto>>> ObtenerTodasLasVacacionesAsync();
    Task<ServiceResult<List<ResumenVacacionesDto>>> ObtenerResumenAsync();
    Task<ServiceResult<bool>> AnularAsync(long idVacacion, long idAnuladoPor);

    // --- Solicitudes de vacaciones: empleado -> jefe directo -> RRHH ---

    // El empleado crea la solicitud; queda pendiente de su jefe directo.
    Task<ServiceResult<SolicitudVacacionDto>> CrearSolicitudAsync(long idUsuario, SolicitudVacacionCrearDto dto);

    // El empleado consulta el estado de sus propias solicitudes.
    Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerMisSolicitudesAsync(long idUsuario);

    // El jefe ve las solicitudes de sus subordinados que esperan su respuesta.
    Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerPendientesParaJefeAsync(long idJefe);

    // El jefe aprueba (pasa a RRHH) o rechaza (termina el flujo) la solicitud.
    Task<ServiceResult<SolicitudVacacionDto>> ResponderComoJefeAsync(long idSolicitud, long idJefe, RespuestaSolicitudVacacionDto dto);

    // RRHH ve las solicitudes que el jefe ya aprobó y esperan el visto bueno final.
    Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerPendientesParaRrhhAsync();

    // RRHH da la aprobación final (genera el descuento real de días) o rechaza.
    Task<ServiceResult<SolicitudVacacionDto>> ResponderComoRrhhAsync(long idSolicitud, long idRrhh, RespuestaSolicitudVacacionDto dto);

    // Genera (sin guardar en ningún lado) la constancia en PDF de una solicitud
    // ya aprobada. El propio empleado puede generar la suya; RRHH, la de cualquiera.
    Task<ServiceResult<byte[]>> GenerarConstanciaAsync(long idSolicitud, long idUsuarioQueConsulta, bool esRrhh);
}