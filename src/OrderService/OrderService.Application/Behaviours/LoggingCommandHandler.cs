using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Behaviours;

public class LoggingCommandHandlerDecorator<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly ICommandHandler<TCommand, TResponse> _inner;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> _logger;

    public LoggingCommandHandlerDecorator(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<LoggingCommandHandlerDecorator<TCommand, TResponse>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        _logger.LogInformation("Handling command {CommandName}: {@Command}", commandName, command);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var response = await _inner.Handle(command, cancellationToken);
            stopwatch.Stop();

            _logger.LogInformation("Handled command {CommandName} in {ElapsedMilliseconds} ms",
                commandName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error while handling command {CommandName}", commandName);
            throw;
        }
    }
}