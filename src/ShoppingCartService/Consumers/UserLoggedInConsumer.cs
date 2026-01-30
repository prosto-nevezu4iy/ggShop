using Contracts;
using MassTransit;
using ShoppingCartService.Abstractions;

namespace ShoppingCartService.Consumers;

public class UserLoggedInConsumer : IConsumer<UserLoggedIn>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ILogger<UserLoggedInConsumer> _logger;

    public UserLoggedInConsumer(IShoppingCartRepository shoppingCartRepository, ILogger<UserLoggedInConsumer> logger)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        try
        {
            _logger.LogInformation("--> Consuming UserLoggedIn: User {UserId}", context.Message.Id);

            await _shoppingCartRepository.TransferAnonymousCartAsync(context.Message.AnonymousId, context.Message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while moving shoppingCart for User {UserId} from {AnonId} user", context.Message.Id, context.Message.AnonymousId);
        }
    }
}