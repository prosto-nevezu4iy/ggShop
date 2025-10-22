using Contracts;
using MassTransit;
using ShoppingCartService.Entities;
using ShoppingCartService.Repositories;

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
            var msg = context.Message;
            _logger.LogInformation("--> Consuming UserLoggedIn: User {UserId}", msg.Id);

            var anonCart = await _shoppingCartRepository.GetBasketAsync(msg.AnonymousId.ToString());
            if (anonCart == null)
            {
                _logger.LogInformation("No anonymous cart for {AnonId}", msg.AnonymousId);
                return;
            }

            var userCart = await _shoppingCartRepository.GetBasketAsync(msg.Id.ToString()) ?? new UserShoppingCart { UserId = msg.Id.ToString(),};

            foreach (var item in anonCart.Items)
            {
                var existing = userCart.Items.FirstOrDefault(i => i.GameId == item.GameId);
                if (existing != null)
                {
                    existing.Quantity += item.Quantity;
                }
                else
                {
                    userCart.Items.Add(item);
                }
            }

            await _shoppingCartRepository.UpdateBasketAsync(userCart);
            await _shoppingCartRepository.DeleteBasketAsync(msg.AnonymousId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while moving shoppingCart for User {UserId} from {AnonId} user", context.Message.Id, context.Message.AnonymousId);
        }
    }
}