using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service;

public interface IProvidersService
{
    Task<ServiceResult<Guid>> RegisterProviderAsync(RegisterProvider message);
    Task<ServiceResult<IPaginatedData<ProviderInfoResult>>> GetAllProvidersAsync(GetAllProviders message);
    Task<ServiceResult<Guid>> InitiateAdminPromotionAsync(InitiateAdminPromotion message);
    Task<ServiceResult<bool>> ConfirmAdminPromotionAsync(ConfirmAdminPromotion message);
    Task<ServiceResult<UserResult>> GetUserDetailsAsync(GetUserDetails message);
    Task<ServiceResult<Guid>> RegisterUserAsync(RegisterUser message);
    Task<ServiceResult<IPaginatedData<UserResult>>> GetUsersByFilterAsync(GetUsersByFilter message);
    Task<ServiceResult> UpdateUserAsync(UpdateUser message);
    Task<ServiceResult<IPaginatedData<ProviderResult>>> GetProvidersByFilterAsync(GetProvidersByFilter message);
    Task<ServiceResult<IPaginatedData<ProviderListResult>>> GetProvidersListByFilterAsync(GetProvidersListByFilter message);
    Task<ServiceResult<ProviderFileResult>> GetProviderFileAsync(GetProviderFile message);
    Task<ServiceResult> ApproveProviderAsync(ApproveProvider message);
    Task<ServiceResult<AdministratorRegisteredProviderResult>> GetProviderByIdAsync(GetProviderById message);
    Task<ServiceResult> ReturnProviderForCorrectionAsync(ReturnProviderForCorrection message);
    Task<ServiceResult> UpdateProviderAsync(UpdateProvider message);
    Task<ServiceResult<IEnumerable<ProviderStatusHistoryResult>>> GetProviderStatusHistoryAsync(GetProviderStatusHistory message);
    Task<ServiceResult> DenyProviderAsync(DenyProvider message);
    Task<ServiceResult<ProviderGeneralInformationAndOfficesResult>> GetProviderGeneralInformationAndOfficesByIdAsync(GetProviderGeneralInformationAndOfficesById message);
    Task<ServiceResult> UpdateProviderGeneralInformationAndOfficesAsync(UpdateProviderGeneralInformationAndOffices message);
    Task<ServiceResult<IPaginatedData<ProviderInfoResult>>> GetProvidersInfoAsync(GetProvidersInfo message);
    Task<ServiceResult<IEnumerable<IProviderOffice>>> GetProviderOfficesAsync(GetProviderOffices message);
    Task<ServiceResult<IEnumerable<ProviderServiceInfoResult>>> GetProviderServicesAsync(GetProviderServices message);
    Task<ServiceResult<UserByUidResult>> GetUserByUidAsync(GetUserByUid message);
    Task<ServiceResult<Guid>> AdministratorRegisterUserAsync(AdministratorRegisterUser message);
    Task<ServiceResult> AdministratorUpdateUserAsync(AdministratorUpdateUser message);
    Task<ServiceResult<IEnumerable<AdministratorActionResult>>> GetUserAdministratorActionsAsync(GetUserAdministratorActions message);
    Task<ServiceResult> DeleteUserAsync(DeleteUser message);
    Task<ServiceResult> RegisterDoneServiceAsync(RegisterDoneService message);
}
