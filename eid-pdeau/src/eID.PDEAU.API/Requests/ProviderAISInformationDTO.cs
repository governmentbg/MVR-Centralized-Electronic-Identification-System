using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Requests;

public class ProviderAISInformationDTO : ProviderAISInformation
{
    public string Name { get; set; }
    public string Project { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public DestinationIpType DestinationIpType { get; set; }
    public string ProtocolPort { get; set; }
}

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
