namespace eID.RO.Contracts.Results;

public interface EmpowermentWithdrawalReasonResult
{
    Guid Id { get; }

    IEnumerable<EmpowermentWithdrawalReasonTranslationResult> Translations { get; }
}
