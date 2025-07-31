using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class ProviderAISInformationValidator : AbstractValidator<ProviderAISInformation>
{
    public ProviderAISInformationValidator()
    {
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Project).NotEmpty().MaximumLength(200);
        RuleFor(r => r.SourceIp).NotEmpty().MaximumLength(50);
        RuleFor(r => r.DestinationIp).NotEmpty().MaximumLength(50);
        RuleFor(r => r.DestinationIpType).NotEmpty().IsInEnum();
        RuleFor(r => r.ProtocolPort).NotEmpty().MaximumLength(50);
    }
}

