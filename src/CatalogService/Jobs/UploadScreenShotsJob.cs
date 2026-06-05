using CatalogService.Abstractions;
using CatalogService.Abstractions.Games;
using Quartz;

namespace CatalogService.Jobs;

public class UploadScreenShotsJob(
    IImageService imageService,
    IGameService gameService,
    ILogger<UploadScreenShotsJob> logger)
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

            var transformedScreenShotUrls = await imageService.UploadScreenShots((IEnumerable<string>)dataMap["screenShotUrls"]);

            await gameService.UpdateScreenShotUrlsAsync(game, transformedScreenShotUrls);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
