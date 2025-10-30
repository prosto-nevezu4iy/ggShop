using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Behaviours;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.Scan(scan => scan.FromAssembliesOf(typeof(ServiceExtensions))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        serviceCollection.Decorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));
        serviceCollection.Decorate(typeof(IQueryHandler<,>), typeof(LoggingQueryHandlerDecorator<,>));
    }
}