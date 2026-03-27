using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace EngenhariasSenac.Helpers;

public class SendEmail
{
    public static void Send(string personalEmail, string institucionalEmail, string subject, string body)
    {
        MailMessage emailMessage = new MailMessage();
        var emailFrom = Environment.GetEnvironmentVariable("SMTP_FROM") ?? throw new InvalidOperationException("SMTP_FROM não definido no .env");
        var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? throw new InvalidOperationException("SMTP_HOST não definido no .env");
        var smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new InvalidOperationException("SMTP_PASSWORD não definido no .env");
        try
        {
            var smtpClient = new SmtpClient(smtpHost, smtpPort);
            smtpClient.EnableSsl = true;
            smtpClient.Timeout = 60000;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(emailFrom, smtpPassword);
            emailMessage.From = new MailAddress(emailFrom, "Comitê das Engenharias Senac");
            emailMessage.Body = body;
            emailMessage.Subject = subject;
            emailMessage.IsBodyHtml = true;
            emailMessage.Priority = MailPriority.Normal;
            emailMessage.To.Add(institucionalEmail);

            if (!string.IsNullOrWhiteSpace(personalEmail) && personalEmail != institucionalEmail)
            {
                emailMessage.To.Add(personalEmail);
            }

            smtpClient.Send(emailMessage);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
