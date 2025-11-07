using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure;

namespace OrderService.Application.Commands.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, bool>
{
    private readonly OrderingContext _dbContext;

    public CancelOrderCommandHandler(OrderingContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == command.OrderId && o.UserId == command.UserId, cancellationToken: cancellationToken);

        if (order == null)
        {
            return false;
        }

        order.SetCancelledStatus();
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}