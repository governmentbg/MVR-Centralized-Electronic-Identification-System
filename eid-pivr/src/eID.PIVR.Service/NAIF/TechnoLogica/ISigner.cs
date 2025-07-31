
using System.Xml;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Defines method to digitally sign objects and PDFs. Additionaly provides security related 
    /// methods for encrypting/decrypting data
    /// </summary>
    public interface ISigner
    {
        /// <summary>
        /// Digitally signs a CommonSignedResponse object. The signature is stored in the Signature
        /// property of the CommonSignedResponse argument.
        /// </summary>
        /// <typeparam name="A">Request type</typeparam>
        /// <typeparam name="R">Response Type</typeparam>
        /// <param name="arg">A CommonSignedResponse object</param>
        void SignXmlDocumentWithCertificate<A, R>(CommonSignedResponse<A, R> arg);

        /// <summary>
        ///  Digitally signs a ServiceResultData object. The signature is stored in the Signature element. If multiple signatures are present the Signature element contains a sequence of signatures. If the response is PDF - the PDF itself is signed (the siganture is embeded in the PDF).
        /// </summary>
        /// <param name="serviceResultData">Object to be signed</param>
        void Sign(ServiceResultData serviceResultData);

        /// <summary>
        /// Digitally signs a PDF document
        /// </summary>
        /// <param name="pdf">PDF document to be ditially signed</param>
        /// <returns>Digitally signed PDF document</returns>
        byte[] SignPDF(byte[] pdf);

        /// <summary>
        /// Validates the digital signature of an XmlElement
        /// </summary>
        /// <param name="doc">Digitally sigend XmlElement</param>
        /// <returns>If the signature of the XmlElement is valid or not</returns>
        bool Validate(XmlElement doc);

        /// <summary>
        /// Validates the digital signature of an XmlDocument
        /// </summary>
        /// <param name="doc">Digitally sigend XmlDocument</param>
        /// <returns>If the signature of the XmlDocument is valid or not</returns>
        bool Validate(XmlDocument doc);

        /// <summary>
        /// Encrypts the provided data with the provided key
        /// </summary>
        /// <param name="encodedKey">Key</param>
        /// <param name="data">Data to be encrypted</param>
        /// <returns>Encrypted data</returns>
        byte[] Encrypt(byte[] encodedKey, byte[] data);

        /// <summary>
        /// Decrypts the provided cipher data with the provided key
        /// </summary>
        /// <param name="encodedKey">Key</param>
        /// <param name="encryptedData">Data to be decrypted</param>
        /// <returns>Decrypted data</returns>
        byte[] Decrypt(byte[] encodedKey, byte[] encryptedData);

        /// <summary>
        /// Generates new symetric key
        /// </summary>
        /// <returns>Generated symetric key</returns>
        byte[] GenerateSymeticKey();

        /// <summary>
        /// Encrypts a symetric key
        /// </summary>
        /// <param name="key">Symetric key to be encrypted</param>
        /// <returns>Encrypted symetric key</returns>
        byte[] EncryptKey(byte[] key);

        /// <summary>
        /// Decrypts a symetric key
        /// </summary>
        /// <param name="encryptedKey">Ciphered key to be decrypted</param>
        /// <returns>Decrypted symetric key</returns>
        byte[] DecryptKey(byte[] encryptedKey);

        /// <summary>
        /// Retrieve the public key of the signer
        /// </summary>
        /// <returns>Retrieved public key</returns>
        string GetPublicKeyString();
    }
}
