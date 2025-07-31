using FluentValidation;
using FluentValidation.Results;

namespace eID.PAN.API.Public.Requests;

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

    public static IEnumerable<KeyValuePair<string, string>> GetValidationErrorList(this ValidationResult result)
        => result.Errors.Select(e => new KeyValuePair<string, string>(e.PropertyName, e.ErrorMessage));
}
