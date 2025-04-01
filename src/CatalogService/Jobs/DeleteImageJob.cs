using CatalogService.Services;
using Quartz;

namespace CatalogService.Jobs;

public class DeleteImageJob : IJob
{
    private readonly IImageService _imageService;
    private readonly ILogger<DeleteImageJob> _logger;

    public DeleteImageJob(IImageService imageService, ILogger<DeleteImageJob> logger)
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
            await _imageService.DeleteImages(dataMap.GetString("imageUrl"));
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
