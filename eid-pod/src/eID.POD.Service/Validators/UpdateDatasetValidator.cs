using eID.POD.Contracts.Commands;
using FluentValidation;

namespace eID.POD.Service.Validators;

public class UpdateDatasetValidator : AbstractValidator<UpdateDataset>
{
    public UpdateDatasetValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.DatasetName).NotEmpty();
        RuleFor(r => r.Description).MaximumLength(5000);
        When(r => r.IsActive, () => { 
            RuleFor(r => r.CronPeriod)
                .NotEmpty()
                .Must(s => ValidatorHelpers.IsValidCronExpression(s))
                .WithMessage("Invalid Cron Expression provided");
        });
        RuleFor(r => r.DataSource)
            .NotEmpty()
            .Must(s => ValidatorHelpers.IsValidAbsoluteUrl(s))
            .WithMessage("{PropertyName} must be absolute url.");
    }
}
