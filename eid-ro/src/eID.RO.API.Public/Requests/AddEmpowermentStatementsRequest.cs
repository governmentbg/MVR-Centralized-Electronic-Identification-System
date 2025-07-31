using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

/// <summary>
/// Used for creating new empowerment statements
/// </summary>
public class AddEmpowermentStatementsRequest
{
    /// <summary>
    /// Setting the type of empowering entity
    /// </summary>
    public OnBehalfOf OnBehalfOf { get; set; }
    /// <summary>
    /// Name of legal entity. When OnBehalfOf.Individual this is taken from the token.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Uid of legal entity. When OnBehalfOf.Individual this is taken from the token.
    /// </summary>
    public string Uid { get; set; } = string.Empty;
    /// <summary>
    /// When OnBehalfOf.LegalEntity the value doesn't matter.
    /// If OnBehalfOf.Individual this is taken from the token.
    /// </summary>
    public IdentifierType UidType { get; set; }
    /// <summary>
    /// List of EGNs or LNCHs of empowered people
    /// </summary>
    public IEnumerable<UserIdentifierData> EmpoweredUids { get; set; } = Enumerable.Empty<UserIdentifierData>();
    /// <summary>
    /// Determines if all people are empowered together or separately
    /// </summary>
    public TypeOfEmpowerment TypeOfEmpowerment { get; set; }
    /// <summary>
    /// Representation of provider - extended reference
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;
    /// <summary>
    /// Provider name, collected and stored in the moment of execution
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;
    /// <summary>
    /// Numeric representation of service, depends on selected provider - extended reference
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// Service Name, collected and stored in the moment of execution
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;
    /// <summary>
    /// Name of the position the issuer has in the legal entity
    /// </summary>
    public string? IssuerPosition { get; set; }
    /// <summary>
    /// List of all selected actions, that can be performed over selected service
    /// </summary>
    public IEnumerable<VolumeOfRepresentationRequest> VolumeOfRepresentation { get; set; } = Enumerable.Empty<VolumeOfRepresentationRequest>();
    /// <summary>
    /// UTC. On this date, once verified and signed, the empowerment can be considered active.
    /// If not provided, the empowerment will become immediately active after signing.
    /// Default: DateTime.UtcNow
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// UTC. Empowerment statement will be active before this moment. Must be at least 1 hour after current time.
    /// Endless empowerment if this date is null
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// List of EGNs or LNCHs of Authorizer people
    /// </summary>
    public IList<AuthorizerIdentifierData> AuthorizerUids { get; set; } = new List<AuthorizerIdentifierData>();
}

public class AddEmpowermentStatementRequestValidator : AbstractValidator<AddEmpowermentStatementsRequest>
{
    public AddEmpowermentStatementRequestValidator(bool allowSelfEmpowerment)
    {
        RuleFor(r => r.OnBehalfOf).NotEmpty().IsInEnum();
        When(r => r.OnBehalfOf == OnBehalfOf.LegalEntity, () =>
        {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EikFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid Eik.");
            RuleFor(r => r.IssuerPosition).NotEmpty();
            RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.UidType).Equal(IdentifierType.NotSpecified);
        });
        When(r => r.OnBehalfOf == OnBehalfOf.Individual, () =>
        {
            RuleFor(r => r.UidType).NotEmpty().IsInEnum();
            When(r => r.UidType == IdentifierType.EGN, () => {
                RuleFor(r => r.Uid)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                        .WithMessage("{PropertyName} invalid EGN.")
                    .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                    .WithMessage("{PropertyName} people below lawful age.");
            });

            When(r => r.UidType == IdentifierType.LNCh, () => {
                RuleFor(r => r.Uid)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid LNCh.");
            });
        });
        RuleFor(r => r.Uid).NotEmpty();

        RuleFor(r => r.EmpoweredUids)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new UserIdentifierValidator()));

        RuleFor(r => r.TypeOfEmpowerment).IsInEnum();
        RuleFor(r => r.ProviderId).NotEmpty();
        RuleFor(r => r.ProviderName).NotEmpty();
        RuleFor(r => r.ServiceName).NotEmpty();
        RuleFor(r => r.VolumeOfRepresentation).NotEmpty()
            .ForEach(r => r.SetValidator(new VolumeOfRepresentationRequestValidator()));
        RuleFor(r => r.StartDate).NotEmpty();
        When(r => r.ExpiryDate.HasValue, () =>
        {
            RuleFor(r => r.ExpiryDate).Cascade(CascadeMode.Stop).GreaterThanOrEqualTo(r => r.StartDate);
        });

        RuleFor(r => r.AuthorizerUids)
            .NotEmpty()
            .Must(r => HaveNoDuplicates(r))
                .WithMessage("{PropertyName} contains duplicates.")
            .ForEach(r => r.SetValidator(new UserIdentifierValidator()));

        if (!allowSelfEmpowerment)
        {
            When(r => r.OnBehalfOf == OnBehalfOf.Individual, () =>
            {
                RuleFor(r => r.AuthorizerUids.Any(au => r.EmpoweredUids.Any(eu => eu.Uid == au.Uid && eu.UidType == au.UidType)))
                    .NotEqual(true)
                    .WithName("AuthorizerUids, EmpoweredUids")
                    .WithMessage("Self empowerment is not allowed");
            });

            When(r => r.OnBehalfOf == OnBehalfOf.LegalEntity && r.AuthorizerUids.DistinctBy(au => (au.Uid, au.UidType)).Count() == 1, () =>
            {
                RuleFor(r => r.EmpoweredUids.Any(eu => eu.Uid == r.AuthorizerUids.First().Uid
                            && eu.UidType == r.AuthorizerUids.First().UidType))
                    .NotEqual(true)
                    .WithName("AuthorizerUids, EmpoweredUids")
                    .WithMessage("Self empowerment is not allowed");
            });
        }
    }
    private bool HaveNoDuplicates(IList<AuthorizerIdentifierData> items)
    {
        if (items is null)
        {
            return true;
        }
        return items.Select(r => r.Uid).Distinct().Count() == items.Count;
    }
}
public class VolumeOfRepresentationRequest : VolumeOfRepresentationResult
{
    public string Name { get; set; } = string.Empty;
}
public class VolumeOfRepresentationRequestValidator : AbstractValidator<VolumeOfRepresentationRequest>
{
    public VolumeOfRepresentationRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty();
    }
}

public class UserIdentifierValidator : AbstractValidator<UserIdentifier>
{
    public UserIdentifierValidator()
    {
        When(r => r.UidType == IdentifierType.EGN, () => {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} people below lawful age.");
        });

        When(r => r.UidType == IdentifierType.LNCh, () => {
            RuleFor(r => r.Uid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid LNCh.");
        });

        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(200)
            .Must(name => ValidatorHelpers.UserIdentifierNameIsValid(name))
            .WithMessage("{PropertyName} contains invalid symbols. Only Bulgarian letters, dashes, apostrophes and space are allowed");
    }
}
