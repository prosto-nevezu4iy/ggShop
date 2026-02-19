using System.Security.Claims;
using CatalogService.Abstractions;
using CatalogService.DTOs;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Infrastructure.Authentication;
using Common.Infrastructure.Authorization;
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
            .WithName(nameof(CreateGame))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.CatalogCreate));

        api.MapPut("/{id:Guid}", UpdateGame)
            .WithName(nameof(UpdateGame))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.CatalogUpdate));

        api.MapDelete("/{id:Guid}", DeleteGame)
            .WithName(nameof(DeleteGame))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.CatalogDelete));

        api.MapPost("/{id:Guid}/ratings", AddRating)
            .WithName(nameof(AddRating))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.CatalogCreate));

        api.MapPut("/{id:Guid}/ratings", UpdateRating)
            .WithName(nameof(UpdateRating))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.CatalogUpdate));

        api.MapDelete("/{id:Guid}/ratings", DeleteRating)
            .WithName(nameof(DeleteRating))
            .RequireAuthorization(policy => policy.RequirePermission(PermissionsList.CatalogDelete));

        return app;
    }

    private static async Task<Ok<PaginatedItems<GameDto>>> GetGames(
        [AsParameters] GamePagedFilterRequest request,
        IGameService gameService) =>
        TypedResults.Ok(await gameService.GetGamesAsync(request));

    static async Task<Results<Ok<GameDto>, ProblemHttpResult>> GetGameById(HttpContext httpContext, Guid id, IGameService gameService)
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
        var result = await userRatingService.AddUserRatingAsync(id, httpContext.User.GetUserIdentity(), createUserRatingDto);

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
        var result = await userRatingService.UpdateUserRatingAsync(id, user.GetUserIdentity(), updateUserRatingDto);

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
        var result = await userRatingService.DeleteUserRatingAsync(id, user.GetUserIdentity());

        return result.Match(
            () => TypedResults.NoContent(),
            ApiResults.ProblemForNoContent
        );
    }
}
