using Intranet.DTOs;

namespace Intranet.Services;

public interface IImagenService
{
    Task<ServiceResult<List<ImagenDto>>> ObtenerActivasAsync();
    Task<ServiceResult<List<ImagenDto>>> ObtenerTodasAsync();
    Task<ServiceResult<ImagenDto>> AgregarAsync(ImagenCrearDto dto);
    Task<ServiceResult<ImagenDto>> ActualizarAsync(long id, ImagenActualizarDto dto);
    Task<ServiceResult<bool>> CambiarEstadoAsync(long id, bool estado);
}