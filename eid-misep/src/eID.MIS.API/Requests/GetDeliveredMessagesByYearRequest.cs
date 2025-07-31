using eID.MIS.Contracts.Requests;
using FluentValidation;

namespace eID.MIS.API.Requests;

public class GetDeliveredMessagesByYearRequest : IValidatableRequest
{
    /// <summary>
    /// Internal validator for the request
    /// </summary>
    /// <returns></returns>
    public virtual IValidator GetValidator() => new GetDeliveredMessagesByYearRequestValidator();
    /// <summary>
    /// Report for the requested year will be returned, grouped by month. Default: DateTime.UtcNow.Year
    /// </summary>
    public int Year { get; set; } = DateTime.UtcNow.Year;
}

internal class GetDeliveredMessagesByYearRequestValidator : AbstractValidator<GetDeliveredMessagesByYearRequest>
{
    public GetDeliveredMessagesByYearRequestValidator()
    {
        RuleFor(r => r.Year).Cascade(CascadeMode.Stop).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}
