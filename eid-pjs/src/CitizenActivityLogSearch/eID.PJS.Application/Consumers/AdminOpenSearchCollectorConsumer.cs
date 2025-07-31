using eID.PJS.Contracts.Commands.Admin;
using eID.PJS.Service.Admin;
using MassTransit;

namespace eID.PJS.Application.Consumers;

public class AdminOpenSearchCollectorConsumer : BaseConsumer,
    IConsumer<GetLogFromUser>,
    IConsumer<GetLogToUser>,
    IConsumer<GetLogFromUserAsFile>,
    IConsumer<GetLogToUserAsFile>
{
    private readonly AdminOpenSearchCollectorService _adminOpenSearchCollectorService;

    public AdminOpenSearchCollectorConsumer(
        ILogger<AdminOpenSearchCollectorConsumer> logger,
        AdminOpenSearchCollectorService adminOpenSearchCollectorService) : base(logger)
    {
        _adminOpenSearchCollectorService = adminOpenSearchCollectorService ?? throw new ArgumentNullException(nameof(adminOpenSearchCollectorService));
    }

    public async Task Consume(ConsumeContext<GetLogFromUser> context)
    {
        await ExecuteMethodAsync(context, () => _adminOpenSearchCollectorService.GetLogFromUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetLogToUser> context)
    {
        await ExecuteMethodAsync(context, () => _adminOpenSearchCollectorService.GetLogToUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetLogFromUserAsFile> context)
    {
        await ExecuteMethodWithoutResponseAsync(context, () => _adminOpenSearchCollectorService.GetLogFromUserAsFileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetLogToUserAsFile> context)
    {
        await ExecuteMethodWithoutResponseAsync(context, () => _adminOpenSearchCollectorService.GetLogToUserAsFileAsync(context.Message));
    }
}

public class AdminOpenSearchCollectorConsumerConsumerDefinition : ConsumerDefinition<AdminOpenSearchCollectorConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<AdminOpenSearchCollectorConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Exponential(3, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(3)));
    }
}
