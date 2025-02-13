namespace Facturation.Services;

public interface IMailService
{
    Task SendEmailAsync(string email, string subject, string message, byte[] attachment = null);
}