using eID.PDEAU.Contracts.Commands;
using FluentValidation;

namespace eID.PDEAU.Service.Validators;

public class GetProviderFileValidator : AbstractValidator<GetProviderFile>
{
    public GetProviderFileValidator()
    {
        RuleFor(r => r.CorrelationId).NotEmpty();

        RuleFor(r => r.ProviderId)
            .NotEmpty()
            .When(r => !r.IsPublic);
        RuleFor(r => r.FileId).NotEmpty();
    }
}
