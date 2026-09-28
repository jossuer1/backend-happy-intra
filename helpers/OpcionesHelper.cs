using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Intranet.Helpers;

/// <summary>
/// Ayuda a validar valores contra las listas fijas de Models/OpcionesFijas.cs.
/// </summary>
public static class OpcionesHelper
{
    /// <summary>
    /// Devuelve el valor canónico de la lista si <paramref name="valor"/> coincide con alguno,
    /// sin importar mayúsculas/minúsculas, espacios de más ni tildes ("Cónyuge" -> "CONYUGE").
    /// Devuelve null si está vacío o no coincide con ninguno.
    /// </summary>
    public static string? Normalizar(string? valor, IEnumerable<string> validas)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var clave = Clave(valor);
        return validas.FirstOrDefault(v => Clave(v) == clave);
    }

    private static string Clave(string texto)
    {
        var descompuesto = texto.Trim().ToUpperInvariant().Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder(descompuesto.Length);
        foreach (var c in descompuesto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return Regex.Replace(sb.ToString(), @"\s+", " ");
    }
}
