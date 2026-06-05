using CatalogService.Abstractions.Games;
using CatalogService.DTOs.Games;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Infrastructure.Authorization;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController(IGameService gameService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of games based on the provided filter criteria.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 200 OK response with a paginated list of games if the request is successful.
    /// Returns a 400 Bad Request response if the request data is invalid.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedItems<GameDto>>> GetGamesAsync([FromQuery] GamePagedFilterRequest request)
    {
        var result = await gameService.GetGamesAsync(request);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Gets a game by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 200 OK response with the game details if found
    /// Returns a 404 Not Found response if the game does not exist
    /// </returns>
    [HttpGet("{id}", Name = nameof(GetGameByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetGameByIdAsync(Guid id)
    {
        var result = await gameService.GetGameByIdAsync(id);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Creates a new game based on the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 201 Created response with the created game details if the request is successful.
    /// Returns a 400 Bad Request response if the request data is invalid.
    /// Returns a 401 Unauthorized response if the user is not authenticated.
    /// Returns a 403 Forbidden response if the user does not have permission to create a game.
    /// Returns a 422 Unprocessable Entity response if the game could not be deleted due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogCreate)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GameDto>> CreateGameAsync(CreateGameDto request)
    {
        var result = await gameService.CreateGameAsync(request);

        return result.Match(
            value => CreatedAtRoute(nameof(GetGameByIdAsync), new { id = value.Id }, value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Updates an existing game with the provided details.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 204 No Content response if the update is successful.
    /// Returns a 400 Bad Request response if the request data is invalid.
    /// Returns a 401 Unauthorized response if the user is not authenticated.
    /// Returns a 403 Forbidden response if the user does not have permission to update a game.
    /// Returns a 404 Not Found response if the game does not exist.
    /// Returns a 422 Unprocessable Entity response if the game could not be deleted due db errors.
    /// </returns>
    [RequirePermission(PermissionsList.CatalogUpdate)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> UpdateGameAsync(Guid id, UpdateGameDto request)
    {
        var result = await gameService.UpdateGameAsync(id, request);

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Deletes a game by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 204 No Content response if the deletion is successful.
    /// Returns a 401 Unauthorized response if the user is not authenticated.
    /// Returns a 403 Forbidden response if the user does not have permission to delete a game.
    /// Returns a 404 Not Found response if the game does not exist.
    /// Returns a 422 Unprocessable Entity response if the game could not be deleted due db errors.
    /// </returns>
    [RequirePermission(PermissionsList.CatalogDelete)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> DeleteGameAsync(Guid id)
    {
        var result = await gameService.DeleteGameAsync(id);

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }
}