using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common.DataContracts
{
    /// <summary>
    /// Wrapper of a request. Used in the .net core platform scenario
    /// </summary>
    [XmlRoot("RequestWrapper", Namespace = "http://egov.bg/RegiX/DataContracts/RequestWrapper")]
    [XmlType(Namespace = "http://egov.bg/RegiX/DataContracts/RequestWrapper")]
    public class RequestWrapper
    {
        /// <summary>
        /// Service request data
        /// </summary>
        [XmlElement]
        public ServiceRequestData Request { get; set; }

        /// <summary>
        /// Access matrix
        /// </summary>
        [XmlElement]
        public AccessMatrix AccessMatrix { get; set; }

        /// <summary>
        /// Additional parameters
        /// </summary>
        [XmlElement]
        public XmlElement AdditionalParameters { get; set; }
    }
}
