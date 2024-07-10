using eID.RO.Contracts.Results;

namespace eID.RO.Service.Entities;

public class EmpowermentDisagreementReason : EmpowermentDisagreementReasonResult
{
    public Guid Id { get; set; }
    public ICollection<EmpowermentDisagreementReasonTranslation> Translations { get; set; } = new List<EmpowermentDisagreementReasonTranslation>();

    IEnumerable<EmpowermentDisagreementReasonTranslationResult> EmpowermentDisagreementReasonResult.Translations { get => Translations; }
}
