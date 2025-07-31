using FluentValidation;

namespace eID.POD.API.Requests;

public class CreateDatasetRequest : IValidatableRequest
{
    /// <summary>
    /// Name of the dataset
    /// </summary>
    public string DatasetName { get; set; }
    /// <summary>
    /// Cron expression used for job schedule
    /// </summary>
    public string CronPeriod { get; set; }
    /// <summary>
    /// Url holding the data which will be uploaded in OpenData
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// When creating Dataset you can set Active to true if you want to schedule a job for data upload at the moment of creating. 
    /// However you can set Active to false if you dont want to set job at the moment of creating. 
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Description is used to hold extra information about the dataset. The Description will be visible for citizens in the OpenData portal
    /// </summary>
    public string Description { get; set; }

    public virtual IValidator GetValidator() => new CreateDatasetRequestValidator();
}

public class CreateDatasetRequestValidator : AbstractValidator<CreateDatasetRequest>
{
    public CreateDatasetRequestValidator()
    {
        RuleFor(r => r.DatasetName).NotEmpty();

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

        RuleFor(r => r.Description).MaximumLength(5000);
    }
}
