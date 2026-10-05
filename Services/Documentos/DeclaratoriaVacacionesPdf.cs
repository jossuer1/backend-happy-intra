using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Intranet.Services.Documentos;

// Documento 3 de 3: Declaración de goce y pago de vacaciones.
public static class DeclaratoriaVacacionesPdf
{
    public static byte[] Generar(DatosDocumentoVacaciones d)
    {
        var portador = d.EsMujer ? "portadora" : "portador";
        var trabajador = d.EsMujer ? "TRABAJADORA" : "TRABAJADOR";

        return Document.Create(contenedor =>
        {
            contenedor.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(10.5f));

                page.Content().Column(col =>
                {
                    col.Spacing(12);

                    PdfComunes.LogoDerecha(col);
                    col.Item().AlignCenter().Text("DECLARACIÓN DE GOCE Y PAGO DE VACACIONES").Bold().FontSize(14);

                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span("Yo, ");
                        t.Span(d.NombreCompleto.ToUpperInvariant()).Bold();
                        t.Span($", {portador} de la cédula de ciudadanía No. ");
                        t.Span(d.Cedula).Bold();
                        t.Span(", en mi calidad de trabajador/a de ");
                        t.Span(d.RazonSocial.ToUpperInvariant()).Bold();
                        t.Span(", de manera libre y voluntaria declaro lo siguiente:");
                    });

                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span("Que hice uso de mis vacaciones correspondientes al período ");
                        t.Span(d.PeriodoLaboral).Bold();
                        t.Span(", desde el ");
                        t.Span(CalculosVacaciones.FechaLarga(d.FechaInicio)).Bold();
                        t.Span(" hasta el ");
                        t.Span(CalculosVacaciones.FechaLarga(d.FechaFin)).Bold();
                        t.Span(", reintegrándome a mis labores el ");
                        t.Span(CalculosVacaciones.FechaLarga(d.FechaReingreso)).Bold();
                        t.Span(".");
                    });

                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span("Asimismo, dejo constancia de que recibí el pago correspondiente a dichas vacaciones, el cual consta registrado en el rol de pagos del mes de ");
                        t.Span($"{d.MesRol} de {d.AnioRol}").Bold();
                        t.Span(", documento que conozco y respecto del cual no tengo observación ni reclamo alguno.");
                    });

                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span(
                        "Por lo expuesto, manifiesto que he disfrutado de mis vacaciones y he recibido el pago correspondiente a las mismas, " +
                        "por lo que no tengo ningún valor pendiente por reclamar respecto del período de vacaciones antes señalado.");
                    });

                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span(
                        "La presente declaración la realizo de forma libre y voluntaria, como constancia de haber gozado de mis " +
                        "vacaciones y de haber recibido el pago correspondiente.");
                    });

                    col.Item().PaddingTop(8).Border(0.8f).Padding(6).Column(c =>
                    {
                        c.Item().Text($"Dado en la ciudad de {d.Ciudad}, a los {d.FechaEmision.Day} días de {CalculosVacaciones.NombreMes(d.FechaEmision.Month)} de {d.FechaEmision.Year}").Bold();
                        c.Item().Text("Aceptado y firmado,");
                    });

                    col.Item().PaddingTop(10).Width(260).Column(c =>
                    {
                        c.Item().PaddingTop(40).LineHorizontal(0.7f);
                        c.Item().Text($"c.c. {d.Cedula}").FontSize(9);
                        c.Item().Text(trabajador).FontSize(9).Bold();
                    });
                });
            });
        }).GeneratePdf();
    }
}
