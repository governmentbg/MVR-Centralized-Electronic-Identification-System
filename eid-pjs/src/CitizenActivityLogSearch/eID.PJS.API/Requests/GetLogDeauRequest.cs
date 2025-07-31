using FluentValidation;

namespace eID.PJS.API.Requests;

public class GetLogDeauRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new GetLogDaeuRequestValidator();
    /// <summary>
    /// Size of cursor return data
    /// </summary>
    public int CursorSize { get; set; }
    /// <summary>
    /// The parameter provides a live cursor that uses the previous page’s results to obtain the next page’s results
    /// In the first request it will be <see cref="null"/>
    /// </summary>
    public IEnumerable<object> CursorSearchAfter { get; set; }
}

public class GetLogDaeuRequestValidator : AbstractValidator<GetLogDeauRequest>
{
    public GetLogDaeuRequestValidator()
    {
        RuleFor(r => r.CursorSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}
