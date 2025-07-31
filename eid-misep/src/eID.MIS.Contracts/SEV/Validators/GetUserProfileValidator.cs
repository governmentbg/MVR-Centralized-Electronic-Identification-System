using eID.MIS.Contracts.SEV.Commands;
using FluentValidation;

namespace eID.MIS.Contracts.SEV.Validators;

public class GetUserProfileValidator : AbstractValidator<GetUserProfile>
{
    public GetUserProfileValidator()
    {
        RuleFor(x => x.CorrelationId).NotEmpty();
        RuleFor(x => x.EIdentityId).NotEmpty();
        RuleFor(x => x.ProfileId).NotEmpty();
    }
}
