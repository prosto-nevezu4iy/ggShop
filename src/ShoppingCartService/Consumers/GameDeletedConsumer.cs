using Contracts;
using MassTransit;
using ShoppingCartService.Abstractions;

namespace ShoppingCartService.Consumers;

public class GameDeletedConsumer(IShoppingCartRepository shoppingCartRepository, ILogger<GameDeletedConsumer> logger)
    : IConsumer<GameDeleted>
{
    public async Task Consume(ConsumeContext<GameDeleted> context)
    {
        try
        {
            logger.LogInformation("--> Consuming GameDeleted event: {Id}", context.Message.Id);

            await shoppingCartRepository.DeleteShoppingCartsByGameIdAsync(context.Message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while deleting game {GameId} from carts", context.Message.Id);
        }
    }
}