using System.Net;
using System.Net.Mail;
using Facturation.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

public class MailService : IMailService, IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MailService> _logger;

    public MailService(IConfiguration configuration, ILogger<MailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message, byte[] attachment = null)
    {
        try
        {
            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
                EnableSsl = true,
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:From"], "Mini-ERP"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            if (attachment != null)
            {
                var memoryStream = new MemoryStream(attachment);
                memoryStream.Position = 0; // Important : réinitialiser la position du stream
                var attachmentFile = new Attachment(memoryStream, "Facture.pdf", "application/pdf");
                mailMessage.Attachments.Add(attachmentFile);
            }

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email envoyé avec succès à {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erreur d'envoi d'email: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return SendEmailAsync(email, subject, htmlMessage, null);
    }
}