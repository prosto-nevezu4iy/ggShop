using CatalogService.DTOs;
using CatalogService.RequestHelpers;
using CatalogService.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CatalogService.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        api.MapGet("/", GetGames);

        api.MapGet("/{id:Guid}", GetGameById);

        api.MapPost("/", CreateGame);

        api.MapPut("/{id:Guid}", UpdateGame);

        api.MapDelete("/{id:Guid}", DeleteGame);

        return app;
    }

    public static async Task<Ok<PaginatedItems<GameDto>>> GetGames([AsParameters] SearchParams request, ICatalogService catalogService)
    {
        return TypedResults.Ok(await catalogService.GetGamesAsync(request));
    }

    public static async Task<Ok<GameDto>> GetGameById(Guid id, ICatalogService catalogService)
    {
        var game = await catalogService.GetGameByIdAsync(id);
        return TypedResults.Ok(game);
    }

    public static async Task<Results<Created<Guid>, ValidationProblem>> CreateGame(CreateGameDto createGameDto, IValidator<CreateGameDto> validator, ICatalogService catalogService)
    {
        var result = await validator.ValidateAsync(createGameDto);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var createdGameId = await catalogService.CreateGameAsync(createGameDto);

        return TypedResults.Created($"/api/catalog/{createdGameId}", createdGameId);
    }

    public static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateGame(Guid id, UpdateGameDto updateGameDto, IValidator<UpdateGameDto> validator, ICatalogService catalogService)
    {
        var result = await validator.ValidateAsync(updateGameDto);

        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        var game = await catalogService.GetGameEntityWithPlatformsByIdAsync(id);

        if (game is null)
        {
            return TypedResults.NotFound();
        }

        await catalogService.UpdateGameAsync(game, updateGameDto);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteGame(Guid id, ICatalogService catalogService)
    {
        var game = await catalogService.GetGameEntityByIdAsync(id);

        if (game is null)
        {
            return TypedResults.NotFound();
        }

        await catalogService.DeleteGameAsync(game);

        return TypedResults.NoContent();
    }
}
