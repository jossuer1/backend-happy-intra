using Intranet.DTOs;

namespace Intranet.Services;

public interface IAuthService
{
    Task<ServiceResult<AuthResultDto>> LoginAsync(LoginDto dto);
    Task<ServiceResult<string>> CambiarContrasenaAsync(CambiarContrasenaDto dto);
    Task<ServiceResult<string>> SolicitarRecuperacionAsync(RecuperarContrasenaDto dto);
}