using System.Diagnostics;
using Common.Presentation;
using Common.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.Abstractions;
using ShoppingCartService.DTOs;
using ShoppingCartService.Extensions;

namespace ShoppingCartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController(IShoppingCartService shoppingCartService, LinkGenerator linkGenerator) : ControllerBase
{
    /// <summary>
    /// Gets a shopping cart.
    /// </summary>
    /// <returns>
    /// Returns a 200 OK response with a shopping cart if the request is successful.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCartDto>> GetShoppingCartAsync()
    {
        return Ok(await shoppingCartService.GetShoppingCartAsync(HttpContext.GetUserId()));
    }

    /// <summary>
    /// Adds an item to shopping cart based on the provided details.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>
    /// Returns a 201 Created response with the created shopping cart if the request is successful.
    /// Returns a 404 Not Found response if the shopping cart does not exist.
    /// Returns a 422 Unprocessable Entity response if the shopping cart could not be created due db errors
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToShoppingCartAsync(CreateShoppingCartItemDto dto)
    {
        var result = await shoppingCartService.AddItemToShoppingCartAsync(HttpContext.GetUserId(), dto);

        return result.Match(
            value => CreatedAtRoute(
                linkGenerator.GetUriByName(HttpContext, nameof(GetShoppingCartAsync)),
                value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Updates an item in shopping cart based on the provided details
    /// </summary>
    /// <param name="gameId"></param>
    /// <param name="dto"></param>
    /// <returns>
    /// Returns a 200 Ok response with the updated shopping cart if the request is successful.
    /// Returns a 404 Not Found response if the shopping cart does not exist.
    /// Returns a 422 Unprocessable Entity response if the shopping cart could not be created due db errors
    /// </returns>
    [HttpPut("{gameId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ShoppingCartDto>> UpdateItemInShoppingCartAsync(
        [FromRoute] Guid gameId, UpdateShoppingCartItemDto dto)
    {
        var result = await shoppingCartService.UpdateQuantityAsync(HttpContext.GetUserId(), gameId, dto);

        return result.Match(
            value => Ok(value),
            ApiResults.Problem
        );
    }

    /// <summary>
    /// Deletes shopping cart
    /// </summary>
    /// <returns>
    /// Returns a 200 Ok response with the updated shopping cart if the request is successful.
    /// Returns a 422 Unprocessable Entity response if the shopping cart could not be created due db errors
    /// </returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> DeleteShoppingCartAsync()
    {
        var result = await shoppingCartService.DeleteShoppingCartAsync(HttpContext.GetUserId());

        return result.Match(
            NoContent,
            ApiResults.Problem
        );
    }
    
    /// <summary>
    /// Seed shopping cart for testing
    /// </summary>
    /// <param name="userCount"></param>
    /// <returns>
    /// Returns a 200 Ok response with the user count if the request is successful.
    /// </returns>
#if DEBUG
    [HttpPost("/seed/{userCount:int}")]
#endif
    public async Task<ActionResult> SeedShoppingCarts(int userCount)
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

        return Ok($"Seeded {userCount} users");
    } 
}