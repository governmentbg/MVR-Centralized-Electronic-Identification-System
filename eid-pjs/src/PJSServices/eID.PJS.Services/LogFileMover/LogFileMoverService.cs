using System.Configuration;
using eID.PJS.Services.Signing;
using eID.PJS.Services.Verification;
using Microsoft.Extensions.Options;

namespace eID.PJS.Services.LogFileMover
{
    public class LogFileMoverService : BackgroundService
    {
        private readonly LogFileMoverSettings _settings;
        private readonly SigningServiceSettings _signingServiceSettings;
        private readonly VerificationServiceSettings _verificationServiceSettings;
        private readonly ILogger<LogFileMoverService> _logger;
        private readonly TimeSpan _interval;

        public LogFileMoverService(IOptions<LogFileMoverSettings> settings,
                                   SigningServiceSettings signingServiceSettings,
                                   VerificationServiceSettings verificationServiceSettings,
                                   ILogger<LogFileMoverService> logger)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (signingServiceSettings is null)
            {
                throw new ArgumentNullException(nameof(signingServiceSettings));
            }

            if (verificationServiceSettings is null)
            {
                throw new ArgumentNullException(nameof(verificationServiceSettings));
            }

            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _settings.Validate();
            _signingServiceSettings = signingServiceSettings;
            _verificationServiceSettings = verificationServiceSettings;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interval = TimeSpan.FromMinutes(Math.Max(_settings.OrphanFilesMoveIntervalInMinutes, 90)); // Minimum threshold
            _logger.LogInformation("LogFileMoverService interval set to {Interval}.", _interval);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_interval);
            do
            {
                try
                {
                    MoveOrphanLogs();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while moving log files.");
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }


        public void MoveOrphanLogs()
        {
            foreach (var monitoredFolder in _signingServiceSettings.MonitoredFolders)
            {
                try
                {
                    var logExt = _verificationServiceSettings.AuditLogFileExtension;
                    var destDir = Path.GetFullPath(monitoredFolder.MonitorFolder);
                    if (string.IsNullOrEmpty(destDir))
                    {
                        throw new ConfigurationErrorsException($"Could not find the destPath in the '{nameof(monitoredFolder.TargetFolderLogs)}' section of the configuration.");
                    }

                    string sourceDir = Path.GetFullPath(Path.GetDirectoryName(destDir) ?? string.Empty);
                    if (string.IsNullOrEmpty(sourceDir))
                    {
                        throw new ConfigurationErrorsException($"Could not find sourceDir in '{destDir}'.");
                    }

                    var orphanFiles = Directory.GetFiles(sourceDir, $"*{logExt}")
                                   .Where(f => File.GetLastWriteTime(f) < DateTime.Now.AddHours(-2))
                                   .ToArray();

                    foreach (var orphanFile in orphanFiles)
                    {
                        try
                        {
                            File.Move(orphanFile, Path.Combine(destDir, Path.GetFileName(orphanFile)));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception during move of file {OrphanFile}.", orphanFile);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during iteration of {MonitorFolder}.", monitoredFolder.MonitorFolder);
                    continue;
                }
            }
        }
    }
}
