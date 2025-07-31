using FluentValidation;

namespace eID.Signing.API.Requests;

public class BoricaAddAccessTokenRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new BoricaAddAccessTokenRequestValidator();

    public string AccessTokenValue { get; set; }
    public DateOnly ExpirationDate { get; set; }
}

public class BoricaAddAccessTokenRequestValidator : AbstractValidator<BoricaAddAccessTokenRequest>
{
    public BoricaAddAccessTokenRequestValidator()
    {
        RuleFor(r => r.AccessTokenValue).NotEmpty();
        RuleFor(r => r.ExpirationDate).NotEmpty();
    }
}
