using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using MassTransit;

namespace eID.PIVR.Application.Consumers
{
    public class RegiXConsumer : BaseConsumer,
        IConsumer<RegiXSearchCommand>
    {
        private readonly IRegiXService _regixService;

        public RegiXConsumer(ILogger<RegiXConsumer> logger, IRegiXService regixService) : base(logger)
        {
            _regixService = regixService ?? throw new ArgumentNullException(nameof(regixService));
        }

        public async Task Consume(ConsumeContext<RegiXSearchCommand> context)
        {
            await ExecuteMethodAsync(context, () => _regixService.SearchAsync(context.Message));
        }
    }
}
