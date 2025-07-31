namespace eID.MIS.Contracts.Results;

public class ServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}
