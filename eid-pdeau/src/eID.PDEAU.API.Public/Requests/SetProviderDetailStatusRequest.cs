using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

public class SetProviderDetailStatusRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new SetProviderDetailStatusRequestValidator();

    public Guid Id { get; set; }
    public ProviderDetailsStatusType Status { get; set; }
}

internal class SetProviderDetailStatusRequestValidator : AbstractValidator<SetProviderDetailStatusRequest>
{
    public SetProviderDetailStatusRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Status).IsInEnum().NotEmpty();
    }
}
