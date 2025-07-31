using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Service;
using MassTransit;

namespace eID.PDEAU.Application.Consumers;

public class OpenDataConsumer : BaseConsumer,
    IConsumer<GetActiveProviders>,
    IConsumer<GetDoneServicesByYear>
{
    private readonly OpenDataService _service;

    public OpenDataConsumer(ILogger<OpenDataConsumer> logger, OpenDataService service) : base(logger)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<GetActiveProviders> context)
    {
        await ExecuteMethodAsync(context, () => _service.GetActiveProvidersAsync());
    }

    public async Task Consume(ConsumeContext<GetDoneServicesByYear> context)
    {
        await ExecuteMethodAsync(context, () => _service.GetDoneServicesByYearAsync(context.Message));
    }
}
