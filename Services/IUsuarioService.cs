using Intranet.DTOs;

namespace Intranet.Services;

public interface IUsuarioService
{
    Task<ServiceResult<UsuarioCreadoDto>> CrearUsuarioAsync(CrearUsuarioDto dto);
    Task<ServiceResult<VacacionesUsuarioActualizadoDto>> ActualizarVacacionesAsync(long idUsuario, ActualizarVacacionesUsuarioDto dto);

    Task<ServiceResult<bool>> ActualizarUsuarioAsync(long idUsuario, ActualizarUsuarioDto dto);
}