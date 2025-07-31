using eID.PIVR.Contracts.Enums;
using FluentValidation;

namespace eID.PIVR.API.Requests;

public class GetByPersonalIdQuery : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetByPersonalIdValidator();

    /// <summary>
    /// Can contain up to 10 digits.
    /// </summary>
    public string PersonalId { get; set; } = string.Empty;
    /// <summary>
    /// Uid type: EGN or LNCh
    /// </summary>
    public UidType UidType { get; set; }
}

public class GetByPersonalIdValidator : AbstractValidator<GetByPersonalIdQuery>
{
    public GetByPersonalIdValidator()
    {
        RuleFor(r => r.PersonalId).NotEmpty().Matches("^\\d+$").MaximumLength(10);
        RuleFor(r => r.UidType).NotEmpty().IsInEnum();
    }
}
