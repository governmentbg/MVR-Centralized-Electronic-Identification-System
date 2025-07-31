using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Service;
using MassTransit;

namespace eID.PDEAU.Application.Consumers;

public class ProvidersDetailsConsumer : BaseConsumer,
    IConsumer<GetProviderDetailsByFilter>,
    IConsumer<GetProviderDetailsById>,
    IConsumer<GetSectionsByFilter>,
    IConsumer<GetSectionById>,
    IConsumer<GetServicesByFilter>,
    IConsumer<GetServiceById>,
    IConsumer<GetAllServiceScopes>,
    IConsumer<SetProviderDetailsStatus>,
    IConsumer<UpdateService>,
    IConsumer<GetDefaultServiceScopes>,
    IConsumer<GetAvailableScopesByProviderId>,
    IConsumer<GetAvailableProviderDetailsByFilter>,
    IConsumer<GetCurrentProviderDetails>,
    IConsumer<RegisterService>,
    IConsumer<ActivateService>,
    IConsumer<DeactivateService>,
    IConsumer<ApproveService>,
    IConsumer<DenyService>
{
    private readonly ProvidersDetailsService _providersDetailsService;

    public ProvidersDetailsConsumer(ILogger<ProvidersDetailsConsumer> logger, ProvidersDetailsService providersDetailsService) 
        : base(logger)
    {
        _providersDetailsService = providersDetailsService ?? throw new ArgumentNullException(nameof(providersDetailsService));
    }

    public async Task Consume(ConsumeContext<GetProviderDetailsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetProviderDetailsByFilterAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<GetAvailableProviderDetailsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetAvailableProviderDetailsByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderDetailsById> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetProviderDetailsByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SetProviderDetailsStatus> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.SetProviderDetailsStatusAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetSectionsByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetSectionsByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetSectionById> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetSectionByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetServicesByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetServicesByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetServiceById> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetServiceByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetAllServiceScopes> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetAllServiceScopesAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<UpdateService> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.UpdateServiceAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RegisterService> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.RegisterServiceAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetDefaultServiceScopes> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetDefaultServiceScopesAsync());
    }

    public async Task Consume(ConsumeContext<GetAvailableScopesByProviderId> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetAvailableScopesByProviderIdAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<GetCurrentProviderDetails> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.GetCurrentProviderDetailsAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<ActivateService> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.ActivateServiceAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<DeactivateService> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.DeactivateServiceAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<ApproveService> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.ApproveServiceAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<DenyService> context)
    {
        await ExecuteMethodAsync(context, () => _providersDetailsService.DenyServiceAsync(context.Message));
    }
}
