using eID.PIVR.Service;
using Quartz;

namespace eID.PIVR.Application.Jobs;

[DisallowConcurrentExecution]
public class PersistDailyApiUsageStatisticsJob : IJob
{
    private readonly ILogger<PersistDailyApiUsageStatisticsJob> _logger;
    private readonly IApiUsagePersistenceService _apiUsagePersistenceService;

    public PersistDailyApiUsageStatisticsJob(
        ILogger<PersistDailyApiUsageStatisticsJob> logger,
        IApiUsagePersistenceService apiUsagePersistenceService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiUsagePersistenceService = apiUsagePersistenceService ?? throw new ArgumentNullException(nameof(apiUsagePersistenceService));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await _apiUsagePersistenceService.FlushToDatabaseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when executing {nameof(PersistDailyApiUsageStatisticsJob)}");
        }
    }
}
