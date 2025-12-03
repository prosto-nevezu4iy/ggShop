using System.Security.Claims;
using CatalogService.Abstractions;
using CatalogService.DTOs;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CatalogService.Endpoints;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGamesApiEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/games");

        api.MapGet("/", GetGames)
            .WithName(nameof(GetGames));

        api.MapGet("/{id:Guid}", GetGameById)
            .WithName(nameof(GetGameById));

        api.MapPost("/", CreateGame)
            .WithName(nameof(CreateGame));

        api.MapPut("/{id:Guid}", UpdateGame)
            .WithName(nameof(UpdateGame));

        api.MapDelete("/{id:Guid}", DeleteGame)
            .WithName(nameof(DeleteGame));

        api.MapPost("/{id:Guid}/ratings", AddRating)
            .WithName(nameof(AddRating));

        api.MapPut("/{id:Guid}/ratings", UpdateRating)
            .WithName(nameof(UpdateRating));

        api.MapDelete("/{id:Guid}/ratings", DeleteRating)
            .WithName(nameof(DeleteRating));

        return app;
    }

    static async Task<Ok<PaginatedItems<GameDto>>> GetGames([AsParameters] GamePagedFilterRequest request, IGameService gameService)
    {
        return TypedResults.Ok(await gameService.GetGamesAsync(request));
    }

    static async Task<Results<Ok<GameDto>, ProblemHttpResult>> GetGameById(Guid id, IGameService gameService)
    {
        var result = await gameService.GetGameByIdAsync(id);

        return result.Match(
            value => TypedResults.Ok(value),
            ApiResults.ProblemForOk
        );
    }

    [Authorize]
    static async Task<Results<Created<GameDto>, ProblemHttpResult>> CreateGame(
        HttpContext httpContext,
        CreateGameDto createGameDto,
        IGameService gameService,
        LinkGenerator linkGenerator)
    {
        var result = await gameService.CreateGameAsync(createGameDto);

        return result.Match(
            value => TypedResults.Created(
                linkGenerator.GetUriByName(httpContext, nameof(GetGameById), new { id = value.Id }), value),
            ApiResults.ProblemForCreated
        );
    }

    [Authorize]
    static async Task<Results<NoContent, ProblemHttpResult>> UpdateGame(
        Guid id,
        UpdateGameDto updateGameDto,
        IGameService gameService)
    {
        var result = await gameService.UpdateGameAsync(id, updateGameDto);

        return result.Match(
            () => TypedResults.NoContent(),
            ApiResults.ProblemForNoContent
        );
    }

    [Authorize]
    static async Task<Results<NoContent, ProblemHttpResult>> DeleteGame(Guid id, IGameService gameService)
    {
        var result = await gameService.DeleteGameAsync(id);

        return result.Match(
            () => TypedResults.NoContent(),
            ApiResults.ProblemForNoContent
        );
    }

    [Authorize]
    static async Task<Results<Created<UserRatingDto>, ProblemHttpResult>> AddRating(
        Guid id,
        HttpContext httpContext,
        CreateUserRatingDto createUserRatingDto,
        IUserRatingService userRatingService,
        LinkGenerator linkGenerator)
    {
        var userId = httpContext.GetUserIdentity();
        var result = await userRatingService.AddUserRatingAsync(id, userId, createUserRatingDto);

        return result.Match(
            value => TypedResults.Created(
                linkGenerator.GetUriByName(httpContext, nameof(GetGameById), new { id = value.GameId }), value),
            ApiResults.ProblemForCreated
        );
    }

    [Authorize]
    static async Task<Results<NoContent, ProblemHttpResult>> UpdateRating(
        Guid id,
        ClaimsPrincipal user,
        UpdateUserRatingDto updateUserRatingDto,
        IUserRatingService userRatingService)
    {
        var userId = user.GetUserIdentity();
        var result = await userRatingService.UpdateUserRatingAsync(id, userId, updateUserRatingDto);

        return result.Match(
            () => TypedResults.NoContent(),
            ApiResults.ProblemForNoContent
        );
    }

    [Authorize]
    static async Task<Results<NoContent, ProblemHttpResult>> DeleteRating(
        Guid id,
        ClaimsPrincipal user,
        IUserRatingService userRatingService)
    {
        var userId = user.GetUserIdentity();
        var result = await userRatingService.DeleteUserRatingAsync(id, userId);

        return result.Match(
            () => TypedResults.NoContent(),
            ApiResults.ProblemForNoContent
        );
    }
}
