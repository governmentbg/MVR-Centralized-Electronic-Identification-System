using FluentValidation;

namespace eID.PIVR.API.Requests;

public class GetApiUsageByYearRequest : IValidatableRequest
{
    /// <summary>
    /// Internal validator for the request
    /// </summary>
    /// <returns></returns>
    public virtual IValidator GetValidator() => new GetApiUsageByYearRequestValidator();
    /// <summary>
    /// Report for the requested year will be returned, grouped by month and registry. Default: DateTime.UtcNow.Year
    /// </summary>
    public int Year { get; set; } = DateTime.UtcNow.Year;
}

internal class GetApiUsageByYearRequestValidator : AbstractValidator<GetApiUsageByYearRequest>
{
    public GetApiUsageByYearRequestValidator()
    {
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
