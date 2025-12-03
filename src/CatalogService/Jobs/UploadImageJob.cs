using CatalogService.Abstractions;
using Quartz;

namespace CatalogService.Jobs;

public class UploadImageJob : IJob
{
    private readonly IGameService _gameService;
    private readonly IImageService _imageService;
    private readonly ILogger<UploadImageJob> _logger;

    public UploadImageJob(IImageService imageService, IGameService gameService, ILogger<UploadImageJob> logger)
    {
        _imageService = imageService;
        _gameService = gameService;
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
            var game = await _gameService.GetGameEntityByIdAsync(gameId);

            if (game is null)
            {
                return;
            }

            var uploadedUrl = await _imageService.UploadImage(dataMap.GetString("imageUrl"));

            await _gameService.UpdateImageUrlAsync(game, uploadedUrl);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}
