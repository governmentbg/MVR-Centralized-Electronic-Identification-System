using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class UpdateServiceRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new UpdateServiceRequestValidator();
    public Guid UserId { get; set; }
    public Guid ProviderDetailsId { get; set; }
    public Guid ServiceId { get; set; }
    public bool IsEmpowerment { get; set; }
    public IEnumerable<string> ServiceScopeNames { get; set; } = new List<string>();
    public IEnumerable<CollectablePersonalInformation> RequiredPersonalInformation { get; set; }
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
}

public class UpdateServicePayload
{
    public Guid UserId { get; set; }
    public Guid ProviderDetailsId { get; set; }
    public bool IsEmpowerment { get; set; }
    public IEnumerable<string> ServiceScopeNames { get; set; } = new List<string>();
    public IEnumerable<CollectablePersonalInformation> RequiredPersonalInformation { get; set; }
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
}

internal class UpdateServiceRequestValidator : AbstractValidator<UpdateServiceRequest>
{
    public UpdateServiceRequestValidator()
    {
        RuleFor(r => r.ServiceId).NotEmpty();
        RuleFor(r => r.ProviderDetailsId).NotEmpty();
        RuleFor(r => r.UserId).NotEmpty();
        When(r => r.RequiredPersonalInformation != null, () =>
        {
            RuleFor(r => r.RequiredPersonalInformation).NotEmpty();
            RuleForEach(r => r.RequiredPersonalInformation)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .IsInEnum();
        });

        RuleFor(r => r.MinimumLevelOfAssurance).NotEmpty().IsInEnum();

        When(r => r.IsEmpowerment, () =>
        {
            RuleFor(r => r.ServiceScopeNames)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(r => IsDistinct(r))
                .WithMessage("There are more than one element with the same value");

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

