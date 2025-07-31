using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Container object used for the transport of binary data
    /// </summary>
    [Serializable()]
    [XmlType(Namespace = "http://egov.bg/RegiX/BinaryArgument")]
    [XmlRoot("BinaryArgument", Namespace = "http://egov.bg/RegiX/BinaryArgument", IsNullable = false)]
    public class BinaryArgument
    {
        /// <summary>
        /// Binary data
        /// </summary>
        [XmlElement]
        public byte[] Binary { get; set; }
    }
}
