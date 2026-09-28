using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Debes ingresar tu usuario (cédula o correo).")]
    [NoSoloEspacios]
    public string Usuario { get; set; } = null!; // Cédula o Correo

    [Required(ErrorMessage = "Debes ingresar tu contraseña.")]
    public string Contrasena { get; set; } = null!;
}

public class CambiarContrasenaDto
{
    [Required(ErrorMessage = "El identificador de usuario es obligatorio.")]
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "Debes ingresar tu contraseña actual.")]
    public string ContrasenaActual { get; set; } = null!;

    [Required(ErrorMessage = "Debes ingresar la nueva contraseña.")]
    [PasswordSegura(LongitudMinima = 8)]
    public string NuevaContrasena { get; set; } = null!;
}

public class RecuperarContrasenaDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    public string Correo { get; set; } = null!; // Correo personal
}
