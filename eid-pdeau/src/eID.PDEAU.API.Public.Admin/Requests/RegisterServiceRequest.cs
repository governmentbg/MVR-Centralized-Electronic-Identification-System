using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class RegisterServiceRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterServiceRequestValidator();
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid ProviderDetailsId { get; set; }
    public bool IsEmpowerment { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? PaymentInfoNormalCost { get; set; }
    public IEnumerable<CollectablePersonalInformation> RequiredPersonalInformation { get; set; }
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
    public IEnumerable<string> ServiceScopeNames { get; set; } = new List<string>();
}

public class RegisterServicePayload
{
    public Guid ProviderDetailsId { get; set; }
    public bool IsEmpowerment { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? PaymentInfoNormalCost { get; set; }
    public IEnumerable<CollectablePersonalInformation> RequiredPersonalInformation { get; set; }
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
    public IEnumerable<string> ServiceScopeNames { get; set; } = new List<string>();
}

internal class RegisterServiceRequestValidator : AbstractValidator<RegisterServiceRequest>
{
    public RegisterServiceRequestValidator()
    {
        RuleFor(r => r.ProviderDetailsId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
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

