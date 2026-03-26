using CatalogService.Abstractions.Genres;
using CatalogService.DTOs.Genres;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Infrastructure.Authorization;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    /// <summary>
    /// Gets a paginated list of genres based on the provided filter criteria.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 200 OK response with a paginated list of genres if the request is successful.
    /// Returns a 400 Bad Request response if the request data is invalid.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedItems<GenreDto>>> GetGenresAsync([FromQuery] GenrePagedFilterRequest request)
    {
        var result = await _genreService.GetGenresAsync(request);

        return result.Match(
            value => Ok(value),
            ApiResults.ProblemForOk1
        );
    }

    /// <summary>
    /// Gets a genre by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 200 OK response with the genre details if found
    /// Returns a 404 Not Found response if the genre does not exist
    /// </returns>
    [HttpGet("{id}", Name = nameof(GetGenreByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenreDto>> GetGenreByIdAsync(Guid id)
    {
        var result = await _genreService.GetGenreByIdAsync(id);

        return result.Match(
            value => Ok(value),
            ApiResults.ProblemForOk1
        );
    }

    /// <summary>
    /// Creates a new genre based on the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 201 Created response with the created genre details if the creation is successful
    /// Returns a 400 Bad Request response if the request is invalid
    /// Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 422 Unprocessable Entity response if the genre could not be created due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogCreate)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<GenreDto>> CreateGenreAsync(CreateGenreDto request)
    {
        var result = await _genreService.CreateGenreAsync(request);

        return result.Match(
            value => CreatedAtRoute(nameof(GetGenreByIdAsync), new { id = value.Id }, value),
            ApiResults.ProblemForOk1
        );
    }

    /// <summary>
    /// Updates an existing genre with the provided details.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 204 No Content response if the update is successful
    /// Returns a 400 Bad Request response if the request is invalid
    ///  Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 404 Not Found response if the genre does not exist
    /// Returns a 422 Unprocessable Entity response if the genre could not be updated due db errors
    ///
    /// </returns>
    [RequirePermission(PermissionsList.CatalogUpdate)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> UpdateGenreAsync(Guid id, UpdateGenreDto request)
    {
        var result = await _genreService.UpdateGenreAsync(id, request);

        return result.Match(
            NoContent,
            ApiResults.ProblemForOk1
        );
    }

    /// <summary>
    /// Deletes a genre by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 204 No Content response if the deletion is successful
    /// Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 404 Not Found response if the genre does not exist
    /// Returns a 422 Unprocessable Entity response if the genre could not be deleted due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogDelete)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> DeleteGenreAsync(Guid id)
    {
        var result = await _genreService.DeleteGenreAsync(id);

        return result.Match(
            NoContent,
            ApiResults.ProblemForOk1
        );
    }
}