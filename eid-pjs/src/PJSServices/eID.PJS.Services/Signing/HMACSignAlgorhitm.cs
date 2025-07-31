using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.AuditLogging;


namespace eID.PJS.Services.Signing
{
    public class HMACSignAlgorhitm : IFileChecksumAlgorhitm
    {
        private readonly ICryptoKeyProvider _cryptoKeyProvider;
        private readonly byte[] _key;
        public HMACSignAlgorhitm(ICryptoKeyProvider keyProvider)
        {
            _cryptoKeyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));

            _key = keyProvider.GetKey();

            if (_key.Length == 0)
                throw new ArgumentException($"Empty crypto key returned from the {keyProvider.GetType().FullName}");
        }

        public ICryptoKeyProvider CryptoKeyProvider => _cryptoKeyProvider;

        public string Calculate(IEnumerable<AuditLogRecord> events, int bufferLength)
        {
            var writer = new ArrayBufferWriter<byte>(bufferLength);

            foreach (var e in events)
            {
                var buffer = Encoding.UTF8.GetBytes(e.Checksum);
                writer.Write(buffer);
            }

            using (var hash = new HMACSHA512(_key))
            {
                var checksum = hash.ComputeHash(writer.WrittenMemory.ToArray());
                return Convert.ToBase64String(checksum);
            }
        }

        public string Calculate(IEnumerable<AuditLogRecord> events)
        {
            return Calculate(events, 88);
        }
    }
}
