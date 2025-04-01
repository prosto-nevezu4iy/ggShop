namespace CatalogService.Services;

public interface IImageService
{
    Task<string> UploadImage(string filePath);
    Task<string> UploadScreenShot(string filePath);
    Task<string[]> UploadScreenShots(IEnumerable<string> screenShots);
    Task UploadImageJob(Guid createdGameId, string imageUrl);
    Task UploadScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls);
    Task DeleteScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls);
    Task DeleteImages(params string[] publicIds);
    Task DeleteImageJob(Guid gameId, string imageUrl);
}