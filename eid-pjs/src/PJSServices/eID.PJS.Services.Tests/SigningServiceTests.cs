using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Serilog.Core;
using System.Configuration;

using eID.PJS.AuditLogging;

using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using eID.PJS.Services.Signing;

#nullable disable

namespace eID.PJS.Services.Tests
{
    public class SigningServiceTests: ServiceTestBase
    {
        private ICryptoKeyProvider _configCryptoKeyProvider;
        private HMACSignAlgorhitm _dummySignAlgorhitm;
        private ILogger<SigningService> _logger;
        private SigningServiceSettings _settings;

        public SigningServiceTests() : base()
        {
            _configCryptoKeyProvider = new ConfigurationCryptoKeyProvider(_config);
            _dummySignAlgorhitm = new HMACSignAlgorhitm(_configCryptoKeyProvider);
            _logger = new NullLogger<SigningService>();
            _settings = _config.GetSection(nameof(SigningServiceSettings)).Get<SigningServiceSettings>();
        }

        //[Fact]
        public void SignTest()
        {
            //var svc = new SigningService(_logger, _settings, _dummySignAlgorhitm);

            //svc.Process();

            //Assert.True(true);
        }

        //[Fact]
        public void DeserializeLogTest()
        {


            //string path = "..\\..\\..\\..\\samples\\eid-iscei_27b4ffab-4403-48ec-928c-fbaf8da0f997_2023080111.audit";

            //var firstLine =  File.ReadLines(path).Skip(1).Take(1).FirstOrDefault();

            //var obj = JsonConvert.DeserializeObject(firstLine, typeof(AuditLogRecord), new JsonSerializerSettings { 
            //    //TypeNameHandling = TypeNameHandling.Auto,
            //}) as AuditLogRecord;

            //var fileChecksum = obj.Checksum;
            //Debug.WriteLine(obj.Checksum);
            //obj.Checksum = null; //To calculate checksum again we need to have the empty checksum property. 
            //var checksum = CalculateChecksum(obj, new DummyCryptoKeyProvider().GetKey());
            //Debug.WriteLine(checksum);

            //Assert.True(fileChecksum.Equals(checksum, StringComparison.InvariantCulture));
        }

        private string CalculateChecksum(AuditLogRecord record, byte[] encryptionKey)
        {
            using (var hash = new HMACSHA512(encryptionKey))
            {
                var hashString = JsonConvert.SerializeObject(record, Formatting.None);

                var data = Encoding.UTF8.GetBytes(hashString);
                var checksum = hash.ComputeHash(data);
                return Convert.ToBase64String(checksum);
            }
        }
    }
}