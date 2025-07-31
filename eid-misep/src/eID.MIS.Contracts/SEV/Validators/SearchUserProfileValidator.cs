using eID.MIS.Contracts.SEV.Commands;
using FluentValidation;

namespace eID.MIS.Contracts.SEV.Validators;

public class SearchUserProfileValidator : AbstractValidator<SearchUserProfile>
{
    public SearchUserProfileValidator()
    {
        RuleFor(x => x.CorrelationId).NotEmpty();
        RuleFor(x => x.EIdentityId).NotEmpty();
        RuleFor(x => x.Identifier).NotEmpty();
        RuleFor(x => x.TargetGroupId).NotEmpty();
    }
}
