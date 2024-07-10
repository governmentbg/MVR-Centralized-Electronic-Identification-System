using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLogAbstractions
{
    public interface ICryptoKeyProvider
    {
        public byte[] GetKey();
    }

    public class DummyCryptoKeyProvider : ICryptoKeyProvider
    {
        public byte[] GetKey()
        {
            return Encoding.ASCII.GetBytes("1234567890");
        }
    }

   
}
