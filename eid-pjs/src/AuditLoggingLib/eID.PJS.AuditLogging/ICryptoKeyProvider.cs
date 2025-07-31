using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.AuditLogging
{
    public interface ICryptoKeyProvider
    {
        public byte[] GetKey();
    }

    public class DummyCryptoKeyProvider : ICryptoKeyProvider
    {
        private string _password;
        public DummyCryptoKeyProvider()
        {
            _password = "0987654321";
        }

        public DummyCryptoKeyProvider(string password)
        {
            _password = password;
        }


        public byte[] GetKey()
        {
            return Encoding.UTF8.GetBytes(_password);
        }
    }

   
}
