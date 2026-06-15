using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Presentation.Extensions;

public static class ControllerExtensions
{
    public static void AddCommonControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
}