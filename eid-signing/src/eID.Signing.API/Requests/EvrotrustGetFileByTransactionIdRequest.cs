using FluentValidation;

namespace eID.Signing.API.Requests;

/// <summary>
/// Get signing status for file by transactionId and groupSigning
/// </summary>
public class EvrotrustGetFileByTransactionIdRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new EvrotrustGetFileByTransactionIdRequestValidator();

    /// <summary>
    /// Id of the previous signing transaction
    /// </summary>
    public string? TransactionId { get; set; }
    /// <summary>
    /// set group signing value
    /// </summary>
    public bool GroupSigning { get; set; }
}

public class EvrotrustGetFileByTransactionIdRequestValidator : AbstractValidator<EvrotrustGetFileByTransactionIdRequest>
{
    public EvrotrustGetFileByTransactionIdRequestValidator()
    {
        RuleFor(r => r.TransactionId).NotEmpty();
    }
}
