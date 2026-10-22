using Intranet.DTOs;

namespace Intranet.Services;

public interface ISolicitudPerfilService
{
    // El empleado envía sus cambios: se validan y quedan PENDIENTE (no se aplica nada).
    // Requiere que RRHH le haya habilitado la ventana y que no haya vencido.
    Task<ServiceResult<long>> EnviarAsync(long idUsuario, ActualizarPerfilPropioDto dto);

    Task<ServiceResult<List<SolicitudCambioPerfilResumenDto>>> ObtenerMiasAsync(long idUsuario);

    // RRHH: listado de todas las solicitudes, opcionalmente filtrado por estado.
    Task<ServiceResult<List<SolicitudCambioPerfilResumenDto>>> ListarAsync(string? estado);

    // Detalle con el comparativo antes/después. Un empleado solo ve las suyas.
    Task<ServiceResult<SolicitudCambioPerfilDetalleDto>> ObtenerDetalleAsync(long idSolicitud, long idSolicitante, bool esRrhh);

    // RRHH aprueba: se aplican los cambios al usuario y se le avisa por correo.
    Task<ServiceResult<bool>> AprobarAsync(long idSolicitud, long idRevisor, string? observacion);

    // RRHH rechaza (motivo obligatorio): no se aplica nada, se le devuelve la ventana
    // al empleado para que corrija y reenvíe, y se le avisa por correo con el motivo.
    Task<ServiceResult<bool>> RechazarAsync(long idSolicitud, long idRevisor, string observacion);
}
