using eID.PDEAU.Contracts.Results;
using FluentValidation;

namespace eID.PDEAU.Service.Requests;

public class CheckDeauRequesterInBulstatRequest
{
    public Guid CorrelationId { get; set; }
    public IAuthorizerUidData AuthorizerData { get; set; }
    public string Uid { get; set; } = string.Empty;
}

public class CheckDeauRequesterInBulstatRequestValidator : AbstractValidator<CheckDeauRequesterInBulstatRequest>
{
    public CheckDeauRequesterInBulstatRequestValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();
        RuleFor(r => r.Uid).NotEmpty();
        RuleFor(r => r.AuthorizerData).NotEmpty();
    }
}
