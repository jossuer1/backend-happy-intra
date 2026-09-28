using Intranet.DTOs;

namespace Intranet.Services;

public interface IUsuarioService
{
    Task<ServiceResult<UsuarioCreadoDto>> CrearUsuarioAsync(CrearUsuarioDto dto);
    Task<ServiceResult<VacacionesUsuarioActualizadoDto>> ActualizarVacacionesAsync(long idUsuario, ActualizarVacacionesUsuarioDto dto);

    Task<ServiceResult<bool>> ActualizarUsuarioAsync(long id, ActualizarUsuarioDto dto);

    // RRHH habilita/deshabilita la ventana de autoedición de perfil.
    Task<ServiceResult<bool>> ActualizarPermisoPerfilAsync(long id, bool habilitar);

    // RRHH activa o desactiva la cuenta de un usuario. Uno desactivado no
    // puede iniciar sesión ni cambiar su contraseña (ver AuthService).
    Task<ServiceResult<bool>> ActualizarEstadoAsync(long id, bool activar);

    // El propio usuario edita su información cuando RRHH le habilitó el permiso.
    // El permiso se consume (vuelve a false) apenas se guarda con éxito.
    Task<ServiceResult<bool>> ActualizarPerfilPropioAsync(long idUsuario, ActualizarPerfilPropioDto dto);
}