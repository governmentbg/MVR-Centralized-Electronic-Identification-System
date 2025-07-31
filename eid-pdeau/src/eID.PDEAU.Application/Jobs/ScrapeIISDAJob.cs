using eID.PDEAU.Application.Options;
using eID.PDEAU.Service;
using Microsoft.Extensions.Options;
using Quartz;

namespace eID.PDEAU.Application.Jobs;

[DisallowConcurrentExecution]
public class ScrapeIISDAJob : IJob
{
    private readonly ILogger<ScrapeIISDAJob> _logger;
    private readonly IISDAScrapeService _scrapeService;
    private readonly ScrapeIISDAOptions _options;

    private const string RetryCountKey = nameof(RetryCountKey);

    public ScrapeIISDAJob(
        ILogger<ScrapeIISDAJob> logger, 
        IOptions<ScrapeIISDAOptions> options,
        IISDAScrapeService scrapeService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        _scrapeService = scrapeService ?? throw new ArgumentNullException(nameof(scrapeService));
        _options.IsValid();
    }

    public async Task Execute(IJobExecutionContext context)
    {
        context.JobDetail.JobDataMap.TryGetInt(RetryCountKey, out var retryCount);

        try
        {
            var addAuditLogError = retryCount == _options.MaxRetriesCount;
            await _scrapeService.ScrapeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when executing {nameof(ScrapeIISDAJob)}");

            ++retryCount;
            if (retryCount > _options.MaxRetriesCount)
            {
                return;
            }

            var trigger = TriggerBuilder
                .Create()
                .StartAt(DateTime.UtcNow.AddMinutes(_options.RetryTimeoutInMinutes))
                .Build();

            var job = JobBuilder.Create<ScrapeIISDAJob>()
                .UsingJobData(RetryCountKey, retryCount)
                .Build();

            await context.Scheduler.ScheduleJob(job, trigger);
        }
    }
}
