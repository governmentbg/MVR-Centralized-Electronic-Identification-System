using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.TimeStamp;

namespace eID.PJS.Services.Entities
{
    public static class HttpFactoryDiExtensions
    {
        /// <summary>
        /// Registers the HTTP client with polly and cert.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void RegisterHttpClientWithPollyAndCert(this IServiceCollection services, IConfiguration configuration)
        {
            services
               .AddHttpClient(HttpClientNames.SignServerHttpClient, client =>
               {

               })
               .ConfigurePrimaryHttpMessageHandler((s) =>
               {
#if VERBOSE
                   Console.WriteLine("VERBOSE: Configuring Certificates");
#endif
                   var sss = s.GetRequiredService<SignServerProviderSettings>();
                   sss.ValidateAndThrow();

                   var bytes = File.ReadAllBytes(Path.GetFullPath(sss.CertificatePath));
                   var certificate = new X509Certificate2(bytes, sss.CertificatePass);

                   return new HttpClientHandler
                   {
                       ClientCertificates = { certificate },
                   };
               })
               .AddPolicyHandler((serviceProvider, request) =>
               {
#if VERBOSE
                   Console.WriteLine("VERBOSE: Configuring RetryPolicy");
#endif
                   var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                   return ApplicationPolicyRegistry.GetRetryPolicy(logger);
               });
        }

        /// <summary>
        /// Registers the time stamp server certificate.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">SignServerProviderSettings is not configured</exception>
        public static void RegisterTimeStampServerCertificate(this IServiceCollection services, IConfiguration configuration)
        {
            var _ssSettings = configuration.GetSection(nameof(SignServerProviderSettings)).Get<SignServerProviderSettings>();

            if (_ssSettings == null)
                throw new ConfigurationErrorsException("SignServerProviderSettings is not configured");

                if (_ssSettings != null)
                _ssSettings.ValidateAndThrow();

            var bytes = File.ReadAllBytes(Path.GetFullPath(_ssSettings.CertificatePath));
            var _certificate = new X509Certificate2(bytes, _ssSettings.CertificatePass);

            services.AddSingleton(_certificate);
        }

    }
}
