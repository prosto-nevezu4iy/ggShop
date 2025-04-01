using CatalogService.Jobs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Quartz;

namespace CatalogService.Services;

public class CloudinaryImageService : IImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ISchedulerFactory _schedulerFactory;

    public CloudinaryImageService(Cloudinary cloudinary, ISchedulerFactory schedulerFactory)
    {
        _cloudinary = cloudinary;
        _schedulerFactory = schedulerFactory;
    }
    public async Task<string> UploadImage(string filePath)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filePath),
            Folder = "games",
            Transformation = new Transformation()
                .Width(300)
                .Crop("fill")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.PublicId;
    }

    public async Task<string> UploadScreenShot(string filePath)
    {
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(filePath),
            Folder = "games",
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

    public async Task UploadImageJob(Guid createdGameId, string imageUrl)
    {
        IJobDetail job = JobBuilder.Create<UploadImageJob>()
            .WithIdentity($"imageJob_{createdGameId}")
            .UsingJobData("gameId", createdGameId)
            .UsingJobData("imageUrl", imageUrl)
        .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"imageTrigger_{createdGameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task UploadScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls)
    {
        var jobDataMap = new JobDataMap
        {
            { "screenShotUrls", screenShotUrls }
        };

        IJobDetail job = JobBuilder.Create<UploadScreenShotsJob>()
            .WithIdentity($"screenShotsJob_{gameId}")
            .UsingJobData("gameId", gameId)
            .UsingJobData(jobDataMap)
        .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"screenShotsTrigger_{gameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        if (scheduler.CheckExists(job.Key).Result) await scheduler.DeleteJob(job.Key);

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task DeleteScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls)
    {
        var jobDataMap = new JobDataMap
        {
            { "screenShotUrls", screenShotUrls.ToArray() }
        };

        IJobDetail job = JobBuilder.Create<DeleteScreenShotsJob>()
            .WithIdentity($"deleteScreenShotsJob_{gameId}")
            .UsingJobData("gameId", gameId)
            .UsingJobData(jobDataMap)
        .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"deleteScreenShotsTrigger_{gameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task DeleteImages(params string[] publicIds)
    {
        await _cloudinary.DeleteResourcesAsync(ResourceType.Image, publicIds);
    }

    public async Task DeleteImageJob(Guid gameId, string imageUrl)
    {
        IJobDetail job = JobBuilder.Create<DeleteImageJob>()
           .WithIdentity($"deleteImageJob_{gameId}")
           .UsingJobData("gameId", gameId)
           .UsingJobData("imageUrl", imageUrl)
       .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"deleteImageTrigger_{gameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }
}