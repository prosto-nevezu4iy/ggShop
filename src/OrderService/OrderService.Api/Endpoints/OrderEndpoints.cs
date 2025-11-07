using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.Api.Extensions;
using OrderService.Application.Commands.CancelOrder;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Application.Queries.GetOrdersByUser;

namespace OrderService.Api.Endpoints;

public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrdersApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders");

        api.MapGet("/", GetOrdersByUserAsync);

        api.MapGet("/{id:Guid}", GetOrderByIdAsync);

        api.MapPost("/", CreateOrderAsync);

        api.MapPut("/{id:Guid}/cancel", CancelOrderAsync);

        return api;
    }

    private static async Task<Ok<IEnumerable<OrderDto>>> GetOrdersByUserAsync(
        HttpContext context,
        IQueryHandler<GetOrdersByUserQuery, IEnumerable<OrderDto>> handler,
        CancellationToken ct)
    {
        var userId = context.GetUserIdentity();
        var query = new GetOrdersByUserQuery(userId);

        var orders = await handler.Handle(query, ct);

        return TypedResults.Ok(orders);
    }

    private static async Task<Ok<OrderDto>> GetOrderByIdAsync(
        Guid id,
        HttpContext context,
        IQueryHandler<GetOrderByIdQuery, OrderDto> handler,
        CancellationToken ct)
    {
        var userId = context.GetUserIdentity();
        var query = new GetOrderByIdQuery(id, userId);
        var order = await handler.Handle(query, ct);

        return TypedResults.Ok(order);
    }

    private static async Task<Created<OrderDto>> CreateOrderAsync(
        HttpContext context,
        ICommandHandler<CreateOrderCommand, OrderDto> handler,
        CancellationToken ct)
    {
        var token = context.GetToken();
        var command = new CreateOrderCommand(token, context.GetUserIdentity());
        var order = await handler.Handle(command, ct);

        return TypedResults.Created($"/api/orders/{order.Id}", order);
    }

    private static async Task<Results<Ok, ProblemHttpResult>> CancelOrderAsync(
        Guid id,
        HttpContext context,
        ICommandHandler<CancelOrderCommand, bool> handler,
        CancellationToken ct)
    {
        var userId = context.GetUserIdentity();
        var command = new CancelOrderCommand(id, userId);
        var result = await handler.Handle(command, ct);

        if (!result)
        {
            return TypedResults.Problem("Cancel order failed.");
        }

        return TypedResults.Ok();
    }
}