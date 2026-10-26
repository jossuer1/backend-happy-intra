using Intranet.DTOs;

namespace Intranet.Services;

public interface IUsuarioService
{
    Task<ServiceResult<UsuarioCreadoDto>> CrearUsuarioAsync(CrearUsuarioDto dto);
    Task<ServiceResult<VacacionesUsuarioActualizadoDto>> ActualizarVacacionesAsync(long idUsuario, ActualizarVacacionesUsuarioDto dto);

    Task<ServiceResult<bool>> ActualizarUsuarioAsync(long id, ActualizarUsuarioDto dto);

    // RRHH habilita/deshabilita la ventana de autoedición de perfil de un empleado.
    // Al habilitar exige fecha límite y avisa por correo al empleado.
    Task<ServiceResult<bool>> ActualizarPermisoPerfilAsync(long id, bool habilitar, DateTime? fechaLimite);

    // RRHH habilita la ventana a todos los activos o a una lista de usuarios, con una misma fecha límite.
    Task<ServiceResult<HabilitarActualizacionMasivaResultadoDto>> HabilitarActualizacionMasivaAsync(
        HabilitarActualizacionMasivaDto dto);

    // RRHH envía un recordatorio a quienes tienen la ventana abierta (aún no respondieron),
    // y opcionalmente amplía su fecha de referencia.
    Task<ServiceResult<RecordatorioActualizacionResultadoDto>> EnviarRecordatorioActualizacionAsync(
        RecordatorioActualizacionDto dto);

    // RRHH activa o desactiva la cuenta de un usuario. Uno desactivado no
    // puede iniciar sesión ni cambiar su contraseña (ver AuthService).
    Task<ServiceResult<bool>> ActualizarEstadoAsync(long id, bool activar);
}
