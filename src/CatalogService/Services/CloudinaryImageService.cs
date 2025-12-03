using CatalogService.Abstractions;
using CatalogService.Configurations;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using static CatalogService.Constants.GameConstants;

namespace CatalogService.Services;

public class CloudinaryImageService : IImageService
{
    private readonly CloudinarySettings _cloudinarySettings;
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageService(IOptions<CloudinarySettings> cloudinarySettings)
    {
        _cloudinarySettings = cloudinarySettings.Value;

        _cloudinary = new Cloudinary(new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret));
    }

    public async Task<string> UploadImage(string filePath)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filePath),
            Folder = FolderName,
            Transformation = new Transformation()
                .Width(300)
                .Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.PublicId;
    }

    public async Task<string> UploadScreenShot(string filePath)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filePath),
            Folder = FolderName,
            Transformation = new Transformation()
                .Width(1048)
                .Height(590)
                .Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.PublicId;
    }

    public async Task<string[]> UploadScreenShots(IEnumerable<string> screenShots)
    {
        var uploadTasks = screenShots.Select(UploadScreenShot).ToList();
        return await Task.WhenAll(uploadTasks);
    }

    public async Task DeleteImages(
        params string[] publicIds) =>
        await _cloudinary.DeleteResourcesAsync(ResourceType.Image, publicIds);
}