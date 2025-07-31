using eID.MIS.Contracts.EP.Commands;
using FluentValidation;

namespace eID.MIS.Service.Validators;

internal class GetPaymentRequestStatusValidator : AbstractValidator<GetPaymentRequestStatus>
{
    public GetPaymentRequestStatusValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.PaymentRequestId).NotEmpty();
        RuleFor(r => r.CitizenProfileId).NotEmpty();
    }
}
