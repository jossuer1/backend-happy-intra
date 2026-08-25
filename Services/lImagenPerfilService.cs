using Microsoft.AspNetCore.Http;

namespace Intranet.Services;

public interface IImagenPerfilService
{
    Task<string> SubirImagenAsync(IFormFile archivo, long idUsuario);
}