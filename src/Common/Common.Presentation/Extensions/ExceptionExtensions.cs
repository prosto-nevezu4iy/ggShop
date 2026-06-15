using Common.Presentation.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Presentation.Extensions;

public static class ExceptionExtensions
{
    public static void AddCommonExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }
}