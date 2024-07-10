using eID.RO.Contracts.Results;

namespace eID.RO.Service.Entities;

public class EmpowermentDisagreementReasonTranslation : EmpowermentDisagreementReasonTranslationResult
{
    public string Language { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
