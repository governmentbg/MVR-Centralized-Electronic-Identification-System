using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetProviderGeneralInformationAndOfficesByIdValidator : AbstractValidator<GetProviderGeneralInformationAndOfficesById>
{
    public GetProviderGeneralInformationAndOfficesByIdValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Id).NotEmpty();
    }
}
