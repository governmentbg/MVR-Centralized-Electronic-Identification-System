using eID.POD.Service.Database;
using eID.POD.Service.Jobs;
using eID.POD.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace eID.POD.Service;

public class JobScheduler
{
    private readonly ILogger<JobScheduler> _logger;
    private readonly ApplicationDbContext _context;
    private readonly OpenDataSettings _openDataSettings;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly DatasetsService _datasetsService;

    public JobScheduler(
        ILogger<JobScheduler> logger,
        ApplicationDbContext context,
        IOptions<OpenDataSettings> openDataSettings,
        ISchedulerFactory schedulerFactory,
        DatasetsService datasetsService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _openDataSettings = (openDataSettings ?? throw new ArgumentNullException(nameof(openDataSettings))).Value;
        _openDataSettings.Validate();
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
        _datasetsService = datasetsService ?? throw new ArgumentNullException(nameof(datasetsService));
    }


    /// <summary>
    /// Gets all active datasets and schedules a job for each one.
    /// </summary>
    public async Task ScheduleJobsAsync()
    {
        if (!_openDataSettings.AutomaticStart)
        {
            _logger.LogInformation("Automatic start for scheduled jobs is turned off. There will be no automatic upload in OpenData.");
            return;
        }

        _logger.LogInformation($"Starting {nameof(ScheduleJobsAsync)}");

        var serviceRequest = await _datasetsService.GetActiveDatasetsAsync();
        if (serviceRequest.StatusCode != System.Net.HttpStatusCode.OK)
        {
            _logger.LogWarning("Failed getting active datasets.");
            return;
        }
        var activeDatasets = serviceRequest.Result;
        if (activeDatasets is null)
        {
            _logger.LogWarning("Active datasets result was null.");
            return;
        }

        if (!activeDatasets.Any())
        {
            _logger.LogInformation("No active datasets found.");
            return;
        }

        _logger.LogInformation("Found {ActiveDatasetsCount} OpenData datasets for upload.", activeDatasets.Count());

        IScheduler scheduler = await _schedulerFactory.GetScheduler();
        foreach (var dataset in activeDatasets)
        {
            try
            {
                // Create job details
                IJobDetail job = JobBuilder.Create<UploadDataJob>()
                    .WithIdentity(dataset.Id.ToString())
                    .Build();

                // Trigger the job to run now
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(dataset.Id.ToString())
                    .WithCronSchedule(dataset.CronPeriod)
                    .Build();

                // Tell Quartz to schedule the job using our trigger
                _logger.LogInformation("Scheduling job for {DatasetId}", dataset.Id);
                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem scheduling job {DatasetId}", dataset.Id);
            }
        }
    }
}
