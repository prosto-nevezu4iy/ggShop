using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Behaviours;

public class LoggingQueryHandlerDecorator<TQuery, TResponse>
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    private readonly IQueryHandler<TQuery, TResponse> _inner;
    private readonly ILogger<LoggingQueryHandlerDecorator<TQuery, TResponse>> _logger;

    public LoggingQueryHandlerDecorator(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<LoggingQueryHandlerDecorator<TQuery, TResponse>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        _logger.LogInformation("Handling query {QueryName}: {@Query}", queryName, query);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var response = await _inner.Handle(query, cancellationToken);
            stopwatch.Stop();

            _logger.LogInformation("Handled query {QueryName} in {ElapsedMilliseconds} ms",
                queryName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error while handling query {QueryName}", queryName);
            throw;
        }
    }
}