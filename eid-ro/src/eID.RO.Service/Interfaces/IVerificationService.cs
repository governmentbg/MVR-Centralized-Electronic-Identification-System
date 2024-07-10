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
    Task<ServiceResult> VerifySignatureAsync(string originalFile, string signature, string uid, IdentifierType uidType, SignatureProvider signatureProvider);
    Task<ServiceResult<bool>> VerifyUidsLawfulAgeAsync(VerifyUidsLawfulAge message);
    Task<ServiceResult<LegalEntityActualState>> GetLegalEntityActualStateAsync(string uid);
    Task<ServiceResult<LegalEntityStateOfPlay>> GetBulstatStateOfPlayByUidAsync(string uid);
    Task<ServiceResult<LegalEntityBulstatVerificationResult>> CheckLegalEntityInBulstatAsync(CheckLegalEntityInBulstatRequest request);
}
