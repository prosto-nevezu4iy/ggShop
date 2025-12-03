using CatalogService.Abstractions;
using CatalogService.DTOs;
using CatalogService.Entities;
using CatalogService.Errors;
using CatalogService.Extensions;
using CatalogService.Infrastructure;
using Common.Domain;
using Common.Presentation.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services;

public class UserRatingService : IUserRatingService
{
    private readonly CatalogContext _dbContext;
    private readonly IValidator<CreateUserRatingDto> _createUserRatingValidator;
    private readonly IValidator<UpdateUserRatingDto> _updateUserRatingValidator;
    private readonly IGameService _gameService;

    public UserRatingService(
        CatalogContext dbContext,
        IValidator<CreateUserRatingDto> createUserRatingValidator,
        IValidator<UpdateUserRatingDto> updateUserRatingValidator,
        IGameService gameService)
    {
        _dbContext = dbContext;
        _createUserRatingValidator = createUserRatingValidator;
        _updateUserRatingValidator = updateUserRatingValidator;
        _gameService = gameService;
    }


    public async Task<Result<UserRatingDto>> AddUserRatingAsync(Guid id, Guid userId, CreateUserRatingDto createUserRatingDto)
    {
        var validationResult = await _createUserRatingValidator.ValidateAsync(createUserRatingDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var game = await _gameService.GetGameEntityByIdAsync(id);

        if (game is null)
        {
            return GameErrors.NotFound(id);
        }

        var newRating = new UserRating
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            UserId = userId,
            Rating = (byte)createUserRatingDto.Rating
        };

        _dbContext.UserRatings.Add(newRating);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GameErrors.UserRatingNotCreated;
        }

        return newRating.ToDto();
    }

    public async Task<Result> UpdateUserRatingAsync(Guid id, Guid userId, UpdateUserRatingDto updateUserRatingDto)
    {
        var validationResult = await _updateUserRatingValidator.ValidateAsync(updateUserRatingDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var game = await _gameService.GetGameEntityByIdAsync(id);

        if (game is null)
        {
            return GameErrors.NotFound(id);
        }

        var existingRating = await _dbContext.UserRatings
            .FirstOrDefaultAsync(r => r.GameId == id && r.UserId == userId);

        if (existingRating is not null)
        {
            existingRating.Rating = (byte)updateUserRatingDto.Rating;
        }

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return GameErrors.UserRatingNotUpdated;
        }

        return Result.Success();
    }

    public async Task<Result> DeleteUserRatingAsync(Guid id, Guid userId)
    {
        var rating = await _dbContext.UserRatings
            .FirstOrDefaultAsync(r => r.GameId == id && r.UserId == userId);

        if (rating is null)
        {
            return GameErrors.UserRatingNotFound;
        }

        _dbContext.UserRatings.Remove(rating);
        var result = await _dbContext.SaveChangesAsync() > 0;

        return result ? Result.Success() : GameErrors.UserRatingNotDeleted;
    }
}