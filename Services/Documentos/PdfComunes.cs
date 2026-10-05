using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Intranet.Services.Documentos;

internal static class PdfComunes
{
    public const string ColorMarca = "#1e5a63";

    // El logo vive en Assets/logo-happypay.png (se copia al publicar, ver .csproj).
    // Si el archivo no está, los PDFs siguen generándose con el nombre en texto.
    private static readonly Lazy<byte[]?> _logo = new(() =>
    {
        try
        {
            var ruta = Path.Combine(AppContext.BaseDirectory, "Assets", "logo-happypay.png");
            return File.Exists(ruta) ? File.ReadAllBytes(ruta) : null;
        }
        catch { return null; }
    });

    public static void LogoDerecha(ColumnDescriptor col)
    {
        var logo = _logo.Value;
        if (logo != null)
            col.Item().AlignRight().Width(70).Image(logo);
        else
            col.Item().AlignRight().Text("HappyPay").Bold().FontSize(16).FontColor(ColorMarca);
    }

    // Línea de firma: raya arriba, texto centrado y "C.I." / "c.c." abajo.
    public static void BloqueFirma(ColumnDescriptor col, string titulo, string? lineaExtra = null, string? documento = "C.I.")
    {
        col.Item().PaddingTop(40).LineHorizontal(0.7f);
        col.Item().AlignCenter().Text(titulo).FontSize(9);
        if (!string.IsNullOrEmpty(lineaExtra))
            col.Item().AlignCenter().Text(lineaExtra).FontSize(9);
        if (!string.IsNullOrEmpty(documento))
            col.Item().Text(documento).FontSize(9);
    }
}
