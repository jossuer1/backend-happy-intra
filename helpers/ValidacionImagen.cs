namespace Intranet.Helpers;

public static class ValidacionImagen
{
    public const long MaximoBytes = 5 * 1024 * 1024;
    public static async Task<string?> ValidarAsync(IFormFile? archivo)
    {
        if (archivo == null || archivo.Length == 0) return "Selecciona una imagen.";
        if (archivo.Length > MaximoBytes) return "La imagen no puede superar los 5 MB.";
        await using var stream = archivo.OpenReadStream();
        var cabecera = new byte[12];
        var leidos = await stream.ReadAtLeastAsync(cabecera, 12, throwOnEndOfStream: false);
        var tipo = DetectarTipo(cabecera.AsSpan(0, leidos));
        if (tipo == null || !string.Equals(tipo, archivo.ContentType, StringComparison.OrdinalIgnoreCase))
            return "El contenido debe corresponder a una imagen JPG, PNG o WEBP del tipo indicado.";
        return null;
    }

    public static string? DetectarTipo(ReadOnlySpan<byte> datos)
    {
        if (datos.Length >= 8 && datos[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) return "image/png";
        if (datos.Length >= 3 && datos[0] == 255 && datos[1] == 216 && datos[2] == 255) return "image/jpeg";
        if (datos.Length >= 12 && datos[..4].SequenceEqual("RIFF"u8) && datos[8..12].SequenceEqual("WEBP"u8)) return "image/webp";
        return null;
    }
}
