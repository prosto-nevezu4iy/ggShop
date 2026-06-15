using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Common.Presentation.Extensions;

public static class MiddlewareExtensions
{
    public static void UseCommonMiddlewarePipeline(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSerilogRequestLogging();
        app.UseExceptionHandler();
        app.MapControllers();
    }
}