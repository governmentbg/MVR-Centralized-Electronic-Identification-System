using FluentValidation;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.Services.Verification
{
    public class VerifyFileRequest: IValidatableRequest
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        public IValidator GetValidator() => new VerifyFileRequestValidator();
    }

    public class VerifyFileRequestValidator: AbstractValidator<VerifyFileRequest>
    {
        public VerifyFileRequestValidator() 
        {
            RuleFor(f => f.FileName).NotEmpty();
        }
    }
}
