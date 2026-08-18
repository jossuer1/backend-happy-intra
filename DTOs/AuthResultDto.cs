// DTOs/AuthResultDto.cs
namespace Intranet.DTOs;

public class AuthResultDto
{
    public bool DebeCambiarContrasena { get; set; }
    public long IdUsuario { get; set; }
    public string? Token { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? CorreoEmpresa { get; set; }
    public string? Rol { get; set; }
    public string? Cargo { get; set; }
}