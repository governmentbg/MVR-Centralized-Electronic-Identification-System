using eID.MIS.Contracts.SEV.External;
using FluentValidation;


namespace eID.MIS.Contracts.SEV.Validators;

public class AddressDataValidator : AbstractValidator<AddressData>
{
    public AddressDataValidator()
    {
        RuleFor(x => x.Residence)
            .NotEmpty();
    }
}
