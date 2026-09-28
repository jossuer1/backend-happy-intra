using System.Globalization;

namespace Intranet.Helpers;

/// <summary>
/// Normaliza los textos que llegan desde los DTOs antes de guardarlos en base de datos,
/// para evitar inconsistencias como espacios de más, o el mismo dato guardado unas veces
/// en mayúsculas y otras en minúsculas.
/// </summary>
public static class TextoHelper
{
    /// <summary>
    /// Recorta espacios y pasa a MAYÚSCULAS. Usar en nombres, apellidos, direcciones,
    /// parentescos, instituciones, y en general cualquier texto "descriptivo" que deba
    /// mantener un orden visual consistente en la base de datos.
    /// </summary>
    public static string? AMayusculas(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return valor is null ? null : string.Empty;

        return valor.Trim().ToUpper(new CultureInfo("es-ES"));
    }

    /// <summary>
    /// Versión no anulable: úsala con campos obligatorios (null! en el modelo).
    /// </summary>
    public static string AMayusculasObligatorio(string valor)
        => AMayusculas(valor) ?? string.Empty;

    /// <summary>
    /// Recorta espacios y pasa a minúsculas. Los correos se normalizan en minúsculas
    /// (y no en mayúsculas) porque así evitamos registrar "Juan@Empresa.com" y
    /// "juan@empresa.com" como si fueran cuentas distintas.
    /// </summary>
    public static string AEmailNormalizado(string valor)
        => valor.Trim().ToLowerInvariant();

    /// <summary>
    /// Solo recorta espacios, sin cambiar mayúsculas/minúsculas. Útil para cédulas,
    /// números de cuenta, teléfonos: no deben "mayuscularse", solo limpiarse.
    /// </summary>
    public static string? SoloTrim(string? valor)
        => valor?.Trim();
}
