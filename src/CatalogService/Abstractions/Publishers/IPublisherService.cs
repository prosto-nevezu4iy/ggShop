using CatalogService.DTOs.Publishers;
using CatalogService.RequestHelpers;
using Common.Application.Pagination;
using Common.Domain;

namespace CatalogService.Abstractions.Publishers;

public interface IPublisherService
{
    Task<Result<PaginatedItems<PublisherDto>>> GetPublishersAsync(PublisherPagedFilterRequest request);
    Task<Result<PublisherDto>> GetPublisherByIdAsync(Guid id);
    Task<Result<PublisherDto>> CreatePublisherAsync(CreatePublisherDto createPublisherDto);
    Task<Result> UpdatePublisherAsync(Guid id, UpdatePublisherDto updatePublisherDto);
    Task<Result> DeletePublisherAsync(Guid id);
}