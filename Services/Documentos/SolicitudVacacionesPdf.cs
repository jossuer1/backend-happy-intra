using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Intranet.Services.Documentos;

// Documento 1 de 3: Solicitud de vacaciones (formato HappyPay).
public static class SolicitudVacacionesPdf
{
    public static byte[] Generar(DatosDocumentoVacaciones d)
    {
        return Document.Create(contenedor =>
        {
            contenedor.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.8f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(9.5f));

                page.Content().Column(col =>
                {
                    col.Spacing(7);

                    PdfComunes.LogoDerecha(col);
                    col.Item().AlignRight().Text($"{d.Ciudad}, {CalculosVacaciones.FechaLarga(d.FechaEmision)}");

                    col.Item().Text("Señor/a");
                    col.Item().Text(d.NombreJefe).Bold();
                    col.Item().Text($"JEFE DE {d.Area.ToUpperInvariant()}");
                    col.Item().Text(d.RazonSocial.ToUpperInvariant());

                    col.Item().PaddingTop(6).AlignCenter()
                        .Text("SOLICITUD DE VACACIONES").Bold().FontSize(12).FontColor(PdfComunes.ColorMarca).Underline();

                    col.Item().Border(0.8f).Padding(6).AlignCenter().Text(
                        "De conformidad al Art. 69 y siguientes del Código del Trabajo, SOLICITO de manera formal " +
                        "se autorice el goce de mis vacaciones, de acuerdo al siguiente desglose de información:");

                    // --- Datos del trabajador ---
                    col.Item().PaddingTop(4).Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.ConstantColumn(190); c.RelativeColumn(); });

                        void Fila(string etiqueta, string valor)
                        {
                            t.Cell().PaddingVertical(2).Text(etiqueta).FontColor(PdfComunes.ColorMarca).Underline();
                            t.Cell().PaddingVertical(2).BorderBottom(0.6f).Text(valor);
                        }

                        Fila("NOMBRE COMPLETO:", d.NombreCompleto);
                        Fila("CÉDULA:", d.Cedula);
                        Fila("ÁREA O DEPARTAMENTO:", d.Area);
                        Fila("CARGO:", d.Cargo);
                        Fila("FECHA DE INGRESO:", d.FechaIngreso.HasValue ? CalculosVacaciones.FechaLarga(d.FechaIngreso.Value) : "");
                        Fila("DÍAS DE VACACIONES DISPONIBLES:", d.DiasDisponiblesAntes.ToString());
                    });

                    col.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text("Solicito mis vacaciones correspondientes al período laboral:");
                        r.ConstantItem(110).Border(0.6f).AlignCenter().Text(d.PeriodoLaboral).Bold();
                    });

                    // --- Desde / Hasta ---
                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(90);
                            c.ConstantColumn(60);
                            c.ConstantColumn(90);
                            c.ConstantColumn(60);
                        });

                        t.Cell().Text("");
                        t.Cell().AlignCenter().Text("día").FontSize(8).Italic();
                        t.Cell().AlignCenter().Text("mes").FontSize(8).Italic();
                        t.Cell().AlignCenter().Text("año").FontSize(8).Italic();

                        void Fila(string etiqueta, DateTime f)
                        {
                            t.Cell().PaddingVertical(2).Text(etiqueta).Bold().FontColor(PdfComunes.ColorMarca);
                            t.Cell().Border(0.6f).AlignCenter().Text(f.Day.ToString());
                            t.Cell().Border(0.6f).AlignCenter().Text(CalculosVacaciones.NombreMes(f.Month));
                            t.Cell().Border(0.6f).AlignCenter().Text(f.Year.ToString());
                        }

                        Fila("DESDE:", d.FechaInicio);
                        Fila("HASTA:", d.FechaFin);
                    });

                    col.Item().Row(r =>
                    {
                        r.ConstantItem(150).Text("Total de días solicitados");
                        r.ConstantItem(50).Border(0.6f).AlignCenter().Text(d.DiasGozados.ToString()).Bold();
                        r.RelativeItem().PaddingLeft(8).Text("días");
                    });

                    col.Item().Text("Observaciones:").Underline();
                    col.Item().Border(0.8f).Padding(6).MinHeight(36).Text(d.Observacion);

                    col.Item().PaddingTop(4).Row(r =>
                    {
                        r.RelativeItem().Text("Confirmo que he coordinado la cobertura de mis funciones durante mi ausencia.");
                        r.ConstantItem(60).Border(0.6f).AlignCenter().Text("SI").Bold();
                    });

                    // --- Firmas ---
                    col.Item().PaddingTop(10).Row(r =>
                    {
                        r.Spacing(40);
                        r.RelativeItem().Column(c => PdfComunes.BloqueFirma(c, "Firma del trabajador"));
                        r.RelativeItem().Column(c => PdfComunes.BloqueFirma(c, "Firma de jefe inmediato"));
                    });

                    col.Item().PaddingTop(10).AlignCenter().Width(200).Column(c =>
                        PdfComunes.BloqueFirma(c, "Talento Humano", "Fecha de registro recibido.", null));

                    // --- Condiciones ---
                    col.Item().PaddingTop(14).Text("Condiciones:").Bold().FontSize(8.5f).Underline();
                    col.Item().Border(0.8f).Padding(4).Column(c =>
                    {
                        c.Item().Text(t =>
                        {
                            t.Span("1. El documento debe ser entregado al ");
                            t.Span("Departamento de Talento Humano").Bold();
                            t.Span(" con la debida antelación para su validación.");
                        });
                        c.Item().Text("2. El jefe de área debe confirmar la disponibilidad y ajuste de las fechas solicitadas, asegurando la cobertura del área.");
                        c.Item().Text("3. Las vacaciones serán autorizadas únicamente después de la firma de Talento Humano.");
                    });
                });
            });
        }).GeneratePdf();
    }
}
