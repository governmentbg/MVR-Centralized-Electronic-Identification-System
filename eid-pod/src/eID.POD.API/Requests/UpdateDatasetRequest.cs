using FluentValidation;

namespace eID.POD.API.Requests;

public class UpdateDatasetRequest : IValidatableRequest
{
    public Guid Id { get; set; }
    public string DatasetName { get; set; }
    public string CronPeriod { get; set; }
    public string DataSource { get; set; }
    public bool IsActive { get; set; }
    public virtual IValidator GetValidator() => new UpdateDatasetRequestValidator();
}

public class UpdateDatasetRequestValidator : AbstractValidator<UpdateDatasetRequest>
{
    public UpdateDatasetRequestValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.DatasetName).NotEmpty();
        RuleFor(r => r.CronPeriod)
            .NotEmpty()
            .Must(s => ValidatorHelpers.IsValidCronExpression(s))
            .WithMessage("Invalid Cron Expression provided");

        RuleFor(r => r.DataSource)
            .NotEmpty()
            .Must(s => ValidatorHelpers.IsValidAbsoluteUrl(s))
            .WithMessage("{PropertyName} must be absolute url.");
    }
}
