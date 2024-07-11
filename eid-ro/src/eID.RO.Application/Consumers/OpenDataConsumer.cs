using eID.RO.Contracts.Commands;
using eID.RO.Service;
using MassTransit;

namespace eID.RO.Application.Consumers;

public class OpenDataConsumer : BaseConsumer,
    IConsumer<GetActivatedEmpowermentsByYear>
{
    private readonly OpenDataService _openDataService;

    public OpenDataConsumer(
        ILogger<EmpowermentConsumer> logger,
        OpenDataService openDataService) : base(logger)
    {
        _openDataService = openDataService ?? throw new ArgumentNullException(nameof(openDataService));
    }

    public async Task Consume(ConsumeContext<GetActivatedEmpowermentsByYear> context)
    {
        await ExecuteMethodAsync(context, () => _openDataService.GetActivatedEmpowermentsByYearAsync(context.Message));
    }
}
