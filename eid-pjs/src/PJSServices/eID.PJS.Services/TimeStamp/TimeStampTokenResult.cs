using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.TimeStamp
{
    public class TimeStampTokenResult
    {
        public string Response {  get; set; }
        public string Request { get; set; }
        public Rfc3161TimestampToken TimestampToken { get; set; }
        public Exception? Error {  get; set; }
    }
}
