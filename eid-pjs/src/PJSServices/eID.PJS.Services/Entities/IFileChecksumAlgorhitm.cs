using eID.PJS.AuditLogging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services
{
    public interface IFileChecksumAlgorhitm
    {
        string Calculate(IEnumerable<AuditLogRecord> events, int bufferLength);
        string Calculate(IEnumerable<AuditLogRecord> events);
        ICryptoKeyProvider CryptoKeyProvider { get; }
    }

    public class DummySignAlgorhitm : IFileChecksumAlgorhitm
    {
        private ICryptoKeyProvider _cryptoKeyProvider;
        public DummySignAlgorhitm(ICryptoKeyProvider keyProvider)
        {
            _cryptoKeyProvider = keyProvider;
        }

        public ICryptoKeyProvider CryptoKeyProvider => _cryptoKeyProvider;

        public string Calculate(IEnumerable<AuditLogRecord> events)
        {
            return Calculate(events, 0);
        }

        public string Calculate(IEnumerable<AuditLogRecord> events, int bufferLength)
        {
            return string.Empty;
        }
    }
}
