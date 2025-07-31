#nullable enable
namespace eID.Signing.Contracts.Results;

public class ServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}
