using eID.MIS.Contracts.EP.Commands;
using FluentValidation;

namespace eID.MIS.Service.Validators;

internal class GetPaymentRequestsValidator : AbstractValidator<GetPaymentRequests>
{
    public GetPaymentRequestsValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.CitizenProfileId).NotEmpty();
    }
}
