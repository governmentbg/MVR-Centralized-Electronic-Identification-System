using FluentValidation;

namespace eID.RO.API.Requests;

public class GetActivatedEmpowermentsByYearRequest : IValidatableRequest
{
    /// <summary>
    /// Internal validator for the request
    /// </summary>
    /// <returns></returns>
    public virtual IValidator GetValidator() => new GetActivatedEmpowermentsByYearRequestValidator();
    /// <summary>
    /// Report for the requested year will be returned, grouped by month and service. Default: DateTime.UtcNow.Year
    /// </summary>
    public int Year { get; set; } = DateTime.UtcNow.Year;
}

internal class GetActivatedEmpowermentsByYearRequestValidator : AbstractValidator<GetActivatedEmpowermentsByYearRequest>
{
    public GetActivatedEmpowermentsByYearRequestValidator()
    {
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
