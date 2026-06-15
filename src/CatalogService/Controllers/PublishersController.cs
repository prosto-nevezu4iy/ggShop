using CatalogService.Abstractions.Publishers;
using CatalogService.DTOs.Publishers;
using CatalogService.RequestHelpers;
using Common.Application.Pagination;
using Common.Infrastructure.Authorization;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishersController(IPublisherService publisherService) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of publishers based on the provided filter criteria.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 200 OK response with a paginated list of publishers if the request is successful.
    /// Returns a 400 Bad Request response if the request data is invalid.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedItems<PublisherDto>>> GetPublishersAsync([FromQuery] PublisherPagedFilterRequest request)
    {
        var result = await publisherService.GetPublishersAsync(request);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Gets a publisher by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 200 OK response with the publisher details if found
    /// Returns a 404 Not Found response if the publisher does not exist
    /// </returns>
    [HttpGet("{id}", Name = nameof(GetPublisherByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PublisherDto>> GetPublisherByIdAsync(Guid id)
    {
        var result = await publisherService.GetPublisherByIdAsync(id);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Creates a new publisher with the provided details.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 201 Created response with the created publisher details if the creation is successful
    /// Returns a 400 Bad Request response if the request is invalid
    /// Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 422 Unprocessable Entity response if the publisher could not be created due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogCreate)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PublisherDto>> CreatePublisherAsync([FromBody] CreatePublisherDto request)
    {
        var result = await publisherService.CreatePublisherAsync(request);

        return result.Match(
            value => CreatedAtRoute(nameof(GetPublisherByIdAsync), new { id = value.Id }, value),
            ApiResults.Problem
        );
    }

     /// <summary>
    /// Updates an existing publisher with the provided details.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns>
    /// Returns a 204 No Content response if the update is successful
    /// Returns a 400 Bad Request response if the request is invalid
    ///  Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 404 Not Found response if the publisher does not exist
    /// Returns a 422 Unprocessable Entity response if the publisher could not be updated due db errors
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
    public async Task<ActionResult> UpdatePublisherAsync(Guid id, UpdatePublisherDto request)
    {
        var result = await publisherService.UpdatePublisherAsync(id, request);

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Deletes a publisher by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>
    /// Returns a 204 No Content response if the deletion is successful
    /// Returns a 401 Unauthorized response if the user is not authenticated
    /// Returns a 403 Forbidden response if the user does not have the required permissions
    /// Returns a 404 Not Found response if the publisher does not exist
    /// Returns a 422 Unprocessable Entity response if the publisher could not be deleted due db errors
    /// </returns>
    [RequirePermission(PermissionsList.CatalogDelete)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> DeletePublisherAsync(Guid id)
    {
        var result = await publisherService.DeletePublisherAsync(id);

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }
}