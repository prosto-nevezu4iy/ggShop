using Microsoft.Extensions.Options;

namespace CatalogService.Configurations;

public class CloudinarySettingsValidation : IValidateOptions<CloudinarySettings>
{
    public ValidateOptionsResult Validate(string name, CloudinarySettings options) => options switch
    {
        { CloudName: null or "" } => ValidateOptionsResult.Fail($"{nameof(CloudinarySettings.CloudName)} is required."),
        { ApiKey: null or "" } => ValidateOptionsResult.Fail($"{nameof(CloudinarySettings.ApiKey)} is required."),
        { ApiSecret: null or "" } => ValidateOptionsResult.Fail($"{nameof(CloudinarySettings.ApiSecret)} is required."),
        _ => ValidateOptionsResult.Success
    };
}
