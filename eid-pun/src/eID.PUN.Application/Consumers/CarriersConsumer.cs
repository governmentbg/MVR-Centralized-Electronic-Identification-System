using eID.PUN.Contracts.Commands;
using eID.PUN.Service;
using MassTransit;

namespace eID.PUN.Application.Consumers;

public class CarriersConsumer : BaseConsumer,
    IConsumer<RegisterCarrier>,
    IConsumer<GetCarriersByFilter>
{
    private readonly CarriersService _carriersService;

    public CarriersConsumer(ILogger<CarriersConsumer> logger, CarriersService carriersService) 
        : base(logger)
    {
        _carriersService = carriersService ?? throw new ArgumentNullException(nameof(carriersService));
    }

    public async Task Consume(ConsumeContext<RegisterCarrier> context)
    {
        await ExecuteMethodAsync(context, () => _carriersService.RegisterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetCarriersByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _carriersService.GetByFilterAsync(context.Message));
    }
}
