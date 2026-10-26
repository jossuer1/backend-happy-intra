using System.ComponentModel.DataAnnotations;
using Intranet.Helpers;

namespace Intranet.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Debes ingresar tu usuario (cédula o correo).")]
    [NoSoloEspacios]
    [StringLength(100)]
    public string Usuario { get; set; } = null!; // Cédula o Correo

    [Required(ErrorMessage = "Debes ingresar tu contraseña.")]
    [StringLength(72)]
    public string Contrasena { get; set; } = null!;
}

public class CambiarContrasenaDto
{
    [Required(ErrorMessage = "El identificador de usuario es obligatorio.")]
    [Range(1, long.MaxValue)]
    public long IdUsuario { get; set; }

    [Required(ErrorMessage = "Debes ingresar tu contraseña actual.")]
    [StringLength(72)]
    public string ContrasenaActual { get; set; } = null!;

    [Required(ErrorMessage = "Debes ingresar la nueva contraseña.")]
    [PasswordSegura(LongitudMinima = 8)]
    [StringLength(72)]
    public string NuevaContrasena { get; set; } = null!;
}

public class RecuperarContrasenaDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
    [StringLength(100)]
    public string Correo { get; set; } = null!; // Correo personal
}
