using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service.Requests;
using eID.RO.Service.Responses;

namespace eID.RO.Service.Interfaces;

public interface IVerificationService
{
    Task<ServiceResult<UidsRestrictionsResult>> CheckUidsRestrictionsAsync(CheckUidsRestrictions message);
    Task<ServiceResult<LegalEntityVerificationResult>> VerifyRequesterInLegalEntityAsync(CheckLegalEntityInNTR message);
    Task<ServiceResult> VerifySignatureAsync(Guid correlationId, string originalFile, string signature, string uid, IdentifierType uidType, SignatureProvider signatureProvider);
    Task<ServiceResult<bool>> VerifyUidsLawfulAgeAsync(VerifyUidsLawfulAge message);
    Task<ServiceResult<bool>> VerifyUidsRegistrationStatusAsync(VerifyUidsRegistrationStatus message);
    Task<ServiceResult<LegalEntityActualState>> GetLegalEntityActualStateAsync(Guid correlationId, string uid);
    Task<ServiceResult<LegalEntityStateOfPlay>> GetBulstatStateOfPlayByUidAsync(Guid correlationId, string uid);
    Task<ServiceResult<LegalEntityBulstatVerificationResult>> CheckLegalEntityInBulstatAsync(CheckLegalEntityInBulstatRequest request);
}
