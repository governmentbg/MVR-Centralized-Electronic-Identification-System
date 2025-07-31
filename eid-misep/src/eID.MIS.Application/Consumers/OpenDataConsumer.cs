using eID.MIS.Contracts.SEV.Commands;
using eID.MIS.Service.SEV;
using MassTransit;

namespace eID.MIS.Application.Consumers;

public class OpenDataConsumer : BaseConsumer,
    IConsumer<GetDeliveredMessagesByYear>
{
    private readonly OpenDataService _openDataService;

    public OpenDataConsumer(
        ILogger<OpenDataConsumer> logger,
        OpenDataService openDataService) : base(logger)
    {
        _openDataService = openDataService ?? throw new ArgumentNullException(nameof(openDataService));
    }

    public async Task Consume(ConsumeContext<GetDeliveredMessagesByYear> context)
    {
        await ExecuteMethodAsync(context, () => _openDataService.GetDeliveredMessagesByYearAsync(context.Message));
    }
}
