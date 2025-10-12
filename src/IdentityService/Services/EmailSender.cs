using brevo_csharp.Api;
using brevo_csharp.Model;
using IdentityService.Configurations;
using IdentityService.Models;
using Microsoft.Extensions.Options;
using Configuration = brevo_csharp.Client.Configuration;
using Task = System.Threading.Tasks.Task;

namespace IdentityService.Services;

public class EmailSender(
    IOptions<AuthMessageSenderOptions> optionsAccessor,
    ILogger<EmailSender> logger)
    : IEmailSender
{
    private readonly ILogger<EmailSender> _logger = logger;
    private AuthMessageSenderOptions Options { get; } = optionsAccessor.Value;

    public async Task SendEmailAsync(string email, Dictionary<string, string> parameters, int templateId)
    {
        if (string.IsNullOrEmpty(Options.BrevoKey))
        {
            throw new Exception("Null BrevoKey");
        }

        await Execute(Options.BrevoKey, email, parameters, templateId);
    }

    private async Task Execute(string brevoKey, string email, Dictionary<string, string> parameters, int templateId)
    {
        if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
        {
            Configuration.Default.AddApiKey("api-key", brevoKey);
        }

        var apiInstance = new TransactionalEmailsApi();
        var sendSmtpEmail = new SendSmtpEmail
        {
            To = [new(email, parameters["USERNAME"])],
            TemplateId = templateId,
            Params = parameters,
        };

        try
        {
            CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            _logger.LogInformation($"Email to {email} queued successfully!");
        }
        catch (Exception e)
        {
            _logger.LogError("Exception when calling TransactionalEmailsApi.SendTransacEmail: " + e.Message );
        }
    }
}