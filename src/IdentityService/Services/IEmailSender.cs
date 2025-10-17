namespace IdentityService.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, Dictionary<string, string> parameters, int templateId);
}