using CatalogService.Abstractions;
using CatalogService.Abstractions.Platforms;
using CatalogService.DTOs.Platforms;
using CatalogService.Entities;
using CatalogService.Enums;
using CatalogService.Errors;
using CatalogService.Extensions;
using CatalogService.Infrastructure;
using CatalogService.RequestHelpers;
using Common.Application.Requests.Pagination;
using Common.Domain;
using Common.Presentation.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Platforms;

public class PlatformService : IPlatformService
{
    private readonly CatalogContext _dbContext;
    private readonly IValidator<PlatformPagedFilterRequest> _platformPagedFilterValidator;
    private readonly IValidator<CreatePlatformDto> _createValidator;
    private readonly IValidator<UpdatePlatformDto> _updateValidator;
    private readonly ISearchBuilder<Platform> _searchBuilder;
    private readonly IOrderBuilder<Platform, PlatformSortOption> _orderBuilder;

    public PlatformService(
        CatalogContext dbContext,
        IValidator<PlatformPagedFilterRequest> platformPagedFilterValidator,
        IValidator<CreatePlatformDto> createValidator,
        IValidator<UpdatePlatformDto> updateValidator,
        ISearchBuilder<Platform> searchBuilder,
        IOrderBuilder<Platform, PlatformSortOption> orderBuilder)
    {
        _dbContext = dbContext;
        _platformPagedFilterValidator = platformPagedFilterValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _searchBuilder = searchBuilder;
        _orderBuilder = orderBuilder;
    }

    public async Task<Result<PaginatedItems<PlatformDto>>> GetPlatformsAsync(PlatformPagedFilterRequest request)
    {
        var validationResult = await _platformPagedFilterValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var query = _dbContext.Platforms.AsNoTracking();

        query = _searchBuilder.Build(query, request.SearchTerm);

        query = _orderBuilder.Build(query, request.Sort);

        var totalItems = await query.CountAsync();

        var platforms = await query
            .Skip(request.GetOffset())
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToListAsync();

        return new PaginatedItems<PlatformDto>(request.PageNumber, request.PageSize, totalItems, platforms);
    }

    public async Task<Result<PlatformDto>> GetPlatformByIdAsync(Guid id)
    {
        var platform = await _dbContext.Platforms.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);

        return platform is null
            ? PlatformErrors.NotFound(id)
            : platform.ToDto();
    }

    public async Task<Result<PlatformDto>> CreatePlatformAsync(CreatePlatformDto createPlatformDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createPlatformDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var platform = createPlatformDto.ToEntity();

        await _dbContext.Platforms.AddAsync(platform);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return PlatformErrors.PlatformNotCreated;
        }

        return platform.ToDto();
    }

    public async Task<Result> UpdatePlatformAsync(Guid id, UpdatePlatformDto updatePlatformDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updatePlatformDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var platform = await _dbContext.Platforms.SingleOrDefaultAsync(g => g.Id == id);

        if (platform is null)
        {
            return PlatformErrors.NotFound(id);
        }

        _dbContext.Entry(platform).CurrentValues.SetValues(updatePlatformDto);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return PlatformErrors.PlatformNotUpdated;
        }

        return Result.Success();
    }

    public async Task<Result> DeletePlatformAsync(Guid id)
    {
        var platform = await _dbContext.Platforms.SingleOrDefaultAsync(g => g.Id == id);

        if (platform is null)
        {
            return PlatformErrors.NotFound(id);
        }

        _dbContext.Platforms.Remove(platform);

        var result = await _dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return PlatformErrors.PlatformNotDeleted;
        }

        return Result.Success();
    }
}