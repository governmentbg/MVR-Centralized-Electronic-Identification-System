using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.Service.Validators;

public class BoricaGetFileStatusByTransactyionIdValidator : AbstractValidator<BoricaGetFileStatusByTransactionId>
{
    public BoricaGetFileStatusByTransactyionIdValidator()
    {
        RuleFor(r => r.TransactionId).NotEmpty();
    }
}
