using Microsoft.Extensions.Logging;
using Quartz;

namespace eID.POD.Service.Jobs;

[DisallowConcurrentExecution]
public class UploadDataJob : IJob
{
    public const string ManualExecutionJobDataKey = "IsManualExecution";

    private readonly ILogger<UploadDataJob> _logger;
    private readonly DatasetsService _datasetsService;

    public UploadDataJob(
        ILogger<UploadDataJob> logger,
        DatasetsService datasetsService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _datasetsService = datasetsService ?? throw new ArgumentNullException(nameof(datasetsService));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("{JobName} started. Dataset id: {DatasetId}", nameof(UploadDataJob), context.JobDetail.Key.Name);

            var serviceRequest = await _datasetsService.GetDatasetByIdAsync(context.JobDetail.Key.Name);
            if (serviceRequest is null)
            {
                _logger.LogWarning("Failed to fetch dataset {DatasetId}.", context.JobDetail.Key.Name);
                return;
            }
            if (serviceRequest.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogWarning("Unsuccessful get dataset {DatasetId} request {GetDatasetStatusCode}.", context.JobDetail.Key.Name, serviceRequest.StatusCode);
                return;
            }
            var dataset = serviceRequest.Result;
            if (dataset is null)
            {
                _logger.LogWarning("Cannot find dataset {DatasetId}.", context.JobDetail.Key.Name);
                return;
            }

            var isManualTriggered = context.MergedJobDataMap.GetBoolean(ManualExecutionJobDataKey);
            if (!dataset.IsActive && !isManualTriggered)
            {
                _logger.LogInformation("Dataset {DatasetId} is currently disabled. It has to be activated to execute it.", context.JobDetail.Key.Name);
                return;
            }

            await _datasetsService.ExecuteUploadDatasetAsync(dataset);
            _logger.LogInformation("{JobName} completed. Dataset id: {DatasetId}", nameof(UploadDataJob), context.JobDetail.Key.Name);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Upload data job failed.");
            return;
        }
    }
}
