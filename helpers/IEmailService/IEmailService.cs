namespace Intranet.Services;

// Archivo adjunto para un correo (ej. los PDFs de vacaciones).
public record EmailAdjunto(string Nombre, byte[] Contenido);

public interface IEmailService
{
    Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent);

    Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent,
        IEnumerable<EmailAdjunto>? adjuntos);
}
