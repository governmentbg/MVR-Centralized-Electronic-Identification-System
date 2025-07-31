using eID.MIS.Contracts.EP.External;
using FluentValidation;

namespace eID.MIS.Contracts.EP.Validators;

public class CreatePaymentPayloadValidator : AbstractValidator<CreatePaymentPayload>
{
    public CreatePaymentPayloadValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CitizenProfileId).NotEmpty();
        RuleFor(x => x.Request)
            .NotNull().SetValidator(x => new RegisterPaymentRequestValidator());
    }
}

public class RegisterPaymentRequestValidator : AbstractValidator<RegisterPaymentRequest>
{
    public RegisterPaymentRequestValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;


        RuleFor(x => x.Actors)
            .NotNull()
            .NotEmpty();
        RuleForEach(x => x.Actors).NotNull().SetValidator(x => new PayerProfileValidator());

        RuleFor(x => x.PaymentData).NotNull().SetValidator(x => new PaymentDataValidator());
    }
}

public class PayerProfileValidator : AbstractValidator<PayerProfile>
{
    public PayerProfileValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Uid).NotNull();
        RuleFor(x => x.Uid.Value).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Uid.Type).IsInEnum();
        RuleFor(x => x.Name).NotEmpty();
    }
}

public class PaymentDataValidator : AbstractValidator<PaymentData>
{
    public PaymentDataValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        //RuleFor(x => x.TypeCode).Equals(0); // Такси за административни услуги | 448007
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ReferenceNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ReferenceType).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AdministrativeServiceUri).MaximumLength(100);
        RuleFor(x => x.AdministrativeServiceSupplierUri).MaximumLength(100);
    }
}
