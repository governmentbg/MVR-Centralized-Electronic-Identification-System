using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

using Steeltoe.Extensions.Configuration;

namespace eID.PJS.Services
{
    public abstract class ServiceSchedulerBase<TScheduler, TSettings, TResult> : BackgroundService where TSettings : SchedulerSettings
    {
        protected readonly ILogger<TScheduler> _logger;
        protected readonly IServiceProvider _services;
        protected readonly TSettings _settings;
        protected readonly GlobalStatus _status;
        protected static object _lock = new object();

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        public ServiceSchedulerBase(IServiceProvider services, ILogger<TScheduler> logger, TSettings settings, GlobalStatus status)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _status = status ?? throw new ArgumentNullException(nameof(status));
        }

        protected abstract void UpdateWorkingStatus(WorkingStatus status);
        protected abstract void UpdateState(TResult? state);
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{name} Started", typeof(TScheduler).Name);

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{name} Stopped", typeof(TScheduler).Name);

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // Don't start the service if scheduled period is zero
            if (_settings.SchedulePeriod == 0)
            {
                UpdateWorkingStatus(WorkingStatus.NotRunning);
                _logger.LogWarning("{name} service is not scheduled to run.", typeof(TScheduler).Name);
                return;
            }

            UpdateWorkingStatus(WorkingStatus.Ready);

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(_settings.SchedulePeriod));

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                UpdateWorkingStatus(WorkingStatus.Ready);

                // For multithreading scenarios.
                if (await Semaphore.WaitAsync(TimeSpan.Zero))
                {
                    try
                    {
                        UpdateWorkingStatus(WorkingStatus.Processing);
                        _logger.LogInformation("Executing {name} {date}...", typeof(TScheduler).Name, DateTime.Now);

                        using (var scope = _services.CreateScope())
                        {
                            var svc = scope.ServiceProvider.GetRequiredService<IProcessingService<TResult>>();
                            var result = svc.Process();

                            UpdateState(result);
                        }

                        UpdateWorkingStatus(WorkingStatus.Ready);
                        _logger.LogInformation("Iteration of {name} completed.", typeof(TScheduler).Name);
                    }
                    catch (Exception ex)
                    {
                        UpdateWorkingStatus(WorkingStatus.Error);
                        _logger.LogError("Failed to execute {name} with exception message: {msg}.", typeof(TScheduler).Name, ex.Message);
                    }
                    finally
                    {
                        Semaphore.Release();
                    }
                }
                else
                {
                    _logger.LogWarning("Another thread is already running the task on {name}. Skipping this run.", typeof(TScheduler).Name);
                }
            }

        }
    }
}
