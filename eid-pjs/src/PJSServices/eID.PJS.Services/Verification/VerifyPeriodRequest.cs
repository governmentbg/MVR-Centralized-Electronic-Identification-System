using FluentValidation;

using Newtonsoft.Json;

namespace eID.PJS.Services.Verification
{
    public class VerifyPeriodRequest: IValidatableRequest
    {
        [JsonProperty("systemId")]
        public string? SystemId {  get; set; }
        
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
        
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        public virtual IValidator GetValidator() => new VerifyPeriodRequestValidator();
    }


    public class VerifyPeriodRequestValidator: AbstractValidator<VerifyPeriodRequest>
    { 
        public VerifyPeriodRequestValidator() 
        {
            RuleFor(f => f.StartDate).NotEmpty().When(m => m.EndDate == null);
            RuleFor(f => f.EndDate).NotEmpty().When(m => m.StartDate == null);

            RuleFor(f => f.StartDate).NotEmpty().When(m => m.EndDate == DateTime.MinValue);
            RuleFor(f => f.EndDate).NotEmpty().When(m => m.StartDate == DateTime.MinValue);

        }
    }
}