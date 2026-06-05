using Contracts;
using MassTransit;
using ShoppingCartService.Abstractions;

namespace ShoppingCartService.Consumers;

public class UserLoggedInConsumer(IShoppingCartRepository shoppingCartRepository, ILogger<UserLoggedInConsumer> logger)
    : IConsumer<UserLoggedIn>
{
    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        try
        {
            logger.LogInformation("--> Consuming UserLoggedIn: User {UserId}", context.Message.Id);

            await shoppingCartRepository.TransferAnonymousCartAsync(context.Message.AnonymousId, context.Message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while moving shoppingCart for User {UserId} from {AnonId} user", context.Message.Id, context.Message.AnonymousId);
        }
    }
}