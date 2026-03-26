using CatalogService.DTOs;
using CatalogService.DTOs.Games;
using Common.Domain;

namespace CatalogService.Abstractions.Games;

public interface IUserRatingService
{
    Task<Result<UserRatingDto>> AddUserRatingAsync(Guid id, Guid userId, CreateUserRatingDto createUserRatingDto);
    Task<Result> UpdateUserRatingAsync(Guid id, Guid userId, UpdateUserRatingDto updateUserRatingDto);
    Task<Result> DeleteUserRatingAsync(Guid id, Guid userId);
}