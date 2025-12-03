using CatalogService.Abstractions;
using CatalogService.Jobs;
using Quartz;
using static CatalogService.Constants.GameConstants;

namespace CatalogService.Services;

public class JobService : IJobService
{
    private readonly ISchedulerFactory _schedulerFactory;

    public JobService(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task UploadImageJob(Guid gameId, string imageUrl)
    {
        IJobDetail job = JobBuilder.Create<UploadImageJob>()
            .WithIdentity($"{ImageJobPrefix}{gameId}")
            .UsingJobData(nameof(gameId), gameId)
            .UsingJobData(nameof(imageUrl), imageUrl)
        .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"{ImageTriggerPrefix}{gameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        if (await scheduler.CheckExists(job.Key))
        {
            return;
        }

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task UploadScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls)
    {
        var jobDataMap = new JobDataMap
        {
            { nameof(screenShotUrls), screenShotUrls }
        };

        IJobDetail job = JobBuilder.Create<UploadScreenShotsJob>()
            .WithIdentity($"{ScreenShotsJobPrefix}{gameId}")
            .UsingJobData(nameof(gameId), gameId)
            .UsingJobData(jobDataMap)
        .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"{ScreenShotsTriggerPrefix}{gameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        if (await scheduler.CheckExists(job.Key))
        {
            return;
        }

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task DeleteImageJob(Guid gameId, string imageUrl)
    {
        IJobDetail job = JobBuilder.Create<DeleteImageJob>()
            .WithIdentity($"{DeleteImageJobPrefix}{gameId}")
            .UsingJobData(nameof(gameId), gameId)
            .UsingJobData(nameof(imageUrl), imageUrl)
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"{DeleteImageTriggerPrefix}{gameId}")
            .StartNow()
            .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        if (await scheduler.CheckExists(job.Key))
        {
            return;
        }

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task DeleteScreenShotsJob(Guid gameId, IEnumerable<string> screenShotUrls)
    {
        var jobDataMap = new JobDataMap
        {
            { nameof(screenShotUrls), screenShotUrls.ToArray() }
        };

        IJobDetail job = JobBuilder.Create<DeleteScreenShotsJob>()
            .WithIdentity($"{DeleteScreenShotsJobPrefix}{gameId}")
            .UsingJobData(nameof(gameId), gameId)
            .UsingJobData(jobDataMap)
        .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity($"{DeleteScreenShotsTriggerPrefix}{gameId}")
            .StartNow()
        .Build();

        var scheduler = await _schedulerFactory.GetScheduler();

        if (await scheduler.CheckExists(job.Key))
        {
            return;
        }

        await scheduler.ScheduleJob(job, trigger);
    }
}