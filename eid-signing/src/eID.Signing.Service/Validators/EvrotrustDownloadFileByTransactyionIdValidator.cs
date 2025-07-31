using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.Service.Validators;

public class EvrotrustDownloadFileByTransactyionIdValidator : AbstractValidator<EvrotrustDownloadFileByTransactionId>
{
    public EvrotrustDownloadFileByTransactyionIdValidator()
    {
        RuleFor(r => r.TransactionId).NotEmpty();
    }
}
