using FluentValidation;

namespace eID.Signing.API.Requests;

/// <summary>
/// Get signing status for file by transactionId
/// </summary>
public class BoricaGetFileByTransactionIdRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new BoricaGetFileByTransactionIdRequestValidator();

    /// <summary>
    /// Id of the previous signing transaction
    /// </summary>
    public string? TransactionId { get; set; }
}

public class BoricaGetFileByTransactionIdRequestValidator : AbstractValidator<BoricaGetFileByTransactionIdRequest>
{
    public BoricaGetFileByTransactionIdRequestValidator()
    {
        RuleFor(r => r.TransactionId).NotEmpty();
    }
}
