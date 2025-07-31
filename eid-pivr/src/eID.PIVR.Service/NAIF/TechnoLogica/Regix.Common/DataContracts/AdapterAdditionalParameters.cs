using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common.DataContracts
{
    /// <summary>
    /// Adapter additional (context) parameters
    /// </summary>
    [Serializable]
    [DataContract]
    public class AdapterAdditionalParameters
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AdapterAdditionalParameters()
        {
            ProcessingData = new List<ProcessingDataEntry>();
        }

        /// <summary>
        /// Caller context
        /// </summary>
        [DataMember(Name = "_x003C_CallContext_x003E_k__BackingField", Order = 1)]
        public CallContext CallContext { get; set; }

        /// <summary>
        /// Identifier of the citizen targeted by the operation
        /// </summary>
        [DataMember(Name = "_x003C_CitizenEGN_x003E_k__BackingField", Order = 3)]
        public string CitizenEGN { get; set; }

        /// <summary>
        /// IP address of the client
        /// </summary>
        [DataMember(Name = "_x003C_ClientIPAddress_x003E_k__BackingField", Order = 4)]
        public string ClientIPAddress { get; set; }

        /// <summary>
        /// Thumbprint of the consumer's certificate
        /// </summary>
        [DataMember(Name = "_x003C_ConsumerCertificateThumbprint_x003E_k__BackingField", Order = 5)]
        public string ConsumerCertificateThumbprint { get; set; }

        /// <summary>
        /// EID token (when ESB is used for connecting to the RegiX Core)
        /// </summary>
        [DataMember(Name = "_x003C_EIDToken_x003E_k__BackingField", Order = 6)]
        public string EIDToken { get; set; }

        /// <summary>
        /// Identifier of the employee executing the operation
        /// </summary>
        [DataMember(Name = "_x003C_EmployeeEGN_x003E_k__BackingField", Order = 7)]
        public string EmployeeEGN { get; set; }

        /// <summary>
        /// Identifier of the organization executing the opration
        /// </summary>
        [DataMember(Name = "_x003C_OrganizationEIK_x003E_k__BackingField", Order = 8)]
        public string OrganizationEIK { get; set; }

        /// <summary>
        /// Organization unit of the employee executing the opration
        /// </summary>
        [DataMember(Name = "_x003C_OrganizationUnit_x003E_k__BackingField", Order = 9)]
        public string OrganizationUnit { get; set; }

        /// <summary>
        /// If the access matrix should be returned as part of the result
        /// </summary>
        [DataMember(Name = "_x003C_ReturnAccessMatrix_x003E_k__BackingField", Order = 13)]
        public bool ReturnAccessMatrix { get; set; }

        /// <summary>
        /// If the result should be signed
        /// </summary>
        [DataMember(Name = "_x003C_SignResult_x003E_k__BackingField", Order = 14)]
        public bool SignResult { get; set; }


        /// <summary>
        /// ApiServiceCallID as received from the RegiX Core
        /// </summary>
        [DataMember(Name = "_x003C_ApiServiceCallId_x003E_k__BackingField", Order = 0, IsRequired = false)]
        public decimal ApiServiceCallId { get; set; }

        /// <summary>
        /// Callback URL used for calling consumer in asynchronous scenario
        /// </summary>
        [DataMember(Name = "_x003C_CallbackURL_x003E_k__BackingField", Order = 2, IsRequired = false)]
        public string CallbackURL { get; set; }

        /// <summary>
        /// Type of the request processing
        /// </summary>
        [DataMember(Name = "_x003C_RequestProcessing_x003E_k__BackingField", Order = 11, IsRequired = false)]
        public RequestProcessing RequestProcessing { get; set; }

        /// <summary>
        /// Type of the response processing
        /// </summary>
        [DataMember(Name = "_x003C_ResponseProcessing_x003E_k__BackingField", Order = 12, IsRequired = false)]
        public ResponseProcessing ResponseProcessing { get; set; }

        /// <summary>
        /// Additional data used during the execution of the operation
        /// </summary>
        [DataMember(Name = "_x003C_ProcessingData_x003E_k__BackingField", Order = 10, IsRequired = false)]
        public List<ProcessingDataEntry> ProcessingData { get; set; }
    }

    /// <summary>
    /// Entry in the additional processing data
    /// </summary>
    public class ProcessingDataEntry
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public XmlElement Value { get; set; }
    }
}
