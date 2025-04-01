using CatalogService.Services;
using Quartz;

namespace CatalogService.Jobs;

public class UploadImageJob : IJob
{
    private readonly ICatalogService _catalogService;
    private readonly IImageService _imageService;
    private readonly ILogger<UploadImageJob> _logger;

    public UploadImageJob(IImageService imageService, ICatalogService catalogService, ILogger<UploadImageJob> logger)
    {
        _imageService = imageService;
        _catalogService = catalogService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        var gameId = dataMap.GetGuid("gameId");

        if (context.RefireCount > 10)
        {
            _logger.LogWarning("Failed to upload image for game {GameId} after {RefireCount} attempts", gameId, context.RefireCount);
            return;
        }

        try
        {
            var game = await _catalogService.GetGameEntityByIdAsync(gameId);

            if (game == null) return;

            var uploadedUrl = await _imageService.UploadImage(dataMap.GetString("imageUrl"));

            await _catalogService.UpdateImageUrlAsync(game, uploadedUrl);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
