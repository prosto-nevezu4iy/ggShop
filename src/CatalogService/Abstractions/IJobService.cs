namespace CatalogService.Abstractions;

public interface IJobService
{
    Task UploadImageJob(Guid gameId, string imageUrl);
    Task UploadScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls);
    Task DeleteImageJob(Guid gameId, string imageUrl);
    Task DeleteScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls);
}