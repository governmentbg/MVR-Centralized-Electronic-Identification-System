using System.Net.Mime;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using FluentValidation;
using UglyToad.PdfPig;
using UglyToad.PdfPig.AcroForms;
using UglyToad.PdfPig.Tokens;

namespace eID.PDEAU.API.Public.Requests;

public class RegisterProviderRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegisterProviderRequestValidator();
    public ProviderType Type { get; set; }
    public string Name { get; set; }
    public string Bulstat { get; set; }
    public string Headquarters { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public RegisterProviderUserDTO Administrator { get; set; }
}
public class RegisterProviderUserDTO : RegisterProviderUser
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class RegisterProviderRequestValidator : AbstractValidator<RegisterProviderRequest>
{
    public RegisterProviderRequestValidator()
    {
        RuleFor(r => r.Type).IsInEnum().NotEmpty();
        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);

        RuleFor(r => r.Bulstat)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EikFormatIsValid(uid))
                .WithMessage("{PropertyName} invalid Eik.");

        RuleFor(r => r.Headquarters).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Address).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(r => r.Phone).NotEmpty().MaximumLength(200);

        RuleFor(r => r.Administrator).NotEmpty().SetValidator(new RegisterProviderUserValidator());
    }
}

public class RegisterProviderUserValidator : AbstractValidator<RegisterProviderUser>
{
    public RegisterProviderUserValidator()
    {
        RuleFor(r => r.Uid)
            .NotEmpty()
            .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid EGN.")
            .Must(uid => ValidatorHelpers.IsLawfulAge(uid)).When(r => r.UidType == IdentifierType.EGN, ApplyConditionTo.CurrentValidator)
            .WithMessage("{PropertyName} is below lawful age.")
            .Must(uid => ValidatorHelpers.LnchFormatIsValid(uid)).When(r => r.UidType == IdentifierType.LNCh, ApplyConditionTo.CurrentValidator)
                .WithMessage("{PropertyName} invalid LNCh.");

        RuleFor(r => r.UidType).NotEmpty().IsInEnum();

        RuleFor(r => r.Name).NotEmpty().MaximumLength(200);
        RuleFor(r => r.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(r => r.Phone).NotEmpty().MaximumLength(200);
    }
}

public class RegisterProviderFormFilesValidator : AbstractValidator<IEnumerable<IFormFile>>
{
    public RegisterProviderFormFilesValidator()
    {
        RuleFor(r => r).NotEmpty().WithName("Files").WithMessage("{PropertyName} must not be empty.");
        RuleForEach(files => files)
            .SetValidator(new FormFileValidator());
    }
}
public class FormFileValidator : AbstractValidator<IFormFile>
{
    public FormFileValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(f => f)
            .NotNull()
            .Must(f => f.ContentType == MediaTypeNames.Application.Pdf)
                .WithMessage("Invalid content type.")
            .Must(f => f.Length <= 5 * 1024 * 1024)
                .WithMessage("File too big. Maximum allowed size is 5Mb.")
            .Custom((f, context) =>
            {
                MemoryStream fileStream = new MemoryStream();
                f.CopyTo(fileStream);
                try
                {
                    using var document = PdfDocument.Open(fileStream.ToArray());
                }
                catch (UglyToad.PdfPig.Exceptions.PdfDocumentEncryptedException)
                {
                    context.AddFailure($"Document {f.FileName} is encrypted.");
                }
                catch (UglyToad.PdfPig.Core.PdfDocumentFormatException)
                {
                    context.AddFailure($"Document {f.FileName} is corrupted.");
                }
                catch (Exception)
                {
                    context.AddFailure($"Unable to read the file {f.FileName}.");
                }
                finally
                {
                    fileStream.Dispose();
                }
            })
            .Must(f =>
            {
                MemoryStream fileStream = new MemoryStream();
                f.CopyTo(fileStream);
                using var document = PdfDocument.Open(fileStream.ToArray());
                fileStream.Dispose();
                document.TryGetForm(out AcroForm acroForm);

                // Check if the PDF has an AcroForm and fields are present
                return acroForm != null && acroForm.Fields.Count > 0 && acroForm.Fields.Any(field => field.Dictionary.TryGet(NameToken.Ft, out var fieldType) &&
                            fieldType is NameToken nameToken &&
                            nameToken.Data.Equals("Sig", StringComparison.OrdinalIgnoreCase));
            })
                .WithMessage(f => string.Format("File {0} is not signed.", f.FileName));
    }
}
