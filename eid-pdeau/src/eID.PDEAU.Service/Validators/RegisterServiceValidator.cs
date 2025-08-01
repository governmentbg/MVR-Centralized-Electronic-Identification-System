using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

internal class RegisterServiceValidator : AbstractValidator<RegisterService>
{
    public RegisterServiceValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.ProviderDetailsId).NotEmpty();
        RuleFor(r => r.Name).NotEmpty();
        When(r => r.RequiredPersonalInformation != null, () =>
        {
            RuleFor(r => r.RequiredPersonalInformation).NotEmpty();
            RuleForEach(r => r.RequiredPersonalInformation)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .IsInEnum();
        });
        RuleFor(r => r.MinimumLevelOfAssurance).NotEmpty().IsInEnum();

        RuleFor(r => r.ServiceScopeNames)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(r => IsDistinct(r))
            .WithMessage("There are more than one element with the same value");

        When(r => !r.IsEmpowerment, () =>
        {
            RuleForEach(s => s.ServiceScopeNames)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(Contracts.Constants.ServiceScopeNameMaxLength);
        });
    }

    private bool IsDistinct(IEnumerable<string> elements)
    {
        if (elements is null)
        {
            return false;
        }

        var encounteredIds = new HashSet<string>();

        foreach (var element in elements)
        {
            if (!encounteredIds.Contains(element))
            {
                encounteredIds.Add(element);
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}
