using eID.RO.Contracts.Enums;
using FluentValidation;

namespace eID.RO.API.Public.Requests;

/// <summary>
/// Get empowerments by DEAU
/// </summary>
public class GetEmpowermentsByDeauRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetEmpowermentsByDeauRequestValidator();
    
    /// <summary>
    /// Type of Behalf of: Individual or LegalEntity
    /// </summary>
    public OnBehalfOf OnBehalfOf { get; set; }

    /// <summary>
    /// Authorizer Eik/Egn/Lnch
    /// </summary>
    public string AuthorizerUid { get; set; } = string.Empty;
    /// <summary>
    /// Used when OnBehalfOf.Individual
    /// </summary>
    public IdentifierType AuthorizerUidType { get; set; }

    /// <summary>
    /// Empowered person Egn/Lnch
    /// </summary>
    public string EmpoweredUid { get; set; } = string.Empty;
    /// <summary>
    /// Indicates what type of Uid EmpoweredUid contains
    /// </summary>
    public IdentifierType EmpoweredUidType { get; set; }

    /// <summary>
    /// Service Id
    /// </summary>
    public int ServiceId { get; set; }
    /// <summary>
    /// Volume of representation.
    /// Optional
    /// </summary>
    public List<string>? VolumeOfRepresentation { get; set; }
    /// <summary>
    /// Status on date time
    /// </summary>
    public DateTime StatusOn { get; set; }
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
    /// <summary>
    /// Page index
    /// </summary>
    public int PageIndex { get; set; } = 1;
    public EmpowermentsByDeauSortBy? SortBy { get; set; }
    public SortDirection SortDirection { get; set; }
}

public class GetEmpowermentsByDeauRequestValidator : AbstractValidator<GetEmpowermentsByDeauRequest>
{
    public GetEmpowermentsByDeauRequestValidator()
    {
        //RuleFor(x => x.OnBehalfOf).NotEmpty().IsInEnum();
        When(x => x.OnBehalfOf == OnBehalfOf.LegalEntity, () =>
        {
            // We validate Uid of LegalEntities for Uid as well because there are cases where it can be used as Bulstat. Examples: ЕТ, свободни професии и др.
            RuleFor(x => x.AuthorizerUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(x => ValidatorHelpers.EikFormatIsValid(x) || ValidatorHelpers.UidFormatIsValid(x))
                    .WithMessage("AuthorizerUid contains invalid Eik.");
        });

        When(x => x.OnBehalfOf == OnBehalfOf.Individual, () =>
        {
            When(r => r.AuthorizerUidType == IdentifierType.EGN, () => {
                RuleFor(r => r.AuthorizerUid)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                        .WithMessage("{PropertyName} invalid EGN.")
                    .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                    .WithMessage("{PropertyName} people below lawful age.");
            });

            When(r => r.AuthorizerUidType == IdentifierType.LNCh, () => {
                RuleFor(r => r.AuthorizerUid)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid LNCh.");
            });
            RuleFor(r => r.AuthorizerUidType).NotEmpty().IsInEnum();
        });

        When(x => x.OnBehalfOf == OnBehalfOf.Empty, () =>
        {
            RuleFor(x => x.AuthorizerUid).NotEmpty();
        });

        RuleFor(x => x.EmpoweredUid).NotEmpty();
        When(r => r.EmpoweredUidType == IdentifierType.EGN, () => {
            RuleFor(r => r.EmpoweredUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} invalid EGN.")
                .Must(uid => ValidatorHelpers.IsLawfulAge(uid))
                .WithMessage("{PropertyName} people below lawful age.");
        });

        When(r => r.EmpoweredUidType == IdentifierType.LNCh, () => {
            RuleFor(r => r.EmpoweredUid)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid LNCh.");
        });
        RuleFor(r => r.EmpoweredUidType).NotEmpty().IsInEnum();

        When(x => x.VolumeOfRepresentation != null, () =>
        {
            RuleForEach(x => x.VolumeOfRepresentation).NotEmpty();
        });

        RuleFor(x => x.StatusOn).NotEmpty();

        RuleFor(r => r.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.SortBy).IsInEnum();
        When(r => r.SortBy.HasValue, () =>
        {
            RuleFor(r => r.SortDirection).IsInEnum();
        });
    }
}
