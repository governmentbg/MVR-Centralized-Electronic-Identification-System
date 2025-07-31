using eID.PJS.Application.Options;
using Microsoft.Extensions.Options;
using Quartz;

namespace eID.PJS.Application.Jobs;

[DisallowConcurrentExecution]
public class ClearLogFilesJob : IJob
{
    private readonly ILogger<ClearLogFilesJob> _logger;
    private readonly ClearLogOptions _clearLogOptions;
    private readonly StorageOptions _storageOptions;

    public ClearLogFilesJob(
        ILogger<ClearLogFilesJob> logger,
        IOptions<ClearLogOptions> clearLogOptions,
        IOptions<StorageOptions> storageOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clearLogOptions = (clearLogOptions ?? throw new ArgumentNullException(nameof(clearLogOptions))).Value;
        _clearLogOptions.IsValid();
        _storageOptions = (storageOptions ?? throw new ArgumentNullException(nameof(storageOptions))).Value;
        _storageOptions.Validate();
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // File date considers as older
            var data = DateTime.UtcNow.Date.AddDays(-(_clearLogOptions.RemoveOlderFilesInDays));

            var path = Path.GetFullPath(_storageOptions.ExportAuditLogsCsvFilesLocation);

            var info = new DirectoryInfo(path);
            var files = info.GetFiles()
                .Where(f => f.CreationTimeUtc <= data)
                .ToArray();

            foreach (var file in files)
            {
                file.Delete();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when executing {nameof(ClearLogFilesJob)}");
        }
    }
}
