using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.Service.Validators;

public class BoricaDownloadFileByTransactyionIdValidator : AbstractValidator<BoricaDownloadFileByTransactionId>
{
    public BoricaDownloadFileByTransactyionIdValidator()
    {
        RuleFor(r => r.TransactionId).NotEmpty();
    }
}
