using Microsoft.AspNetCore.Http;

namespace Intranet.Services;

public interface ICloudinaryUploadService
{
    Task<string> SubirBannerAsync(IFormFile archivo);
}