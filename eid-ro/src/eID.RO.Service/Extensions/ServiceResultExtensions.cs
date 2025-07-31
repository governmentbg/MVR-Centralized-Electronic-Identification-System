using eID.RO.Contracts.Results;

namespace eID.RO.Service.Extensions;

public static class ServiceResultExtensions
{
    public static ServiceResult<T> ToType<T>(this ServiceResult result, T? data = default)
        => new ServiceResult<T> { Error = result.Error, Errors = result.Errors, Result = data, StatusCode = result.StatusCode };
}
