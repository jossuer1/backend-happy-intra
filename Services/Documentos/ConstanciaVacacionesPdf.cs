using Intranet.DTOs;
using Intranet.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Intranet.Services.Documentos;

// Genera la constancia de una solicitud de vacaciones YA APROBADA, en PDF.
// A propósito no hay ningún método "Guardar": el documento se arma en memoria
// (byte[]) cada vez que se solicita y se descarta apenas se envía al navegador.
// Si algo del registro cambia (por ejemplo, un ajuste posterior de saldo), la
// siguiente descarga siempre refleja la información más reciente.
public static class ConstanciaVacacionesPdf
{
    public static byte[] Generar(SolicitudVacacion solicitud, SaldoVacacionesDto? saldoActual)
    {
        var documento = Document.Create(contenedor =>
        {
            contenedor.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(11));

                // --- Encabezado ---
                page.Header().Column(col =>
                {
                    col.Item().Text("HAPPY PAY").FontSize(20).Bold().FontColor("#FF6B00");
                    col.Item().Text("Constancia de Vacaciones Aprobadas").FontSize(14).SemiBold();
                    col.Item().PaddingTop(6).LineHorizontal(1).LineColor("#FF6B00");
                });

                // --- Cuerpo ---
                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(10);

                    var nombreCompleto = $"{solicitud.Usuario.Nombre} {solicitud.Usuario.Apellido}";
                    var cargo = solicitud.Usuario.Cargo?.Nombre;

                    col.Item().Text(texto =>
                    {
                        texto.Span("Este documento certifica que el/la colaborador(a) ");
                        texto.Span(nombreCompleto).Bold();
                        texto.Span($", con cédula {solicitud.Usuario.Cedula}");
                        if (!string.IsNullOrWhiteSpace(cargo))
                            texto.Span($", cargo de {cargo},");
                        texto.Span(" tiene aprobado el siguiente período de vacaciones:");
                    });

                    col.Item().PaddingTop(6).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(180);
                            columns.RelativeColumn();
                        });

                        void Fila(string etiqueta, string valor)
                        {
                            table.Cell().PaddingVertical(3).Text(etiqueta).SemiBold();
                            table.Cell().PaddingVertical(3).Text(valor);
                        }

                        Fila("Fecha de inicio:", solicitud.FechaInicio.ToString("dd/MM/yyyy"));
                        Fila("Fecha de fin:", solicitud.FechaFin.ToString("dd/MM/yyyy"));
                        Fila("Días tomados:", $"{solicitud.DiasSolicitados} día(s)");
                        Fila("Motivo:", solicitud.Motivo);
                        Fila("Fecha de la solicitud:", solicitud.FechaSolicitud.ToString("dd/MM/yyyy"));

                        Fila("Aprobado por (jefe directo):",
                            solicitud.JefeAprobador != null
                                ? $"{solicitud.JefeAprobador.Nombre} {solicitud.JefeAprobador.Apellido}"
                                : "-");
                        Fila("Fecha de aprobación del jefe:",
                            solicitud.FechaRespuestaJefe?.ToString("dd/MM/yyyy HH:mm") ?? "-");

                        Fila("Aprobado por (RRHH):",
                            solicitud.RrhhAprobador != null
                                ? $"{solicitud.RrhhAprobador.Nombre} {solicitud.RrhhAprobador.Apellido}"
                                : "-");
                        Fila("Fecha de aprobación de RRHH:",
                            solicitud.FechaRespuestaRrhh?.ToString("dd/MM/yyyy HH:mm") ?? "-");

                        if (saldoActual != null)
                            Fila("Saldo disponible restante:", $"{saldoActual.DiasDisponibles} día(s)");
                    });

                    col.Item().PaddingTop(20).Text(
                            "Este documento se genera automáticamente a partir de la información registrada " +
                            "en la intranet de Happy Pay y no requiere firma física.")
                        .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                });

                // --- Pie de página ---
                page.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });

        return documento.GeneratePdf();
    }
}
