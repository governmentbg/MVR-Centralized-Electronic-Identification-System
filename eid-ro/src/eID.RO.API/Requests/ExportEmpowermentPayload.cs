using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.API.Requests;

public class ExportEmpowermentPayload : IValidatableRequest
{
    public virtual IValidator GetValidator() => new ExportEmpowermentPayloadValidator();
    public string Html { get; set; }
}

public class ExportEmpowermentPayloadValidator : AbstractValidator<ExportEmpowermentPayload>
{
    public ExportEmpowermentPayloadValidator()
    {
        RuleFor(r => r.Html).NotEmpty();
    }
}
