using Contracts;
using MassTransit;
using ShoppingCartService.Abstractions;

namespace ShoppingCartService.Consumers;

public class GameDeletedConsumer : IConsumer<GameDeleted>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILogger<GameDeletedConsumer> _logger;

    public GameDeletedConsumer(IShoppingCartRepository shoppingCartRepository, ILogger<GameDeletedConsumer> logger)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GameDeleted> context)
    {
        try
        {
            _logger.LogInformation("--> Consuming GameDeleted event: {Id}", context.Message.Id);

            await _shoppingCartRepository.DeleteShoppingCartsByGameIdAsync(context.Message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting game {GameId} from carts", context.Message.Id);
        }
    }
}