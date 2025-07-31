using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Container object used for the transport of encrypted data
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://egov.bg/RegiX/EncryptedArgument")]
    [XmlRoot("EncryptedArgument", Namespace = "http://egov.bg/RegiX/EncryptedArgument", IsNullable = false)]
    public class EncryptedArgument
    {
        /// <summary>
        /// Encrypted symetric key
        /// </summary>
        [XmlElement]
        public byte[] EncryptedKey { get; set; }

        /// <summary>
        /// Encrypted data
        /// </summary>
        [XmlElement]
        public byte[] EncryptedData { get; set; }
    }
}
