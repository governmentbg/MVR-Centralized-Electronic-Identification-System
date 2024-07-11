namespace eID.RO.Contracts.Results;

public class ServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}
