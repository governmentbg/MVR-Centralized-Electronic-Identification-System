using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using MassTransit;

namespace eID.PAN.Application.Consumers;

public class SmtpConfigurationsConsumer : BaseConsumer,
    IConsumer<GetSmtpConfigurationsByFilter>,
    IConsumer<GetSmtpConfigurationById>,
    IConsumer<UpdateSmtpConfiguration>,
    IConsumer<CreateSmtpConfiguration>,
    IConsumer<DeleteSmtpConfiguration>
{
    private readonly SmtpConfigurationsService _smtpConfigurationsService;

    public SmtpConfigurationsConsumer(
        ILogger<SmtpConfigurationsConsumer> logger,
        SmtpConfigurationsService smtpConfigurationsService) : base(logger)
    {
        _smtpConfigurationsService = smtpConfigurationsService ?? throw new ArgumentNullException(nameof(smtpConfigurationsService));
    }

    public async Task Consume(ConsumeContext<CreateSmtpConfiguration> context)
    {
        await ExecuteMethodAsync(context, () => _smtpConfigurationsService.CreateAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetSmtpConfigurationsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _smtpConfigurationsService.GetByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetSmtpConfigurationById> context)
    {
        await ExecuteMethodAsync(context, () => _smtpConfigurationsService.GetByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<UpdateSmtpConfiguration> context)
    {
        await ExecuteMethodAsync(context, () => _smtpConfigurationsService.UpdateAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<DeleteSmtpConfiguration> context)
    {
        await ExecuteMethodAsync(context, () => _smtpConfigurationsService.DeleteAsync(context.Message));
    }
}


