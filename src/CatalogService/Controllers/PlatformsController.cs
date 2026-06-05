using CatalogService.Abstractions.Platforms;
using CatalogService.DTOs.Platforms;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Infrastructure.Authorization;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController(IPlatformService platformService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of platforms based on the provided filter criteria.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 200 OK response with a paginated list of platforms if the request is successful.
    /// Returns a 400 Bad Request response if the request data is invalid.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedItems<PlatformDto>>> GetPlatformsAsync([FromQuery] PlatformPagedFilterRequest request)
    {
        var result = await platformService.GetPlatformsAsync(request);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Gets a platform by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 200 OK response with the platform details if found
    /// Returns a 404 Not Found response if the platform does not exist
    /// </returns>
    [HttpGet("{id}", Name = nameof(GetPlatformByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlatformDto>> GetPlatformByIdAsync(Guid id)
    {
        var result = await platformService.GetPlatformByIdAsync(id);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Creates a new platform based on the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 201 Created response with the created platform details if the creation is successful
    /// Returns a 400 Bad Request response if the request is invalid
    /// Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 422 Unprocessable Entity response if the platform could not be created due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogCreate)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PlatformDto>> CreatePlatformAsync(CreatePlatformDto request)
    {
        var result = await platformService.CreatePlatformAsync(request);

        return result.Match(
            value => CreatedAtRoute(nameof(GetPlatformByIdAsync), new { id = value.Id }, value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Updates an existing platform with the provided details.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 204 No Content response if the update is successful
    /// Returns a 400 Bad Request response if the request is invalid
    ///  Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 404 Not Found response if the platform does not exist
    /// Returns a 422 Unprocessable Entity response if the platform could not be updated due db errors
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
    public async Task<ActionResult> UpdatePlatformAsync(Guid id, UpdatePlatformDto request)
    {
        var result = await platformService.UpdatePlatformAsync(id, request);

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Deletes a platform by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 204 No Content response if the deletion is successful
    /// Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 404 Not Found response if the platform does not exist
    /// Returns a 422 Unprocessable Entity response if the platform could not be deleted due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogDelete)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> DeletePlatformAsync(Guid id)
    {
        var result = await platformService.DeletePlatformAsync(id);

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }
}