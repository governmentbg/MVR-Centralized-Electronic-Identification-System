using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Results;
using FluentValidation;

namespace eID.PDEAU.API.Public.Admin.Requests;

public class UpdateProviderGeneralInformationAndOfficesPayload
{
    public string GeneralInformation { get; set; } = string.Empty;
    public IEnumerable<IProviderOffice> Offices { get; set; } = new List<ProviderOffice>();
}

public class UpdateProviderGeneralInformationAndOfficesRequest : IValidatableRequest, UpdateProviderGeneralInformationAndOffices
{
    public virtual IValidator GetValidator() => new UpdateProviderGeneralInformationAndOfficesRequestValidator();

    public string GeneralInformation { get; set; } = string.Empty;
    public IEnumerable<IProviderOffice> Offices { get; set; } = new List<ProviderOffice>();

    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
}

public class ProviderOffice : IProviderOffice
{
    public string Name { get; set; }
    public string Address { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}

internal class UpdateProviderGeneralInformationAndOfficesRequestValidator : AbstractValidator<UpdateProviderGeneralInformationAndOfficesRequest>
{
    public UpdateProviderGeneralInformationAndOfficesRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(r => r.CorrelationId)
            .NotEmpty();

        RuleFor(r => r.Id)
            .NotEmpty();

        RuleFor(r => r.GeneralInformation)
            .NotEmpty()
            .MaximumLength(Constants.Providers.GeneralInformationMaxLength);

        RuleFor(r => r.Offices)
            .NotEmpty()
            .ForEach(r => r.SetValidator(new ProviderOfficeValidator()))
            .Must(r => IsDistinct(r))
            .WithMessage("There are more than one element with the same value");
    }

    private bool IsDistinct(IEnumerable<IProviderOffice> elements)
    {
        if (elements is null)
        {
            return false;
        }

        return elements.Count() == elements.DistinctBy(d => (d.Name, d.Address, d.Lat, d.Lon)).Count();
    }
}

internal class ProviderOfficeValidator : AbstractValidator<IProviderOffice>
{
    public ProviderOfficeValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(Constants.ProviderOffices.NameMaxLength);

        RuleFor(r => r.Address)
            .NotEmpty()
            .MaximumLength(Constants.ProviderOffices.AddressMaxLength);

        RuleFor(r => r.Lat)
            .NotEmpty();

        RuleFor(r => r.Lon)
            .NotEmpty();
    }
}
