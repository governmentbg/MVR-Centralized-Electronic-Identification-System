using eID.RO.Contracts.Results;
using FluentValidation;

namespace eID.RO.Service.Requests;

public class CheckLegalEntityInBulstatRequest
{
    public IEnumerable<IAuthorizerUidData> AuthorizerUids { get; set; }
    public string Uid { get; set; } = string.Empty;
}

public class CheckLegalEntityInBulstatRequestValidator : AbstractValidator<CheckLegalEntityInBulstatRequest>
{
    public CheckLegalEntityInBulstatRequestValidator()
    {
        RuleFor(r => r.Uid).NotEmpty();
        RuleFor(r => r.AuthorizerUids)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .ForEach(uid => uid.NotNull());
    }
}
