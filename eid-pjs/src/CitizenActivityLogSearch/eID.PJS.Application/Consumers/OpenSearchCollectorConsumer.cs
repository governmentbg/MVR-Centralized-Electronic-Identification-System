using eID.PJS.Contracts.Commands;
using eID.PJS.Service;
using MassTransit;

namespace eID.PJS.Application.Consumers;

public class OpenSearchCollectorConsumer : BaseConsumer,
    IConsumer<GetLogUserFromMe>,
    IConsumer<GetLogUserToMe>,
    IConsumer<GetLogDeau>
{
    private readonly OpenSearchCollectorService _openSearchCollectorService;

    public OpenSearchCollectorConsumer(
        ILogger<OpenSearchCollectorConsumer> logger,
        OpenSearchCollectorService openSearchCollectorService) : base(logger)
    {
        _openSearchCollectorService = openSearchCollectorService ?? throw new ArgumentNullException(nameof(openSearchCollectorService));
    }

    public async Task Consume(ConsumeContext<GetLogUserFromMe> context)
    {
        await ExecuteMethodAsync(context, () => _openSearchCollectorService.GetLogUserFromMeAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetLogUserToMe> context)
    {
        await ExecuteMethodAsync(context, () => _openSearchCollectorService.GetLogUserToMeAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetLogDeau> context)
    {
        await ExecuteMethodAsync(context, () => _openSearchCollectorService.GetLogDeauAsync(context.Message));
    }
}
