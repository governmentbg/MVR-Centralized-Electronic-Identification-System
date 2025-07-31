using eID.Signing.Contracts.Commands;
using FluentValidation;

namespace eID.Signing.API.Requests;

public class BoricaAddAccessTokenValidator : AbstractValidator<BoricaAddAccessToken>
{
    public BoricaAddAccessTokenValidator()
    {
        RuleFor(r => r.AccessTokenValue).NotEmpty();
        RuleFor(r => r.ExpirationDate).NotEmpty();
    }
}
