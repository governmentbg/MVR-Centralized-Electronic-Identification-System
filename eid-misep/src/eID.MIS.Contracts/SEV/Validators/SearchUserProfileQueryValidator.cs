using eID.MIS.Contracts.SEV.External;
using FluentValidation;

namespace eID.MIS.Contracts.SEV.Validators;

public class SearchUserProfileQueryValidator : AbstractValidator<SearchUserProfileQuery>
{
    public SearchUserProfileQueryValidator()
    {
        RuleFor(x => x.EIdentityId).NotEmpty();
        RuleFor(x => x.Identifier).NotEmpty();
        RuleFor(x => x.TargetGroupId).NotEmpty();
    }
}
