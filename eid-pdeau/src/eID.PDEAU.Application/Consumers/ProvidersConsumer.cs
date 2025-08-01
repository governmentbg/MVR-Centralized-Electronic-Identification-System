using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Service;
using MassTransit;

namespace eID.PDEAU.Application.Consumers;

public class ProvidersConsumer : BaseConsumer,
    IConsumer<RegisterProvider>,
    IConsumer<GetAllProviders>,
    IConsumer<InitiateAdminPromotion>,
    IConsumer<ConfirmAdminPromotion>,
    IConsumer<GetUserDetails>,
    IConsumer<RegisterUser>,
    IConsumer<GetUsersByFilter>,
    IConsumer<UpdateUser>,
    IConsumer<GetProvidersByFilter>,
    IConsumer<GetProvidersListByFilter>,
    IConsumer<GetProviderFile>,
    IConsumer<ApproveProvider>,
    IConsumer<GetProviderById>,
    IConsumer<ReturnProviderForCorrection>,
    IConsumer<UpdateProvider>,
    IConsumer<GetProviderStatusHistory>,
    IConsumer<DenyProvider>,
    IConsumer<GetProviderGeneralInformationAndOfficesById>,
    IConsumer<UpdateProviderGeneralInformationAndOffices>,
    IConsumer<GetProvidersInfo>,
    IConsumer<GetProviderOffices>,
    IConsumer<GetProviderServices>,
    IConsumer<GetUserByUid>,
    IConsumer<AdministratorRegisterUser>,
    IConsumer<AdministratorUpdateUser>,
    IConsumer<GetUserAdministratorActions>,
    IConsumer<DeleteUser>,
    IConsumer<RegisterDoneService>
{
    private readonly IProvidersService _providersService;

    public ProvidersConsumer(
        ILogger<ProvidersConsumer> logger,
        IProvidersService providersService) : base(logger)
    {
        _providersService = providersService ?? throw new ArgumentNullException(nameof(providersService));
    }

    public async Task Consume(ConsumeContext<RegisterProvider> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.RegisterProviderAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetAllProviders> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetAllProvidersAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<InitiateAdminPromotion> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.InitiateAdminPromotionAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ConfirmAdminPromotion> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.ConfirmAdminPromotionAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetUserDetails> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetUserDetailsAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RegisterUser> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.RegisterUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetUsersByFilter> context)
    {
       await ExecuteMethodAsync(context, async () => await _providersService.GetUsersByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<UpdateUser> context)
    {
       await ExecuteMethodAsync(context, async () => await _providersService.UpdateUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProvidersByFilter> context)
    {
       await ExecuteMethodAsync(context, async () => await _providersService.GetProvidersByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProvidersListByFilter> context)
    {
       await ExecuteMethodAsync(context, async () => await _providersService.GetProvidersListByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderFile> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProviderFileAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ApproveProvider> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.ApproveProviderAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderById> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProviderByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ReturnProviderForCorrection> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.ReturnProviderForCorrectionAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<UpdateProvider> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.UpdateProviderAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderStatusHistory> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProviderStatusHistoryAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<DenyProvider> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.DenyProviderAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderGeneralInformationAndOfficesById> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProviderGeneralInformationAndOfficesByIdAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<UpdateProviderGeneralInformationAndOffices> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.UpdateProviderGeneralInformationAndOfficesAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProvidersInfo> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProvidersInfoAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderOffices> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProviderOfficesAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetProviderServices> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetProviderServicesAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetUserByUid> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetUserByUidAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<AdministratorRegisterUser> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.AdministratorRegisterUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<AdministratorUpdateUser> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.AdministratorUpdateUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetUserAdministratorActions> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.GetUserAdministratorActionsAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<DeleteUser> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.DeleteUserAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<RegisterDoneService> context)
    {
        await ExecuteMethodAsync(context, async () => await _providersService.RegisterDoneServiceAsync(context.Message));
    }
}
