using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using MassTransit;

namespace eID.PIVR.Application.Consumers;

public class IdentityChecksConsumer : BaseConsumer,
    IConsumer<GetIdChanges>,
    IConsumer<GetStatutChanges>
{
    private readonly IdentityChecksService _service;

    public IdentityChecksConsumer(
        ILogger<IdentityChecksConsumer> logger,
        IdentityChecksService service) : base(logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task Consume(ConsumeContext<GetIdChanges> context)
    {
        await ExecuteMethodAsync(context, async () => await _service.GetIdChangesAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetStatutChanges> context)
    {
        await ExecuteMethodAsync(context, async () => await _service.GetStatutChangesAsync(context.Message));
    }
}
