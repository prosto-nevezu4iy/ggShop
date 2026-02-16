using brevo_csharp.Api;
using brevo_csharp.Model;
using IdentityService.Configurations;
using Microsoft.Extensions.Options;
using Configuration = brevo_csharp.Client.Configuration;
using Task = System.Threading.Tasks.Task;
using static IdentityService.Constants.IdentityConstants;

namespace IdentityService.Services;

public class EmailSender(
    IOptions<AuthMessageSenderOptions> optionsAccessor,
    ILogger<EmailSender> logger)
    : IEmailSender
{
    private AuthMessageSenderOptions Options { get; } = optionsAccessor.Value;

    public async Task SendEmailAsync(string email, Dictionary<string, string> parameters, int templateId)
    {
        await Execute(Options.BrevoKey, email, parameters, templateId);
    }

    private async Task Execute(string brevoKey, string email, Dictionary<string, string> parameters, int templateId)
    {
        if (!Configuration.Default.ApiKey.ContainsKey(ApiKey))
        {
            Configuration.Default.AddApiKey(ApiKey, brevoKey);
        }

        var apiInstance = new TransactionalEmailsApi();
        var sendSmtpEmail = new SendSmtpEmail
        {
            To = [new(email, parameters[Username])],
            TemplateId = templateId,
            Params = parameters,
        };

        try
        {
            await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
            logger.LogInformation($"Email to {email} queued successfully!");
        }
        catch (Exception e)
        {
            logger.LogError("Exception when calling TransactionalEmailsApi.SendTransacEmail: " + e.Message );
        }
    }
}