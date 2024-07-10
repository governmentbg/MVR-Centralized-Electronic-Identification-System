using System.Security.Cryptography;
using System.Text;

namespace eID.PAN.Service;

public static class AesEncryptDecryptHelper
{
    public static string EncryptPassword(string plainPasswordText, string key, string vector)
    {
        // Check arguments.
        if (string.IsNullOrWhiteSpace(plainPasswordText))
        {
            throw new ArgumentNullException(nameof(plainPasswordText));
        }

        byte[] encryptedBytes;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
            aesAlg.IV = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(vector)));

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainPasswordText);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return Convert.ToBase64String(encryptedBytes);
    }

    public static string DecryptPassword(string encryptedPasswordText, string key, string vector)
    {
        // Check arguments.
        if (string.IsNullOrWhiteSpace(encryptedPasswordText))
        {
            throw new ArgumentNullException(nameof(encryptedPasswordText));
        }

        var cipherText = Convert.FromBase64String(encryptedPasswordText);
        // Declare the string used to hold
        // the decrypted text.
        string plaintext = string.Empty;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(key)));
            aesAlg.IV = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(vector)));

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}
