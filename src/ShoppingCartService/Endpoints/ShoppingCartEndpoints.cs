using Common.Infrastructure.Authorization;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.Abstractions;
using ShoppingCartService.DTOs;
using ShoppingCartService.Extensions;

namespace ShoppingCartService.Endpoints;

public static class ShoppingCartEndpoints
{
    public static IEndpointRouteBuilder MapShoppingCartApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/shopping-cart");

        api.MapGet("/", GetShoppingCart)
            .WithName(nameof(GetShoppingCart));

        api.MapPost("/", AddItemToBasket)
            .WithName(nameof(AddItemToBasket))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.ShoppingCartCreate));

        api.MapPut("/{GameId:Guid}", UpdateShoppingCartItem)
            .WithName(nameof(UpdateShoppingCartItem))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.ShoppingCartUpdate));

        api.MapDelete("/", DeleteShoppingCart)
            .WithName(nameof(DeleteShoppingCart))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.ShoppingCartDelete));

        #if DEBUG
                api.MapPost("/seed/{userCount:int}", SeedShoppingCarts);
        #endif

        return app;
    }

    static async Task<Ok<ShoppingCartDto>> GetShoppingCart(HttpContext context, IShoppingCartService shoppingCartService)
    {
        return TypedResults.Ok(await shoppingCartService.GetShoppingCartAsync(context.GetUserId()));
    }

    static async Task<Results<Created<ShoppingCartDto>, ProblemHttpResult>> AddItemToBasket(HttpContext httpContext, LinkGenerator linkGenerator, CreateShoppingCartItemDto dto, IShoppingCartService shoppingCartService)
    {
        var result = await shoppingCartService.AddItemToShoppingCartAsync(httpContext.GetUserId(), dto);

        return result.Match(
            value => TypedResults.Created(
                linkGenerator.GetUriByName(httpContext, nameof(GetShoppingCart)),
                value),
            ApiResults.ProblemForCreated
        );
    }

    static async Task<Results<Ok<ShoppingCartDto>, ProblemHttpResult>> UpdateShoppingCartItem([FromRoute] Guid gameId, HttpContext context, UpdateShoppingCartItemDto dto, IShoppingCartService shoppingCartService)
    {
        var result = await shoppingCartService.UpdateQuantityAsync(context.GetUserId(), gameId, dto);

        return result.Match(
            value => TypedResults.Ok(value),
            ApiResults.ProblemForOk
        );
    }

    static async Task<Results<NoContent, ProblemHttpResult>> DeleteShoppingCart(HttpContext context, IShoppingCartService shoppingCartService)
    {
        var result = await shoppingCartService.DeleteShoppingCartAsync(context.GetUserId());

        return result.Match(
            () => TypedResults.NoContent(),
            ApiResults.ProblemForNoContent
        );
    }

    static async Task<Ok<string>> SeedShoppingCarts(
        int userCount,
        IShoppingCartService shoppingCartService)
    {
        const int batchSize = 10;
        var tasks = new List<Task>(batchSize);

        for (int i = 0; i < userCount; i++)
        {
            var userId = Guid.NewGuid();
            var dto = new CreateShoppingCartItemDto(
                Guid.Parse("019b5a7b-3ebe-7629-a162-42191e95fd8e"),
                "Baldur's Gate 3",
                1,
                "test"
            );

            tasks.Add(shoppingCartService.AddItemToShoppingCartAsync(userId, dto));

            if (tasks.Count is batchSize)
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
                Console.WriteLine($"Seeded {i + 1}/{userCount}");
            }
        }

        if (tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }

        return TypedResults.Ok($"Seeded {userCount} users");
    }
}