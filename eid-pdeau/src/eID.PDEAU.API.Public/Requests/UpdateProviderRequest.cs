using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;

namespace eID.PDEAU.API.Public.Requests;

internal class UpdateProviderRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new UpdateProviderRequestValidator();
    public Guid ProviderId { get; set; }
    /// <summary>
    /// Update comment, what was done.
    /// </summary>
    public string Comment { get; set; }
}

internal class UpdateProviderUserDTO : UpdateProviderUser
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

internal class UpdateProviderAISInformationDTO : UpdateProviderAISInformation
{
    public string Name { get; set; }
    public string Project { get; set; }
    public string SourceIp { get; set; }
    public string DestinationIp { get; set; }
    public DestinationIpType DestinationIpType { get; set; }
    public string ProtocolPort { get; set; }
}

internal class UpdateProviderRequestValidator : AbstractValidator<UpdateProviderRequest>
{
    public UpdateProviderRequestValidator()
    {
        RuleFor(r => r.Comment)
            .NotEmpty()
            .MaximumLength(Constants.ProviderStatusHistory.CommentMaxLength);
    }
}

internal class UpdateProviderFormFilesValidator : AbstractValidator<IEnumerable<IFormFile>>
{
    /// <summary>
    /// Check if the files have unique names
    /// </summary>
    /// <param name="existingFileNameList">Stored files list</param>
    public UpdateProviderFormFilesValidator(IEnumerable<string> existingFileNameList)
    {
        RuleFor(r => r).NotEmpty().WithName("Files").WithMessage("{PropertyName} must not be empty.");
        RuleForEach(files => files)
            .SetValidator(new FormFileValidator());

        RuleFor(x => x.Select(ff => Path.GetFileName(ff.FileName)))
            .Custom((names, context) =>
            {
                var duplicates = names.Intersect(existingFileNameList).ToList();

                if (duplicates.Any())
                {
                    context.AddFailure("Files", $"Following files are duplicated: {string.Join(", ", duplicates)}");
                }
            });
    }
}
