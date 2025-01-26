using System.Net;
using System.Net.Mail;
using Facturation.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

public class MailService : IMailService, IEmailSender
{
    private readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message, byte[] attachment = null)
    {
        var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
        {
            Port = int.Parse(_configuration["Smtp:Port"]),
            Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["Smtp:From"], displayName: "Mini-ERP"),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        // Ajouter la pièce jointe si elle est fournie
        if (attachment != null)
        {
            using (var memoryStream = new MemoryStream(attachment))
            {
                var attachmentFile = new Attachment(memoryStream, "Facture.pdf", "application/pdf");
                mailMessage.Attachments.Add(attachmentFile);
            }
        }

        await smtpClient.SendMailAsync(mailMessage);
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return SendEmailAsync(email, subject, htmlMessage, null); // Appelle la méthode IMailService sans pièce jointe
    }
}