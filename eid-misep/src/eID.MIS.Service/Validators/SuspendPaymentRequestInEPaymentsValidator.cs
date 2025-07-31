using eID.MIS.Contracts.EP.Commands;
using FluentValidation;

namespace eID.MIS.Service.Validators;

internal class SuspendPaymentRequestInEPaymentsValidator : AbstractValidator<SuspendPaymentRequestInEPayments>
{
    public SuspendPaymentRequestInEPaymentsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PaymentRequestId).NotEmpty();
    }
}
