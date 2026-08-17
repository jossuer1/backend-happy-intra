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

    public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
    {
        var apiKey = _configuration["BrevoSettings:ApiKey"];
        var senderEmail = _configuration["BrevoSettings:SenderEmail"];
        var senderName = _configuration["BrevoSettings:SenderName"];

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
        return response.IsSuccessStatusCode;
    }
}