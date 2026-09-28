using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Intranet.Helpers;

/// <summary>
/// Exige que la contraseña tenga al menos 8 caracteres, una mayúscula, una minúscula,
/// un número y un carácter especial. Se usa en los DTOs de autenticación en vez de
/// repetir la validación a mano en cada servicio/controlador.
/// </summary>
public class PasswordSeguraAttribute : ValidationAttribute
{
    private static readonly Regex Mayuscula = new(@"[A-ZÁÉÍÓÚÑ]", RegexOptions.Compiled);
    private static readonly Regex Minuscula = new(@"[a-záéíóúñ]", RegexOptions.Compiled);
    private static readonly Regex Numero = new(@"[0-9]", RegexOptions.Compiled);
    private static readonly Regex Especial = new(@"[^A-Za-z0-9]", RegexOptions.Compiled);

    public int LongitudMinima { get; set; } = 8;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var contrasena = value as string;

        if (string.IsNullOrWhiteSpace(contrasena))
            return new ValidationResult("La contraseña es obligatoria.");

        if (contrasena.Length < LongitudMinima)
            return new ValidationResult($"La contraseña debe tener al menos {LongitudMinima} caracteres.");

        if (!Mayuscula.IsMatch(contrasena))
            return new ValidationResult("La contraseña debe incluir al menos una letra mayúscula.");

        if (!Minuscula.IsMatch(contrasena))
            return new ValidationResult("La contraseña debe incluir al menos una letra minúscula.");

        if (!Numero.IsMatch(contrasena))
            return new ValidationResult("La contraseña debe incluir al menos un número.");

        if (!Especial.IsMatch(contrasena))
            return new ValidationResult("La contraseña debe incluir al menos un carácter especial (por ejemplo: !@#$%&*).");

        return ValidationResult.Success;
    }
}

/// <summary>
/// Evita que un campo de texto obligatorio pase la validación solo con espacios en blanco
/// (por ejemplo Nombre = "   "). [Required] por sí solo no detecta este caso.
/// </summary>
public class NoSoloEspaciosAttribute : ValidationAttribute
{
    public NoSoloEspaciosAttribute()
    {
        ErrorMessage = "Este campo no puede estar vacío.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null) return true; // [Required] ya se encarga de exigir el valor
        if (value is not string texto) return true;
        return !string.IsNullOrWhiteSpace(texto);
    }
}
