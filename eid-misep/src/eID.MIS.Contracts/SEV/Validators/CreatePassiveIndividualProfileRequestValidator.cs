using eID.MIS.Contracts.SEV.External;
using FluentValidation;

namespace eID.MIS.Contracts.SEV.Validators;

public class CreatePassiveIndividualProfileRequestValidator : AbstractValidator<CreatePassiveIndividualProfileRequest>
{
    public CreatePassiveIndividualProfileRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .Must(x => ValidatorHelpers.UidFormatIsValid(x) || ValidatorHelpers.EikFormatIsValid(x))
                .WithMessage("{PropertyName} is invalid.");

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.MiddleName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Phone)
            .NotEmpty();

        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address)
                .SetValidator(new AddressDataValidator());
        });
    }
}
