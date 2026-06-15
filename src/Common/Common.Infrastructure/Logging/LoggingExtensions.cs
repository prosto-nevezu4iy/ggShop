using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static void AddCommonLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));
    }
}