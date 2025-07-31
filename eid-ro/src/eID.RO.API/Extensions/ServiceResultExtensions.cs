using eID.RO.Contracts.Results;

namespace eID.RO.API.Extensions;

public static class ServiceResultExtensions
{
    public  static ServiceResult AsPlainServiceResult(this ServiceResult result)
        => new ServiceResult { Error = result.Error, Errors = result.Errors, StatusCode = result.StatusCode };
}
