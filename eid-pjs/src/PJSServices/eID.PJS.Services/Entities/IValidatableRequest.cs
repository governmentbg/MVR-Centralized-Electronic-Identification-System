using FluentValidation;
using FluentValidation.Results;

namespace eID.PJS.Services;

/// <summary>
/// Interface to mark types that can be validated using FluentValidation
/// </summary>
public interface IValidatableRequest
{
    /// <summary>
    /// Gets the validator.
    /// </summary>
    /// <returns></returns>
    IValidator GetValidator();
}

/// <summary>
/// Extensions for validation of requests
/// </summary>
public static class ValidatableRequestExtensions
{
    /// <summary>
    /// Returns true if the entity is valid.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>
    ///   <c>true</c> if the specified request is valid; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValid(this IValidatableRequest request)
    {
        var validationResult = request.GetValidator().Validate(new ValidationContext<IValidatableRequest>(request));
        return validationResult.IsValid;
    }

    /// <summary>
    /// Gets the validation result.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public static ValidationResult GetValidationResult(this IValidatableRequest request)
        => request.GetValidator().Validate(new ValidationContext<IValidatableRequest>(request));

    /// <summary>
    /// Validates the specified request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns></returns>
    public static ValidationResult Validate(this IValidatableRequest request)
        => request.GetValidator().Validate(new ValidationContext<IValidatableRequest>(request));

}
