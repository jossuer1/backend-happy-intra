// Helpers/IEmailService/PlantillasCorreo.cs
namespace Intranet.Services;

public static class PlantillasCorreo
{
    public static string BienvenidaConCredenciales(string nombre, string usuario, string claveTemporal, string urlIntranet)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <body style=""margin:0; padding:0; background-color:#f4f4f7; font-family:Segoe UI, Arial, sans-serif;"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f4f7; padding:32px 0;"">
            <tr>
              <td align=""center"">
                <table width=""480"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.06);"">

                  <tr>
                    <td style=""background-color:#FF6B00; padding:32px 24px; text-align:center;"">
                      <h1 style=""margin:0; color:#ffffff; font-size:24px;"">¡Bienvenido a Happy Pay! 🎉</h1>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:32px 24px;"">
                      <p style=""font-size:16px; color:#333333; margin:0 0 16px;"">
                        Hola <strong>{nombre}</strong>,
                      </p>
                      <p style=""font-size:15px; color:#555555; line-height:1.6; margin:0 0 24px;"">
                        ¡Qué gusto tenerte en el equipo! Ya tienes acceso a nuestra intranet, el lugar donde vas a poder
                        conocer a tus compañeros, tu equipo de trabajo y todo lo que pasa por acá dentro.
                      </p>

                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#FFF4EB; border-radius:8px; padding:16px; margin-bottom:24px;"">
                        <tr>
                          <td style=""padding:16px;"">
                            <p style=""margin:0 0 8px; font-size:13px; color:#999999; text-transform:uppercase; letter-spacing:0.5px;"">Tus credenciales de acceso</p>
                            <p style=""margin:0 0 4px; font-size:15px; color:#333333;"">Usuario: <strong>{usuario}</strong></p>
                            <p style=""margin:0; font-size:15px; color:#333333;"">Contraseña temporal: <strong>{claveTemporal}</strong></p>
                          </td>
                        </tr>
                      </table>

                      <p style=""font-size:14px; color:#777777; margin:0 0 24px;"">
                        Por seguridad, te pediremos cambiar esta contraseña la primera vez que ingreses.
                      </p>

                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                          <td align=""center"">
                            <a href=""{urlIntranet}"" style=""display:inline-block; background-color:#FF6B00; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600; padding:14px 32px; border-radius:8px;"">
                              Ingresar a la Intranet
                            </a>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>

                  <tr>
                    <td style=""padding:20px 24px; background-color:#fafafa; text-align:center;"">
                      <p style=""margin:0; font-size:12px; color:#aaaaaa;"">Happy Pay · Este correo fue generado automáticamente</p>
                    </td>
                  </tr>

                </table>
              </td>
            </tr>
          </table>
        </body>
        </html>";
    }
}