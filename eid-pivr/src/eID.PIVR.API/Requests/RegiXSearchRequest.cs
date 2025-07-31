using FluentValidation;
using Newtonsoft.Json;

namespace eID.PIVR.API.Requests;

public class RegiXSearchRequest : IValidatableRequest
{
    public virtual IValidator GetValidator() => new RegiXSearchRequestValidator();
    [JsonProperty(PropertyName = "operation")]
    public string Operation { get; set; } = string.Empty;
    [JsonProperty(PropertyName = "argument")]
    public RegiXArgument Argument { get; set; } = new RegiXArgument();
}

public class RegiXSearchRequestValidator : AbstractValidator<RegiXSearchRequest>
{
    public RegiXSearchRequestValidator()
    {
        RuleFor(r => r.Operation).NotEmpty();
        RuleFor(r => r.Argument).SetValidator(new RegiXArgumentValidator());
    }
}

public class RegiXArgumentValidator : AbstractValidator<RegiXArgument>
{
    public RegiXArgumentValidator()
    {
        RuleFor(r => r.Type).NotEmpty();
        RuleFor(r => r.Xmlns).NotEmpty();
        RuleFor(r => r.Parameters).NotEmpty();
    }
}

public class RegiXArgument
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; } = string.Empty;
    [JsonProperty(PropertyName = "xmlns")]
    public string Xmlns { get; set; } = string.Empty;
    [JsonProperty(PropertyName = "parameters")]
    public List<Dictionary<string, RegiXArgumentParameter>> Parameters { get; set; } = new List<Dictionary<string, RegiXArgumentParameter>>();
}

public class RegiXArgumentParameter
{
    [JsonProperty(PropertyName = "parameterType")]
    public string ParameterType { get; set; } = string.Empty;
    [JsonProperty(PropertyName = "parameterStringValue")]
    public string? ParameterStringValue { get; set; }
    [JsonProperty(PropertyName = "parameterDateValue")]
    public DateTime? ParameterDateValue { get; set; }
    [JsonProperty(PropertyName = "parameterNumberValue")]
    public long? ParameterNumberValue { get; set; }
    [JsonProperty(PropertyName = "precision")]
    public int? Precision { get; set; }
    [JsonProperty(PropertyName = "parameters")]
    public List<Dictionary<string, RegiXArgumentParameter>>? Parameters { get; set; }
    [JsonProperty(PropertyName = "attributes")]
    public List<Dictionary<string, string>>? Attributes { get; set; }
}
