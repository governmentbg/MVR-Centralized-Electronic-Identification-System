namespace eID.RO.Contracts.Results;

public interface EmpowermentsVerificationSagasCheckResult
{
    public bool AllSagasExistAndFinished { get; set; }
    public IEnumerable<Guid> MissingIds { get; set; }
}
