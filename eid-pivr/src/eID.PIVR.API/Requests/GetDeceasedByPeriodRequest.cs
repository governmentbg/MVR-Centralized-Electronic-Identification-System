using FluentValidation;

namespace eID.PIVR.API.Requests;

public class GetDeceasedByPeriodRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetDeceasedByPeriodRequestValidator();
    /// <summary>
    /// Created on from
    /// </summary>
    public DateTime From { get; set; }
    /// <summary>
    /// Created on to
    /// </summary>
    public DateTime To { get; set; }
}

internal class GetDeceasedByPeriodRequestValidator : AbstractValidator<GetDeceasedByPeriodRequest>
{
    public GetDeceasedByPeriodRequestValidator()
    {
        RuleFor(r => r.From)
            .NotEmpty();

        RuleFor(r => r.To)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.From <= x.To)
                .WithMessage($"{nameof(GetDeceasedByPeriodRequest.From)} must lower or equal than {nameof(GetDeceasedByPeriodRequest.To)}");
    }
}
