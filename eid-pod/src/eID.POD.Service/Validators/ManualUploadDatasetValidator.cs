using eID.POD.Contracts.Commands;
using FluentValidation;

namespace eID.POD.Service.Validators;

public class ManualUploadDatasetValidator : AbstractValidator<ManualUploadDataset>
{
    public ManualUploadDatasetValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}
