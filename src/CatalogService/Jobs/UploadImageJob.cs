using CatalogService.Abstractions;
using CatalogService.Abstractions.Games;
using Quartz;

namespace CatalogService.Jobs;

public class UploadImageJob(IImageService imageService, IGameService gameService, ILogger<UploadImageJob> logger)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        var gameId = dataMap.GetGuid("gameId");

        if (context.RefireCount > 10)
        {
            logger.LogWarning("Failed to upload image for game {GameId} after {RefireCount} attempts", gameId, context.RefireCount);
            return;
        }

        try
        {
            var game = await gameService.GetGameEntityByIdAsync(gameId);

            if (game is null)
            {
                return;
            }

            var uploadedUrl = await imageService.UploadImage(dataMap.GetString("imageUrl"));

            await gameService.UpdateImageUrlAsync(game, uploadedUrl);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
