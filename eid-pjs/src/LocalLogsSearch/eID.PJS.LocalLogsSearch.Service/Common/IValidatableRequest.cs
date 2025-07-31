using FluentValidation;
using FluentValidation.Results;

namespace eID.PJS.LocalLogsSearch.Service;

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

    public static ValidationResult Validate(this IValidatableRequest request)
        => request.GetValidator().Validate(new ValidationContext<IValidatableRequest>(request));
}
