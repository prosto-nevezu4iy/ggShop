using CatalogService.Abstractions;
using Quartz;

namespace CatalogService.Jobs;

public class DeleteScreenShotsJob : IJob
{
    private readonly IImageService _imageService;
    private readonly ILogger<DeleteScreenShotsJob> _logger;

    public DeleteScreenShotsJob(IImageService imageService, ILogger<DeleteScreenShotsJob> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        var gameId = dataMap.GetGuid("gameId");

        if (context.RefireCount > 10)
        {
            _logger.LogWarning("Failed to delete images for game {GameId} after {RefireCount} attempts", gameId, context.RefireCount);
            return;
        }

        try
        {
            await _imageService.DeleteImages((string[])dataMap["screenShotUrls"]);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
