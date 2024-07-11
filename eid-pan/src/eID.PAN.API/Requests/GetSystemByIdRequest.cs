using FluentValidation;

namespace eID.PAN.API.Requests
{
    public class GetSystemByIdRequest : IValidatableRequest
    {
        public virtual IValidator GetValidator() => new GetSystemByIdRequestValidator();
        public Guid Id { get; set; }
    }

    public class GetSystemByIdRequestValidator : AbstractValidator<GetSystemByIdRequest>
    {
        public GetSystemByIdRequestValidator() 
        {
            RuleFor(r => r.Id)
                .NotEmpty();
        }
    }
}
