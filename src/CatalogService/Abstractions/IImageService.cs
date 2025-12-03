namespace CatalogService.Abstractions;

public interface IImageService
{
    Task<string> UploadImage(string filePath);
    Task<string> UploadScreenShot(string filePath);
    Task<string[]> UploadScreenShots(IEnumerable<string> screenShots);
    Task DeleteImages(params string[] publicIds);
}