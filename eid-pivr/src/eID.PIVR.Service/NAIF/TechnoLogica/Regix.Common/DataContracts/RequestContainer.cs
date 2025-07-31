using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.DataContracts
{
    /// <summary>
    /// Request container
    /// </summary>
    [Serializable]
    [XmlType]
    public class RequestContainer
    {
        /// <summary>
        /// Identifying attribute
        /// </summary>
        [XmlAttributeAttribute("id", DataType = "ID")]
        public string DataID = "request";

        /// <summary>
        /// Request's xml
        /// </summary>
        [XmlAnyElement]
        public XmlElement Request { get; set; }

    }
}
