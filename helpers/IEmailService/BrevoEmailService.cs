using System.Text;
using System.Text.Json;

namespace Intranet.Services;

public class BrevoEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BrevoEmailService> _logger;

    public BrevoEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<BrevoEmailService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
    {
        var apiKey = _configuration["BrevoSettings:ApiKey"];
        var senderEmail = _configuration["BrevoSettings:SenderEmail"];
        var senderName = _configuration["BrevoSettings:SenderName"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("No se pudo enviar el correo a {ToEmail}: BrevoSettings:ApiKey está vacío en la configuración.", toEmail);
            return false;
        }

        var payload = new
        {
            sender = new { name = senderName, email = senderEmail },
            to = new[] { new { email = toEmail, name = toName } },
            subject = subject,
            htmlContent = htmlContent
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("api-key", apiKey);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            // Antes esto se perdía en silencio: solo se sabía "falló" (bool false),
            // sin el motivo. Brevo devuelve el detalle del error en el body (por
            // ejemplo, "invalid api-key" o "sender not authorized"), así que lo
            // dejamos en el log para no tener que adivinar la próxima vez.
            var detalle = await response.Content.ReadAsStringAsync();
            _logger.LogError(
                "Brevo respondió {StatusCode} al intentar enviar correo a {ToEmail}. Detalle: {Detalle}",
                (int)response.StatusCode, toEmail, detalle);
        }

        return response.IsSuccessStatusCode;
    }
}