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
    /// Response container
    /// </summary>
    [Serializable]
    [XmlType]
    public class ResponseContainer
    {
        /// <summary>
        /// Identifying attribute
        /// </summary>
        [XmlAttribute("id", DataType = "ID")]
        public string DataID = "response";

        /// <summary>
        /// Response's XML
        /// </summary>
        [XmlAnyElement]
        public XmlElement Response { get; set; }
    }
}
