using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using eID.PJS.Services.TimeStamp;
using System.Runtime.Intrinsics.X86;
using Steeltoe.Extensions.Configuration;
using System.Configuration;
using eID.PJS.Services.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace eID.PJS.Services.Tests
{
    public class TimeStampServerTests: ServiceTestBase
    {
        private SignServerProviderSettings _ssSettings;
        private IServiceProvider _serviceProvider;
        private X509Certificate2 _certificate;
        public TimeStampServerTests()
        {
            _services.AddSingleton(svc =>
            {
                if (_config == null) throw new ArgumentNullException(nameof(SignServerProviderSettings));

                return _config.GetSection(nameof(SignServerProviderSettings)).Get<SignServerProviderSettings>();
            });

            _services.AddScoped<ITimeStampProvider, SignServerRESTProvider>();
            _services.RegisterHttpClientWithPollyAndCert(_config);

            _serviceProvider = _services.BuildServiceProvider();

            _ssSettings = _serviceProvider.GetRequiredService<SignServerProviderSettings>();

            var bytes = File.ReadAllBytes(Path.GetFullPath(_ssSettings.CertificatePath));
            _certificate = new X509Certificate2(bytes, _ssSettings.CertificatePass);
            

            //var bytes = File.ReadAllBytes(Path.GetFullPath(sss.CertificatePath));
            //var certificate = new X509Certificate2(bytes);

            //var httpFactory = IHttpClientFactory

            //_ssSettings = new SignServerProviderSettings
            //{
            //    BaseUrl = "https://signserver.mvreid.local",
            //    RequestTokenUrl = "/signserver/rest/v1/workers/TimeMonitor/process",
            //    CertificatePath = "Cert\\timestamp-cert-chain.crt"
            //};

        }

        //[Fact]
        public void CreateRequestTest()
        {
            byte[] data = File.ReadAllBytes("cert\\test.data");

            var ext = new X509ExtensionCollection();

            var request = Rfc3161TimestampRequest.CreateFromData(data, HashAlgorithmName.SHA256, requestSignerCertificates: true);

            var result = new byte[100_000];
            int bytesWritten = 0;

            var hash = request.GetMessageHash();

            if (request.TryEncode(result, out bytesWritten))
            {
                var buffer = new Span<byte>(result, 0, bytesWritten);
                File.WriteAllBytes("cert\\test.req", buffer.ToArray());
            }
            else
            {

            }
        }

      
        //[Fact]
        public async Task SignServerRESTProvider_RequestToken_Test()
        {
            var mgr = _serviceProvider.GetRequiredService<ITimeStampProvider>();

            var result = await mgr.RequestToken("ghXhGcFuBKh1fR/NdWdg9oPK5txvkpeKVDXFpNAIlwgCXRg4Zupz77AT8HbIRtSvwmRtGbUFhFW25Dx9DIm4LA==");

            File.WriteAllText("cert\\test-rest.tsrx", result.Response);

        }

        //[Fact]
        public async Task SignServerRESTProvider_ValidateToken_Test()
        {
            var hash = "ghXhGcFuBKh1fR/NdWdg9oPK5txvkpeKVDXFpNAIlwgCXRg4Zupz77AT8HbIRtSvwmRtGbUFhFW25Dx9DIm4LA==";

            var tokenData = File.ReadAllText("cert\\test-rest.tsrx");

            var mgr = _serviceProvider.GetRequiredService<ITimeStampProvider>();

            var result = mgr.VerifyToken(hash, tokenData, _certificate);

        }

    }
}
