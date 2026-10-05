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

    // El jefe aprueba o rechaza. Al aprobar: se descuentan los días, se generan los 3
    // documentos y se envían por correo al empleado (y se avisa a RRHH).
    Task<ServiceResult<SolicitudVacacionDto>> ResponderComoJefeAsync(long idSolicitud, long idJefe, RespuestaSolicitudVacacionDto dto);

    // RRHH: todas las solicitudes (más recientes primero) para consultar su estado y bajar los documentos.
    Task<ServiceResult<List<SolicitudVacacionDto>>> ObtenerTodasLasSolicitudesAsync();

    // Genera (sin guardar en ningún lado) los 3 PDFs de una solicitud aprobada:
    // solicitud, acta y declaración de goce y pago.
    Task<ServiceResult<List<Documentos.DocumentoPdf>>> GenerarDocumentosAsync(long idSolicitud, long idUsuarioQueConsulta, bool esRrhh);
}
