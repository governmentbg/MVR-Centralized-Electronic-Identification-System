using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Common.DataContracts
{
    /// <summary>
    /// Data container (not typed)
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    [XmlRoot(ElementName = "Data", Namespace = "http://egov.bg/RegiX/SignedData")]
    public class DataContainer
    {
        /// <summary>
        /// Data identifier attribute
        /// </summary>
        [XmlAttributeAttribute("id", DataType = "ID")]
        public string DataID = "data";

        /// <summary>
        /// Contains the request argument of the operation. It contains the data from
        /// the Argument property of the ServiceRequest object
        /// </summary>
        [XmlElement]
        public RequestContainer Request { get; set; }

        /// <summary>
        /// Contains the response accoring to the request argument and access matrix
        /// Additional information for the structure of the response see: http://regixaisweb.egov.bg/RegiXInfo
        /// </summary>
        [XmlElement]
        public ResponseContainer Response { get; set; }

        /// <summary>
        /// Access matrix. Describes the accessible properties of the response (depenes of the caller privileges)
        /// </summary>
        [XmlElement(IsNullable = true)]
        public DataAccessMatrix Matrix { get; set; }
    }

    /// <summary>
    /// Typed data container. DataContract namespace and DataMember names added for compatibility with RegiX II adapters.
    /// </summary>
    /// <typeparam name="A">Request typed</typeparam>
    /// <typeparam name="R">Response Type</typeparam>
    [Serializable]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TechnoLogica.RegiX.Common.TransportObject")]
    public class DataContainer<A, R>
    {
        /// <summary>
        /// Data identifier attribute
        /// </summary>
        [XmlAttribute("id")]
        [DataMember]
        public string DataID = "data";

        /// <summary>
        /// Request
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_x003C_Request_x003E_k__BackingField")]
        public A Request { get; set; }

        /// <summary>
        /// Response
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_x003C_Response_x003E_k__BackingField")]
        public R Response { get; set; }

        /// <summary>
        /// Data access matrix
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_x003C_Matrix_x003E_k__BackingField")]
        public DataAccessMatrix Matrix { get; set; }
    }
}
