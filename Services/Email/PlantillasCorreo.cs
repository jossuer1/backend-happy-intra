// Helpers/IEmailService/PlantillasCorreo.cs
using System.Net;

namespace Intranet.Services;

public static class PlantillasCorreo
{
  public static string AvisoSolicitudVacaciones(string nombre, string empleado, long id,
    DateTime inicio, DateTime fin, int dias, string? observacion, string url, bool nueva)
  {
    var titulo = nueva ? "Nueva solicitud de vacaciones" : "Solicitud de vacaciones rechazada";
    var mensaje = nueva
      ? $"{WebUtility.HtmlEncode(empleado)} envió una solicitud de vacaciones para tu revisión."
      : "Tu jefe directo rechazó tu solicitud de vacaciones. No se descontaron días de tu saldo.";
    var detalle = string.IsNullOrWhiteSpace(observacion) ? ""
      : $"<p><strong>{(nueva ? "Motivo" : "Observación del jefe")}:</strong> {WebUtility.HtmlEncode(observacion)}</p>";
    var cuerpo = $"""
      <p>Hola <strong>{WebUtility.HtmlEncode(nombre)}</strong>,</p>
      <p>{mensaje}</p>
      <p><strong>Solicitud:</strong> #{id}<br/>
      <strong>Desde:</strong> {inicio:dd/MM/yyyy}<br/>
      <strong>Hasta:</strong> {fin:dd/MM/yyyy}<br/>
      <strong>Días:</strong> {dias}</p>
      {detalle}
      {BotonIntranet(WebUtility.HtmlEncode(url))}
      """;
    return MarcoVacaciones(titulo, cuerpo);
  }
  public static string BienvenidaConCredenciales(string nombre, string usuario, string claveTemporal, string urlIntranet)
  {
    return $@"
        <!DOCTYPE html>
        <html>
        <body style=""margin:0; padding:0; background-color:#f8fafc; font-family:Segoe UI, Arial, sans-serif;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f8fafc; padding:32px 0;"">
            <tr>
              <td align=""center"">
                <table width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.06);"">

                  <tr>
                    <td style=""background-color:#1e5a63; padding:32px 24px; text-align:center;"">
                      <h1 style=""margin:0; color:#ffffff; font-size:24px;"">¡Bienvenido a Happy Pay! 🎉</h1>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:32px 24px;"">
                      <p style=""font-size:16px; color:#334155; margin:0 0 16px;"">
                        Hola <strong>{nombre}</strong>,
                      </p>
                      <p style=""font-size:15px; color:#475569; line-height:1.6; margin:0 0 24px;"">
                        ¡Qué gusto tenerte en el equipo! Ya tienes acceso a nuestra intranet, el lugar donde vas a poder
                        conocer a tus compañeros, tu equipo de trabajo y todo lo que pasa por acá dentro.
                      </p>

                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px; padding:16px; margin-bottom:24px;"">
                        <tr>
                          <td style=""padding:16px;"">
                            <p style=""margin:0 0 8px; font-size:13px; color:#64748b; text-transform:uppercase; letter-spacing:0.5px;"">Tus credenciales de acceso</p>
                            <p style=""margin:0 0 4px; font-size:15px; color:#1e293b;"">Usuario: <strong>{usuario}</strong></p>
                            <p style=""margin:0; font-size:15px; color:#1e293b;"">Contraseña temporal: <strong>{claveTemporal}</strong></p>
                          </td>
                        </tr>
                      </table>

                      <p style=""font-size:14px; color:#64748b; margin:0 0 24px;"">
                        Por seguridad, te pediremos cambiar esta contraseña la primera vez que ingreses.
                      </p>

                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                          <td align=""center"">
                            <a href=""{urlIntranet}"" style=""display:inline-block; background-color:#1e5a63; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600; padding:14px 32px; border-radius:8px;"">
                              Ingresar a la Intranet
                            </a>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:20px 24px; background-color:#f8fafc; border-top:1px solid #e2e8f0; text-align:center;"">
                      <p style=""margin:0; font-size:12px; color:#94a3b8;"">Happy Pay · Este correo fue generado automáticamente</p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>";
  }

  public static string RecuperacionContrasena(string nombre, string claveTemporal, string urlIntranet)
  {
    return $@"
        <!DOCTYPE html>
        <html>
        <body style=""margin:0; padding:0; background-color:#f8fafc; font-family:Segoe UI, Arial, sans-serif;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f8fafc; padding:32px 0;"">
            <tr>
              <td align=""center"">
                <table width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.06);"">

                  <tr>
                    <td style=""background-color:#1e5a63; padding:32px 24px; text-align:center;"">
                      <h1 style=""margin:0; color:#ffffff; font-size:24px;"">Recuperación de contraseña 🔐</h1>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:32px 24px;"">
                      <p style=""font-size:16px; color:#334155; margin:0 0 16px;"">
                        Hola <strong>{nombre}</strong>,
                      </p>
                      <p style=""font-size:15px; color:#475569; line-height:1.6; margin:0 0 24px;"">
                        Recibimos una solicitud para restablecer tu contraseña de la intranet. Generamos una nueva
                        contraseña temporal para que puedas volver a ingresar.
                      </p>

                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px; padding:16px; margin-bottom:24px;"">
                        <tr>
                          <td style=""padding:16px;"">
                            <p style=""margin:0 0 8px; font-size:13px; color:#64748b; text-transform:uppercase; letter-spacing:0.5px;"">Tu nueva contraseña temporal</p>
                            <p style=""margin:0; font-size:15px; color:#1e293b;"">Contraseña temporal: <strong>{claveTemporal}</strong></p>
                          </td>
                        </tr>
                      </table>

                      <p style=""font-size:14px; color:#64748b; margin:0 0 24px;"">
                        Por seguridad, te pediremos cambiar esta contraseña apenas ingreses. Si tú no solicitaste este
                        cambio, contacta de inmediato a RRHH.
                      </p>

                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                          <td align=""center"">
                            <a href=""{urlIntranet}"" style=""display:inline-block; background-color:#1e5a63; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600; padding:14px 32px; border-radius:8px;"">
                              Ingresar a la Intranet
                            </a>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:20px 24px; background-color:#f8fafc; border-top:1px solid #e2e8f0; text-align:center;"">
                      <p style=""margin:0; font-size:12px; color:#94a3b8;"">Happy Pay · Este correo fue generado automáticamente</p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>";
  }

  public static string Cumpleanos(string nombre)
  {
    return $@"
        <!DOCTYPE html>
        <html>
        <body style=""margin:0; padding:0; background-color:#f8fafc; font-family:Segoe UI, Arial, sans-serif;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f8fafc; padding:32px 0;"">
            <tr>
              <td align=""center"">
                <table width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.06);"">

                  <tr>
                    <td style=""background-color:#1e5a63; padding:32px 24px; text-align:center;"">
                      <h1 style=""margin:0; color:#ffffff; font-size:24px;"">¡Feliz cumpleaños, {nombre}! 🎉🎂</h1>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:32px 24px;"">
                      <p style=""font-size:16px; color:#334155; margin:0 0 16px;"">
                        Hola <strong>{nombre}</strong>,
                      </p>
                      <p style=""font-size:15px; color:#475569; line-height:1.6; margin:0 0 16px;"">
                        Todo el equipo de <strong>Happy Pay</strong> quiere desearte un muy feliz cumpleaños.
                        Gracias por ser parte de este equipo y aportar todos los días con tu trabajo.
                      </p>
                      <p style=""font-size:15px; color:#475569; line-height:1.6; margin:0 0 24px;"">
                        ¡Que tengas un día increíble rodeado de las personas que quieres! 🥳
                      </p>
                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px;"">
                        <tr>
                          <td style=""padding:16px; text-align:center;"">
                            <p style=""margin:0; font-size:15px; color:#1e5a63; font-weight:600;"">Con cariño, tu equipo de Happy Pay 💚</p>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:20px 24px; background-color:#f8fafc; border-top:1px solid #e2e8f0; text-align:center;"">
                      <p style=""margin:0; font-size:12px; color:#94a3b8;"">Happy Pay · Este correo fue generado automáticamente</p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>";
  }

  // Marco común (mismo estilo que los demás correos) para los avisos de vacaciones.
  private static string MarcoVacaciones(string titulo, string cuerpoHtml)
  {
    return $"""
        <!DOCTYPE html>
        <html>
        <body style="margin:0; padding:0; background-color:#f8fafc; font-family:Segoe UI, Arial, sans-serif;">
          <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f8fafc; padding:32px 0;">
            <tr>
              <td align="center">
                <table width="520" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.06);">
                  <tr>
                    <td style="background-color:#1e5a63; padding:28px 24px; text-align:center;">
                      <h1 style="margin:0; color:#ffffff; font-size:22px;">{titulo}</h1>
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:28px 24px; font-size:15px; color:#475569; line-height:1.6;">
                      {cuerpoHtml}
                    </td>
                  </tr>
                  <tr>
                    <td style="padding:20px 24px; background-color:#f8fafc; border-top:1px solid #e2e8f0; text-align:center;">
                      <p style="margin:0; font-size:12px; color:#94a3b8;">Happy Pay · Este correo fue generado automáticamente</p>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;
  }

  // Para el empleado: aprobación del jefe + qué hacer con los 3 documentos adjuntos.
  public static string VacacionesAprobadasEmpleado(string nombre, string fechaInicio, string fechaFin,
      string fechaReingreso, int dias, int diasPendientes)
  {
    var cuerpo = $"""
      <p style="margin:0 0 16px;">Hola <strong>{nombre}</strong>,</p>
      <p style="margin:0 0 16px;">Tu jefe directo aprobó tus vacaciones:</p>
      <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px; margin-bottom:16px;">
        <tr><td style="padding:16px; color:#1e5a63;">
          <strong>Desde:</strong> {fechaInicio}<br/>
          <strong>Hasta:</strong> {fechaFin}<br/>
          <strong>Regresas a laborar:</strong> {fechaReingreso}<br/>
          <strong>Días:</strong> {dias} &nbsp;·&nbsp; <strong>Días pendientes:</strong> {diasPendientes}
        </td></tr>
      </table>
      <p style="margin:0 0 8px;"><strong>Qué debes hacer ahora:</strong></p>
      <ol style="margin:0 0 16px; padding-left:20px;">
        <li>Imprime los 3 documentos adjuntos (también puedes descargarlos desde la intranet): solicitud, acta y declaración.</li>
        <li>Fírmalos y haz que tu jefe inmediato también los firme.</li>
        <li><strong>Entrégalos a Talento Humano</strong> (en físico o escaneados por correo).</li>
      </ol>
      <p style="margin:0;">Los días ya fueron descontados de tu saldo.</p>
      """;
    return MarcoVacaciones("Tus vacaciones fueron aprobadas ✅", cuerpo);
  }

  // Para RRHH: solo un aviso, sin adjuntos.
  public static string VacacionesAprobadasRrhh(string nombreRrhh, string empleado, string fechaInicio,
      string fechaFin, string fechaReingreso, int dias)
  {
    var cuerpo = $"""
      <p style="margin:0 0 16px;">Hola <strong>{nombreRrhh}</strong>,</p>
      <p style="margin:0 0 16px;">El jefe directo aprobó las vacaciones de <strong>{empleado}</strong>:</p>
      <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px; margin-bottom:16px;">
        <tr><td style="padding:16px; color:#1e5a63;">
          <strong>Desde:</strong> {fechaInicio}<br/>
          <strong>Hasta:</strong> {fechaFin}<br/>
          <strong>Regresa a laborar:</strong> {fechaReingreso}<br/>
          <strong>Días:</strong> {dias}
        </td></tr>
      </table>
      <p style="margin:0;">Los días ya se descontaron del saldo. Ten presente que el colaborador debe hacerte llegar los documentos firmados.</p>
      """;
    return MarcoVacaciones($"Vacaciones aprobadas: {empleado}", cuerpo);
  }

  // ---------------------------------------------------------------------
  // Autoedición de perfil (ventana habilitada por RRHH + resolución de cambios)
  // ---------------------------------------------------------------------

  private static string BotonIntranet(string url)
    => $"""
      <table width="100%" cellpadding="0" cellspacing="0" style="margin-top:8px;">
        <tr><td align="center">
          <a href="{url}" style="display:inline-block; background-color:#1e5a63; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600; padding:14px 32px; border-radius:8px;">
            Ir a la Intranet
          </a>
        </td></tr>
      </table>
      """;

  public static string ActualizacionPerfilHabilitada(string nombre, string fechaLimite, string urlIntranet)
  {
    var cuerpo = $"""
      <p style="margin:0 0 16px;">Hola <strong>{WebUtility.HtmlEncode(nombre)}</strong>,</p>
      <p style="margin:0 0 16px;">Recursos Humanos habilitó la actualización de tu información personal en la intranet.</p>
      <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px; margin-bottom:16px;">
        <tr><td style="padding:16px; color:#1e5a63;">
          Puedes actualizarla <strong>hasta el {fechaLimite}</strong>.
        </td></tr>
      </table>
      <p style="margin:0 0 16px;">Podrás revisar tus datos de contacto, estado civil, familiares, contactos de emergencia y títulos. Tus cambios serán revisados por RRHH antes de aplicarse.</p>
      {BotonIntranet(urlIntranet)}
      """;
    return MarcoVacaciones("Actualiza tu información 📝", cuerpo);
  }

  // Recordatorio amable: sin lenguaje de sanción. fechaReferencia puede ser null.
  public static string RecordatorioActualizacionPerfil(string nombre, string? fechaReferencia, string urlIntranet)
  {
    var referencia = string.IsNullOrWhiteSpace(fechaReferencia)
      ? ""
      : $"""
      <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f0f7f8; border:1px solid #d0e3e5; border-radius:8px; margin-bottom:16px;">
        <tr><td style="padding:16px; color:#1e5a63;">
          La fecha de referencia era el <strong>{fechaReferencia}</strong>.
        </td></tr>
      </table>
      """;
    var cuerpo = $"""
      <p style="margin:0 0 16px;">Hola <strong>{WebUtility.HtmlEncode(nombre)}</strong>,</p>
      <p style="margin:0 0 16px;">Recursos Humanos te recuerda que aún está pendiente que revises y actualices tu información personal en la intranet.</p>
      {referencia}
      <p style="margin:0 0 16px;">Si tus datos ya están al día, también puedes confirmarlo con un clic desde la intranet. Tus cambios serán revisados por RRHH antes de aplicarse.</p>
      {BotonIntranet(urlIntranet)}
      """;
    return MarcoVacaciones("Recordatorio: actualiza tu información 🔔", cuerpo);
  }

  public static string CambiosPerfilAprobados(string nombre, string? observacion)
  {
    var nota = string.IsNullOrWhiteSpace(observacion)
      ? ""
      : $"<p style=\"margin:16px 0 0;\"><strong>Comentario de RRHH:</strong> {WebUtility.HtmlEncode(observacion)}</p>";
    var cuerpo = $"""
      <p style="margin:0 0 16px;">Hola <strong>{WebUtility.HtmlEncode(nombre)}</strong>,</p>
      <p style="margin:0;">Recursos Humanos <strong>aprobó</strong> los cambios que enviaste a tu información personal. Ya están reflejados en tu perfil.</p>
      {nota}
      """;
    return MarcoVacaciones("Tus cambios fueron aprobados ✅", cuerpo);
  }

  public static string CambiosPerfilRechazados(string nombre, string motivo, string fechaLimite, string urlIntranet)
  {
    var cuerpo = $"""
      <p style="margin:0 0 16px;">Hola <strong>{WebUtility.HtmlEncode(nombre)}</strong>,</p>
      <p style="margin:0 0 16px;">Recursos Humanos <strong>no aprobó</strong> los cambios que enviaste a tu información personal.</p>
      <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#fff7ed; border:1px solid #fed7aa; border-radius:8px; margin-bottom:16px;">
        <tr><td style="padding:16px; color:#9a3412;">
          <strong>Motivo:</strong> {WebUtility.HtmlEncode(motivo)}
        </td></tr>
      </table>
      <p style="margin:0 0 16px;">Volvimos a habilitar tu edición: puedes corregir y enviar de nuevo <strong>hasta el {fechaLimite}</strong>.</p>
      {BotonIntranet(urlIntranet)}
      """;
    return MarcoVacaciones("Tus cambios no fueron aprobados", cuerpo);
  }
}
