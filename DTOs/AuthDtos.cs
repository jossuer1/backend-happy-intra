namespace Intranet.DTOs;

public class LoginDto
{
    public string Usuario { get; set; } = null!; // Cédula o Correo
    public string Contrasena { get; set; } = null!;
}

public class CambiarContrasenaDto
{
    public long IdUsuario { get; set; }
    public string ContrasenaActual { get; set; } = null!;
    public string NuevaContrasena { get; set; } = null!;
}

public class RecuperarContrasenaDto
{
    public string Correo { get; set; } = null!; // Correo personal
}