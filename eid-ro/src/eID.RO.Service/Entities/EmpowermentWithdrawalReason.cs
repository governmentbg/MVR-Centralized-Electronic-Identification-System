using eID.RO.Contracts.Results;

namespace eID.RO.Service.Entities;

public class EmpowermentWithdrawalReason : EmpowermentWithdrawalReasonResult
{
    public Guid Id { get; set; }

    public ICollection<EmpowermentWithdrawalReasonTranslation> Translations { get; set; } = new List<EmpowermentWithdrawalReasonTranslation>();

    IEnumerable<EmpowermentWithdrawalReasonTranslationResult> EmpowermentWithdrawalReasonResult.Translations { get => Translations; }
}
