using CatalogService.Abstractions;
using CatalogService.Abstractions.Publishers;
using CatalogService.DTOs.Platforms;
using CatalogService.DTOs.Publishers;
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

namespace CatalogService.Services.Publishers;

public class PublisherService(
    CatalogContext dbContext,
    IValidator<PublisherPagedFilterRequest> publisherPagedFilterValidator,
    IValidator<CreatePublisherDto> createValidator,
    IValidator<UpdatePublisherDto> updateValidator,
    ISearchBuilder<Publisher> searchBuilder,
    IOrderBuilder<Publisher, PublisherSortOption> orderBuilder)
    : IPublisherService
{
    public async Task<Result<PaginatedItems<PublisherDto>>> GetPublishersAsync(PublisherPagedFilterRequest request)
    {
        var validationResult = await publisherPagedFilterValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var query = dbContext.Publishers.AsNoTracking();

        query = searchBuilder.Build(query, request.SearchTerm);

        query = orderBuilder.Build(query, request.Sort);

        var totalItems = await query.CountAsync();

        var publishers = await query
            .Skip(request.GetOffset())
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToListAsync();

        return new PaginatedItems<PublisherDto>(request.PageNumber, request.PageSize, totalItems, publishers);
    }

    public async Task<Result<PublisherDto>> GetPublisherByIdAsync(Guid id)
    {
        var publisher = await dbContext.Publishers.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id);

        return publisher is null
            ? PublisherErrors.NotFound(id)
            : publisher.ToDto();
    }

    public async Task<Result<PublisherDto>> CreatePublisherAsync(CreatePublisherDto createPublisherDto)
    {
        var validationResult = await createValidator.ValidateAsync(createPublisherDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var publisher = createPublisherDto.ToEntity();

        await dbContext.Publishers.AddAsync(publisher);

        var result = await dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return PublisherErrors.PublisherNotCreated;
        }

        return publisher.ToDto();
    }

    public async Task<Result> UpdatePublisherAsync(Guid id, UpdatePublisherDto updatePublisherDto)
    {
        var validationResult = await updateValidator.ValidateAsync(updatePublisherDto);

        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.ToErrorDictionary());
        }

        var publisher = await dbContext.Publishers.SingleOrDefaultAsync(g => g.Id == id);

        if (publisher is null)
        {
            return PublisherErrors.NotFound(id);
        }

        dbContext.Entry(publisher).CurrentValues.SetValues(updatePublisherDto);

        var result = await dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return PublisherErrors.PublisherNotUpdated;
        }

        return Result.Success();
    }

    public async Task<Result> DeletePublisherAsync(Guid id)
    {
        var publisher = await dbContext.Publishers.SingleOrDefaultAsync(g => g.Id == id);

        if (publisher is null)
        {
            return PublisherErrors.NotFound(id);
        }

        dbContext.Publishers.Remove(publisher);

        var result = await dbContext.SaveChangesAsync() > 0;

        if (!result)
        {
            return PublisherErrors.PublisherNotDeleted;
        }

        return Result.Success();
    }
}