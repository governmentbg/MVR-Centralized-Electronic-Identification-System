using eID.PUN.Contracts.Results;
using Microsoft.Extensions.Logging;

namespace eID.PUN.Service;

public class EventRegistrationService : BaseService
{
    private readonly ILogger<EventRegistrationService> _logger;
    private readonly IEventsRegistrator _eventsRegistrator;

    public EventRegistrationService(
        ILogger<EventRegistrationService> logger,
        IEventsRegistrator eventsRegistrator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventsRegistrator = eventsRegistrator ?? throw new ArgumentNullException(nameof(eventsRegistrator));
    }

    public async Task<ServiceResult<bool>> RegisterEventsAsync()
    {
        var isSuccessfullySend = await _eventsRegistrator.RegisterAsync();
        return Ok(isSuccessfullySend);
    }
}
