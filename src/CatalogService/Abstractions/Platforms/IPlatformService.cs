using CatalogService.DTOs.Platforms;
using CatalogService.RequestHelpers;
using Common.Application.Pagination;
using Common.Domain;

namespace CatalogService.Abstractions.Platforms;

public interface IPlatformService
{
    Task<Result<PaginatedItems<PlatformDto>>> GetPlatformsAsync(PlatformPagedFilterRequest request);
    Task<Result<PlatformDto>> GetPlatformByIdAsync(Guid id);
    Task<Result<PlatformDto>> CreatePlatformAsync(CreatePlatformDto createPlatformDto);
    Task<Result> UpdatePlatformAsync(Guid id, UpdatePlatformDto updatePlatformDto);
    Task<Result> DeletePlatformAsync(Guid id);
}