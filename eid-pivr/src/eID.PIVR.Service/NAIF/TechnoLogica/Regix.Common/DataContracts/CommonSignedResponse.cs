using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.TransportObjects;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Common.DataContracts
{
    /// <summary>
    /// Common signed response (not type defined)
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    [XmlRoot(ElementName = "SignedData")]
    public class CommonSignedResponse
    {
        /// <summary>
        /// Data container
        /// </summary>
        [XmlElement]
        public DataContainer Data { get; set; }

    }

    /// <summary>
    /// Typed common sigend response. DataContract namespace and DataMember names added for compatibility with RegiX II adapters.
    /// </summary>
    /// <typeparam name="A">Type of the request</typeparam>
    /// <typeparam name="R">Type of the response</typeparam>
    [Serializable]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    [XmlRoot(ElementName = "CommonSignedResponse")]    
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/TechnoLogica.RegiX.Common.TransportObject")]
    public class CommonSignedResponse<A, R> : IProvideServiceResultData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CommonSignedResponse()
        {
            IsReady = true;
        }

        /// <summary>
        /// Typed data container
        /// </summary>
        [XmlElement(ElementName = "DataContainer")]
        [DataMember( Name = "_x003C_Data_x003E_k__BackingField", Order = 0)]
        public DataContainer<A, R> Data { get; set; }

        /// <summary>
        /// Signature
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_x003C_Signature_x003E_k__BackingField", Order = 1)]
        public XmlElement Signature { get; set; }

        /// <summary>
        /// If the result is ready
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_x003C_IsReady_x003E_k__BackingField", Order = 2, IsRequired =false)]
        public bool IsReady { get; set; }

        /// <summary>
        /// Verification code used in asynchronouse scenario
        /// </summary>
        [XmlElement]
        [DataMember(Name = "_x003C_VerificationCode_x003E_k__BackingField", Order = 3, IsRequired = false)]
        public string VerificationCode { get; set; }

        /// <summary>
        /// XML serialization of the object
        /// </summary>
        /// <returns>Serialized object</returns>
        public string XmlSerialize()
        {
            var resultDocument = SigningUtils.GetCommonSignedResponseWithSignature(Data.Request.XmlSerialize(), Data.Response.XmlSerialize(), Data.Matrix, Signature);
            return resultDocument.OuterXml;
        }

        /// <summary>
        /// Converts the object to not typed one
        /// </summary>
        /// <returns>Not typed service result data</returns>
        public virtual ServiceResultData ToServiceResultDataSigned()
        {
            ServiceResultData result = new ServiceResultData();
            // Data Response is checked for RegiX II cmpatibility (IsReady is false for those adapters).
            result.IsReady = IsReady || (Data != null && Data.Response != null);
            if (result.IsReady)
            {
                result.Data = SigningUtils.GetDataContainer(Data.Request.XmlSerialize().ToXmlElement(), Data.Response.XmlSerialize().ToXmlElement(), Data.Matrix);
                result.Signature = Signature;
            }
            result.VerificationCode = VerificationCode;
            result.HasError = false;
            return result;
        }
    }
}
