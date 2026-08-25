using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Intranet.Services;

// Requiere el paquete NuGet: dotnet add package CloudinaryDotNet
public class CloudinaryUploadService : ICloudinaryUploadService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryUploadService(IConfiguration configuracion)
    {
        var cuenta = new Account(
            configuracion["Cloudinary:CloudName"],
            configuracion["Cloudinary:ApiKey"],
            configuracion["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(cuenta);
    }

    public async Task<string> SubirBannerAsync(IFormFile archivo)
    {
        await using var stream = archivo.OpenReadStream();

        var parametros = new ImageUploadParams
        {
            File = new FileDescription(archivo.FileName, stream),

            PublicId = $"intranet/banners/{Guid.NewGuid()}",
            Overwrite = false,

            Transformation = new Transformation()
                .Width(1600).Crop("limit"),
        };

        var resultado = await _cloudinary.UploadAsync(parametros);

        if (resultado.Error != null)
        {
            throw new InvalidOperationException(
                $"Error al subir el banner a Cloudinary: {resultado.Error.Message}");
        }

        return resultado.SecureUrl.ToString();
    }
}
