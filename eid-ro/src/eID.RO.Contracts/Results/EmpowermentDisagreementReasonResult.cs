namespace eID.RO.Contracts.Results;

public interface EmpowermentDisagreementReasonResult
{
    Guid Id { get; }
    IEnumerable<EmpowermentDisagreementReasonTranslationResult> Translations { get; }
}
