using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Intranet.Services;

// Requiere el paquete NuGet: dotnet add package CloudinaryDotNet
public class CloudinaryImagenPerfilService : IImagenPerfilService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImagenPerfilService(IConfiguration configuracion)
    {
        var cuenta = new Account(
            configuracion["Cloudinary:CloudName"],
            configuracion["Cloudinary:ApiKey"],
            configuracion["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(cuenta);
    }

    public async Task<string> SubirImagenAsync(IFormFile archivo, long idUsuario)
    {
        await using var stream = archivo.OpenReadStream();

        var parametros = new ImageUploadParams
        {
            File = new FileDescription(archivo.FileName, stream),

            // PublicId fijo por usuario: cada foto nueva reemplaza a la
            // anterior en Cloudinary en vez de ir acumulando imágenes
            // huérfanas (y de paso la URL final no cambia de "forma").
            PublicId = $"intranet/usuarios/{idUsuario}",
            Overwrite = true,
            Invalidate = true, // limpia la caché del CDN para que se vea la nueva foto de inmediato

            // Recorte cuadrado centrado en el rostro para el avatar
            Transformation = new Transformation()
                .Width(400).Height(400).Crop("fill").Gravity("face"),
        };

        var resultado = await _cloudinary.UploadAsync(parametros);

        if (resultado.Error != null)
        {
            throw new InvalidOperationException(
                $"Error al subir la imagen a Cloudinary: {resultado.Error.Message}");
        }

        return resultado.SecureUrl.ToString();
    }
}