using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Intranet.Models;

namespace Intranet.Services.Documentos;

// Documento 2 de 3: Acta de vacaciones (empleador y trabajador).
public static class ActaVacacionesPdf
{
    public static byte[] Generar(DatosDocumentoVacaciones d)
    {
        var trabajador = d.EsMujer ? "LA TRABAJADORA" : "EL TRABAJADOR";
        var tituloPersona = d.EsMujer ? "la señora" : "el señor";
        var portador = d.EsMujer ? "portadora" : "portador";
        var nacionalidad = Nacionalidades.Gentilicio(d.Nacionalidad, d.EsMujer);

        return Document.Create(contenedor =>
        {
            contenedor.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.2f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(10.5f));

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    PdfComunes.LogoDerecha(col);
                    col.Item().AlignCenter().Text("ACTA DE VACACIONES").Bold().FontSize(14);

                    col.Item().Text("Comparecientes. –").Bold();
                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span("Comparecen a la celebración acta, por una parte, la empresa ");
                        t.Span($"{d.RazonSocial.ToUpperInvariant()}, ").Bold();
                        t.Span("representada legalmente por el señor ");
                        t.Span(d.RepresentanteLegal.ToUpperInvariant()).Bold();
                        t.Span(", que en lo sucesivo y para los efectos de esta acta se la denominará el ¨EMPLEADOR¨, y, por otra parte, ");
                        t.Span($"{tituloPersona} ");
                        t.Span(d.NombreCompleto.ToUpperInvariant()).Bold();
                        t.Span($", de nacionalidad {nacionalidad}, {portador} de la cédula de ciudadanía No. ");
                        t.Span(d.Cedula).Bold();
                        t.Span($", quien comparece por sus propios y personales derechos y se le denominará ¨{trabajador}¨. " +
                               "Ambos, libre y voluntariamente, convienen en celebrar la presente acta de conformidad con las siguientes cláusulas:");
                    });

                    col.Item().Text("PRIMERA. – Antecedentes y Periodo de Vacaciones:").Bold();
                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span("Considerando que el Código de Trabajo en su artículo 69 y siguientes, establece que los empleados privados " +
                               "tendrán derecho a gozar anualmente de un periodo ininterrumpido de quince días, incluidos los días no laborables; " +
                               "y siendo que las presentes vacaciones corresponden al periodo ");
                        t.Span(d.PeriodoLaboral).Bold();
                        t.Span(", las partes estipulan por mutuo acuerdo que las mismas sean concedidas en las siguientes fechas:");
                    });

                    col.Item().PaddingLeft(10).Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.ConstantColumn(180); c.RelativeColumn(); });

                        void Fila(string etiqueta, string valor)
                        {
                            t.Cell().PaddingVertical(3).Text(etiqueta).Bold();
                            t.Cell().PaddingVertical(3).Text(valor);
                        }

                        Fila("FECHA DE INICIO:", CalculosVacaciones.FechaLarga(d.FechaInicio));
                        Fila("FECHA DE CULMINACIÓN:", CalculosVacaciones.FechaLarga(d.FechaFin));
                        Fila("FECHA DE INGRESO:", CalculosVacaciones.FechaLarga(d.FechaReingreso));
                        Fila("DÍAS GOZADOS:", d.DiasGozados.ToString());
                        Fila("DÍAS PENDIENTES:", d.DiasPendientes.ToString());
                    });

                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span(
                        $"Luego de cumplido este periodo {(d.EsMujer ? "la trabajadora" : "el trabajador")} ingresará a sus actividades " +
                        "laborales normales. En caso de inasistencia se procederá con las sanciones establecidas en la ley.");
                    });

                    col.Item().Text("SEGUNDA. - Declaraciones Finales. –").Bold();
                    col.Item().Text(t =>
                    {
                        t.Justify();
                        t.Span(
                        "Las partes aceptan y ratifican el contenido de las cláusulas que anteceden, sin reserva de ninguna clase " +
                        "y por convenir a sus intereses, y lo suscriben en dos ejemplares.");
                    });

                    col.Item().PaddingTop(8).Border(0.8f).Padding(6).Column(c =>
                    {
                        c.Item().Text($"Dado en la ciudad de {d.Ciudad}, a los {d.FechaEmision.Day} días de {CalculosVacaciones.NombreMes(d.FechaEmision.Month)} de {d.FechaEmision.Year}").Bold();
                        c.Item().Text("Aceptado y firmado,");
                    });

                    col.Item().PaddingTop(10).Row(r =>
                    {
                        r.Spacing(40);
                        r.RelativeItem().Column(c =>
                        {
                            c.Item().PaddingTop(40).LineHorizontal(0.7f);
                            c.Item().Text(d.RepresentanteLegal.ToUpperInvariant()).FontSize(9).Bold();
                            c.Item().Text("GERENTE GENERAL").FontSize(9);
                            c.Item().Text(d.RazonSocial.ToUpperInvariant()).FontSize(9).Bold();
                        });
                        r.RelativeItem().Column(c =>
                        {
                            c.Item().PaddingTop(40).LineHorizontal(0.7f);
                            c.Item().Text($"c.c. {d.Cedula}").FontSize(9);
                            c.Item().Text(d.EsMujer ? "TRABAJADORA" : "TRABAJADOR").FontSize(9).Bold();
                        });
                    });
                });
            });
        }).GeneratePdf();
    }
}
