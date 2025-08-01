using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetUserDetailsValidator : AbstractValidator<GetUserDetails>
{
    public GetUserDetailsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
    }
}
