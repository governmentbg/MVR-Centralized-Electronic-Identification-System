using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.Service.Validators;

public class EvrotrustGetFileStatusByTransactyionIdValidator : AbstractValidator<EvrotrustGetFileStatusByTransactionId>
{
    public EvrotrustGetFileStatusByTransactyionIdValidator()
    {
        RuleFor(r => r.TransactionId).NotEmpty();
    }
}
