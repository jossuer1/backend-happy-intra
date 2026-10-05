using System.Text;
using System.Text.Json;

namespace Intranet.Services;

public class BrevoEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public BrevoEmailService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
        => SendEmailAsync(toEmail, toName, subject, htmlContent, null);

    public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent,
        IEnumerable<EmailAdjunto>? adjuntos)
    {
        var apiKey = _configuration["BrevoSettings:ApiKey"];
        var senderEmail = _configuration["BrevoSettings:SenderEmail"];
        var senderName = _configuration["BrevoSettings:SenderName"];

        // Brevo recibe los adjuntos como { name, content } con el contenido en base64.
        var listaAdjuntos = adjuntos?
            .Select(a => new { name = a.Nombre, content = Convert.ToBase64String(a.Contenido) })
            .ToArray();

        var payload = new Dictionary<string, object>
        {
            ["sender"] = new { name = senderName, email = senderEmail },
            ["to"] = new[] { new { email = toEmail, name = toName } },
            ["subject"] = subject,
            ["htmlContent"] = htmlContent
        };

        if (listaAdjuntos is { Length: > 0 })
            payload["attachment"] = listaAdjuntos;

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("api-key", apiKey);

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}
