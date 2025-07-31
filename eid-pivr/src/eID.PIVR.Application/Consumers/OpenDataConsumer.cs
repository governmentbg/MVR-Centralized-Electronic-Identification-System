using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using MassTransit;

namespace eID.PIVR.Application.Consumers;

public class OpenDataConsumer : BaseConsumer,
    IConsumer<GetApiUsageByYear>
{
    private readonly OpenDataService _openDataService;

    public OpenDataConsumer(
        ILogger<OpenDataConsumer> logger,
        OpenDataService openDataService) : base(logger)
    {
        _openDataService = openDataService ?? throw new ArgumentNullException(nameof(openDataService));
    }

    public async Task Consume(ConsumeContext<GetApiUsageByYear> context)
    {
        await ExecuteMethodAsync(context, () => _openDataService.GetApiUsageByYearAsync(context.Message));
    }
}
