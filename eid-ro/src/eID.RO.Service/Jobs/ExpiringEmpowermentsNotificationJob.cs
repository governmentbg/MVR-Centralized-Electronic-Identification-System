using System.Net;
using eID.RO.Contracts.Commands;
using eID.RO.Service.Database;
using eID.RO.Service.EventsRegistration;
using eID.RO.Service.Options;
using eID.RO.Service.Requests;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;

namespace eID.RO.Service.Jobs;

[DisallowConcurrentExecution]
public class ExpiringEmpowermentsNotificationJob : IJob
{
    private readonly ILogger<ExpiringEmpowermentsNotificationJob> _logger;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly EmpowermentsService _empowermentsService;
    private readonly ApplicationDbContext _context;

    public ExpiringEmpowermentsNotificationJob(
        ILogger<ExpiringEmpowermentsNotificationJob> logger,
        IPublishEndpoint publishEndpoint,
        EmpowermentsService empowermentsService,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _empowermentsService = empowermentsService ?? throw new ArgumentNullException(nameof(empowermentsService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{JobName} started. Id: {JobKey}", nameof(ExpiringEmpowermentsNotificationJob), context.JobDetail.Key);

        var jobSettings = await _context.ScheduledJobSettings
            .FirstOrDefaultAsync(x => x.JobName == nameof(ExpiringEmpowermentsNotificationJob));

        if (jobSettings is null)
        {
            _logger.LogWarning("Cannot find ScheduledJobSettings for {JobName}", nameof(ExpiringEmpowermentsNotificationJob));
            return;
        }

        if (string.IsNullOrWhiteSpace(jobSettings.JobSettings))
        {
            _logger.LogWarning("Missing JobSettings. {JobName} {JobSettingsId}", jobSettings.JobName, jobSettings.Id);
            return;
        }

        var configuration = JsonConvert.DeserializeObject<ExpiringEmpowermentsNotificationJobSettings>(jobSettings?.JobSettings);
        if (configuration is null)
        {
            _logger.LogWarning("Failed deserializing JobSettings for {JobName} {JobSettingsId}", jobSettings?.JobName, jobSettings?.Id);
            return;
        }
        if (configuration.DaysUntilExpiration <= 0)
        {
            _logger.LogWarning("Days until expiration must be a positive number greater than 0: {DaysUntilExpiration}. JobSettingsId: {JobSettingsId}", configuration.DaysUntilExpiration, jobSettings?.Id);
            return;
        }
        //Get empowerments close to expiry and notify users
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var serviceResult = await _empowermentsService.GetExpiringEmpowermentsAsync(new GetExpiringEmpowermentsRequest
        {
            CorrelationId = InVar.CorrelationId,
            DaysUntilExpiration = configuration.DaysUntilExpiration
        });
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (serviceResult.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogWarning("Unable to obtain Expiring Empowerments. StatusCode: {StatusCode}; Error: {Errors}", serviceResult.StatusCode, serviceResult.Errors);
            return;
        }

        var expiringEmpowerments = serviceResult.Result;

        if (expiringEmpowerments is null || !expiringEmpowerments.Any())
        {
            _logger.LogWarning("No expiring empowerments found.");
            return;
        }

        _logger.LogInformation("Found {ExpiringEmpowermentsCount} expiring empowerments.", expiringEmpowerments.Count());

        var tasks = new List<Task>();
        foreach (var empowerment in expiringEmpowerments)
        {
            // Gets all unique Uids to sent email to
            var uniqueUids = empowerment.EmpoweredUids
                .Concat(empowerment.AuthorizerUids)
                .DistinctBy(x => x.Uid); // If by any chance there is repeatable Uids, we do not send several notifications for single Uid

            var notifyTask = _publishEndpoint.Publish<NotifyUids>(new
            {
                InVar.CorrelationId,
                EmpowermentId = empowerment.Id,
                Uids = uniqueUids,
                EventCode = Events.EmpowermentExpiringSoon.Code,
                Events.EmpowermentExpiringSoon.Translations
            });

            tasks.Add(notifyTask);
        }

        await Task.WhenAll(tasks);
    }
}
