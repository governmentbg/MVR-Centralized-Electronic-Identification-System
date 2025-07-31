using FluentValidation;

namespace eID.PIVR.API.Requests
{
        public class RelationsSearchRequest : IValidatableRequest
        {
            public virtual IValidator GetValidator() => new RelationsSearchRequestValidator();

            public string Identifier { get; set; } = string.Empty;
        }

        internal class RelationsSearchRequestValidator : AbstractValidator<RelationsSearchRequest>
        {
            public RelationsSearchRequestValidator()
            {
            RuleFor(r => r.Identifier)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(uid => ValidatorHelpers.EgnFormatIsValid(uid))
                    .WithMessage("{PropertyName} is invalid EGN.");
            }
        }
    }
