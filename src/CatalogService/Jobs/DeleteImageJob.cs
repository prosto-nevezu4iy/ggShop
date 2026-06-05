using CatalogService.Abstractions;
using CatalogService.Abstractions.Games;
using Quartz;

namespace CatalogService.Jobs;

public class DeleteImageJob(IImageService imageService, ILogger<DeleteImageJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        var gameId = dataMap.GetGuid("gameId");

        if (context.RefireCount > 10)
        {
            logger.LogWarning("Failed to delete images for game {GameId} after {RefireCount} attempts", gameId, context.RefireCount);
            return;
        }

        try
        {
            await imageService.DeleteImages(dataMap.GetString("imageUrl"));
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
