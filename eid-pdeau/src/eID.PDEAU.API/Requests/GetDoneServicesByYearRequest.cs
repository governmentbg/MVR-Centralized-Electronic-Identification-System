using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class GetDoneServicesByYearRequest : IValidatableRequest
{
    /// <summary>
    /// Internal validator for the request
    /// </summary>
    /// <returns></returns>
    public virtual IValidator GetValidator() => new GetDoneServicesByYearRequestValidator();
    /// <summary>
    /// Report for the requested year will be returned, grouped by provider, month and service. Default: DateTime.UtcNow.Year
    /// </summary>
    public int Year { get; set; } = DateTime.UtcNow.Year;
}

internal class GetDoneServicesByYearRequestValidator : AbstractValidator<GetDoneServicesByYearRequest>
{
    public GetDoneServicesByYearRequestValidator()
    {
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
