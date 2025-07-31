using System.Security.Cryptography;
using System.Text;

namespace eID.PJS.Service;

public static class EncryptionHelper
{
    public static string Key { get; private set; }

    public static string Encrypt(string dataToEncrypt, bool allowEmptyStrings = false)
    {
        if (Key is null)
        {
            throw new ArgumentNullException("Encryption key");
        }

        if (!allowEmptyStrings && string.IsNullOrWhiteSpace(dataToEncrypt))
        {
            return null;
        }

        byte[] encrypted;

        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(Key);
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(dataToEncrypt);
        }
        encrypted = msEncrypt.ToArray();

        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string dataToDecrypt, Aes aes, ICryptoTransform descriptor)
    {
        if (Key is null)
        {
            throw new ArgumentNullException("Encryption key");
        }

        if (string.IsNullOrEmpty(dataToDecrypt) || string.IsNullOrWhiteSpace(dataToDecrypt))
        {
            return null;
        }

        if (aes is null)
        {
            throw new ArgumentNullException(nameof(aes));
        }

        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        var buffer = Convert.FromBase64String(dataToDecrypt);
        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, descriptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }

    public static void SetEncryptionKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
        }

        Key = key;
    }
}
