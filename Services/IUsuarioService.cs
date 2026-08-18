using Intranet.DTOs;

namespace Intranet.Services;

public interface IUsuarioService
{
    Task<ServiceResult<UsuarioCreadoDto>> CrearUsuarioAsync(CrearUsuarioDto dto);
}