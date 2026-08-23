namespace Intranet.DTOs;

public class UsuarioCreadoDto
{
    public long IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string UsuarioAcceso { get; set; } = null!;
    public string CorreoEmpresa { get; set; } = null!;
    public string CorreoPersonal { get; set; } = null!;
    public string ClaveTemporal { get; set; } = null!;
    public bool DebeCambiarContrasena { get; set; }
}