using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.TimeStamp
{
    public class SignServerProviderSettings
    {
        public string BaseUrl {  get; set; }
        public string RequestTokenUrl { get; set; }
        public string CertificatePath { get; set; }
        public string? CertificatePass { get; set; }

        public void ValidateAndThrow()
        {

            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new ArgumentNullException(nameof(BaseUrl));

            if (string.IsNullOrWhiteSpace(CertificatePath))
                throw new ArgumentNullException(nameof(CertificatePath));

            if (string.IsNullOrWhiteSpace(RequestTokenUrl))
                throw new ArgumentNullException(nameof(RequestTokenUrl));
        }
    }
}
