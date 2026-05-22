using System.Text;
using System.Text.Json;

namespace EngenhariasSenac.Helpers;

public class SendEmail
{
    private static readonly HttpClient _httpClient = new();

    public static void Send(string personalEmail, string institucionalEmail, string subject, string body)
    {
        var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY")
            ?? throw new InvalidOperationException("BREVO_API_KEY não definido no .env");
        var emailFrom = Environment.GetEnvironmentVariable("SMTP_FROM")
            ?? throw new InvalidOperationException("SMTP_FROM não definido no .env");

        var recipients = new List<object> { new { email = institucionalEmail } };
        if (!string.IsNullOrWhiteSpace(personalEmail) && personalEmail != institucionalEmail)
        {
            recipients.Add(new { email = personalEmail });
        }

        var payload = new
        {
            sender = new { name = "Comitê das Engenharias Senac", email = emailFrom },
            to = recipients,
            subject,
            htmlContent = body
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
        request.Headers.Add("api-key", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = _httpClient.Send(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            throw new Exception($"Brevo API error {(int)response.StatusCode}: {error}");
        }
    }
}
