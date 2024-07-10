using eID.POD.Contracts.Commands;
using FluentValidation;

namespace eID.POD.Service.Validators;

public class CreateDatasetValidator : AbstractValidator<CreateDataset>
{
    public CreateDatasetValidator()
    {
        RuleFor(r => r.DatasetName).NotEmpty();

        RuleFor(r => r.CronPeriod)
            .NotEmpty()
            .Must(s => ValidatorHelpers.IsValidCronExpression(s))
            .WithMessage("Invalid Cron Expression provided");
           
        RuleFor(r => r.DataSource)
            .NotEmpty()
            .Must(s => ValidatorHelpers.IsValidAbsoluteUrl(s))
            .WithMessage("{PropertyName} must be absolute url.");

        RuleFor(r => r.CreatedBy).NotEmpty();
    }
}
