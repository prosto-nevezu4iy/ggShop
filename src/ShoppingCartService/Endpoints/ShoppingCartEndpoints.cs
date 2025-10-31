using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.DTOs;
using ShoppingCartService.Entities;
using ShoppingCartService.Services;

namespace ShoppingCartService.Endpoints;

public static class ShoppingCartEndpoints
{
    public static IEndpointRouteBuilder MapShoppingCartApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/shopping-cart");

        api.MapGet("/", GetShoppingCart);

        api.MapPost("/", AddShoppingCart);

        api.MapPut("/{GameId:Guid}", UpdateShoppingCartItem);

        api.MapDelete("/", DeleteShoppingCart);

        return app;
    }

    private static async Task<Ok<ShoppingCartResponse>> GetShoppingCart(HttpContext context, IShoppingCartService shoppingCartService)
    {
        return TypedResults.Ok(await shoppingCartService.GetShoppingCartAsync(context));
    }

    private static async Task<Ok<ShoppingCartResponse>> AddShoppingCart(AddCartItemRequest request, HttpContext context, IShoppingCartService shoppingCartService)
    {
        return TypedResults.Ok(await shoppingCartService.AddItemToCartAsync(request, context));
    }

    private static async Task<Ok<ShoppingCartResponse>> UpdateShoppingCartItem([FromRoute] Guid gameId, HttpContext context, IShoppingCartService shoppingCartService)
    {
        return TypedResults.Ok(await shoppingCartService.UpdateItemQuantityAsync(gameId, context));
    }

    private static async Task<Results<NoContent, Ok>> DeleteShoppingCart(HttpContext context, IShoppingCartService shoppingCartService)
    {
        await shoppingCartService.DeleteShoppingCartAsync(context);

        return TypedResults.NoContent();
    }
}