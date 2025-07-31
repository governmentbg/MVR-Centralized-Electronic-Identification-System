using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.AuditLogging
{
    public class ConfigurationCryptoKeyProvider : ICryptoKeyProvider
    {
        public ConfigurationCryptoKeyProvider(IConfiguration config) 
        {
            if(config == null) throw new ArgumentNullException("config");

            EncryptionKeyString = config.GetSection(nameof(ConfigurationCryptoKeyProvider)).GetValue<string>("EncryptionKey");

            if (string.IsNullOrWhiteSpace(EncryptionKeyString))
                throw new ArgumentNullException($"EncryptionKey must be set in configuration section '{nameof(ConfigurationCryptoKeyProvider)}'");
        }

        public string EncryptionKeyString { get; set; }

        public byte[] GetKey()
        {
            return Encoding.ASCII.GetBytes(EncryptionKeyString);
        }

    }
}
