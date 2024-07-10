using FluentValidation;
using FluentValidation.Results;

namespace eID.RO.API.Public.Requests;

public interface IValidatableRequest
{
    IValidator GetValidator();
}

public static class ValidatableRequestExtensions
{
    public static bool IsValid(this IValidatableRequest request)
    {
        var validationResult = request.GetValidator().Validate(new ValidationContext<IValidatableRequest>(request));
        return validationResult.IsValid;
    }

    public static ValidationResult GetValidationResult(this IValidatableRequest request)
        => request.GetValidator().Validate(new ValidationContext<IValidatableRequest>(request));
}
