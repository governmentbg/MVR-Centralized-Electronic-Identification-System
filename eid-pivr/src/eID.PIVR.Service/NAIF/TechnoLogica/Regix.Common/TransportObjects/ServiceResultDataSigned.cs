using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Generic object returned as result of operation's execution
    /// </summary>
    /// <typeparam name="R">Resquest type</typeparam>
    /// <typeparam name="T">Response type</typeparam>
    [Serializable]
    [XmlRoot("ServiceResultData", Namespace = "http://egov.bg/RegiX/SignedData")]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    public class ServiceResultDataSigned<R, T> : IProvideServiceResultData, IErrorInfo
        where T : class
        where R : class
    {
        /// <summary>
        /// Field containing error message
        /// </summary>
        private string _error;

        /// <summary>
        /// If data is received from the administratin's register
        /// </summary>
        [XmlElement]
        public bool IsReady { get; set; }

        /// <summary>
        /// Result object
        /// </summary>
        [XmlElement]
        public T Result { get; set; }

        /// <summary>
        /// Request object
        /// </summary>
        [XmlElement]
        public R Request { get; set; }

        /// <summary>
        /// Access matrix. Describes the properties to which the client has access
        /// </summary>
        [XmlElement]
        public DataAccessMatrix Matrix { get; set; }

        /// <summary>
        /// If there was an error during the execution of the operation
        /// </summary>
        [XmlElement]
        public bool HasError { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [XmlElement]
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                HasError = !string.IsNullOrEmpty(_error);
            }
        }

        /// <summary>
        /// Signature of the DataContainer
        /// </summary>
        [XmlAnyElement]
        public XmlElement Signature { get; set; }

        /// <summary>
        /// Identifier of the operation call
        /// </summary>
        [XmlElement]
        public decimal ServiceCallID { get; set; }

        /// <summary>
        /// Verification code used in asynchronous scenairo
        /// </summary>
        [XmlElement]
        public string VerificationCode { get; set; }

        /// <summary>
        /// Converts to signed ServiceResultData
        /// </summary>
        /// <returns>The constructed object</returns>
        public ServiceResultData ToServiceResultDataSigned()
        {
            ServiceResultData srd =
                new ServiceResultData()
                {
                    IsReady = IsReady,
                    HasError = HasError,
                    Error = Error,
                    ServiceCallID = ServiceCallID,
                    VerificationCode = VerificationCode
                };
            if (IsReady && Result != null && !HasError)
            {   
                srd.Data = SigningUtils.GetDataContainer(Request.XmlSerialize().ToXmlElement(), Result.XmlSerialize().ToXmlElement(), Matrix); 
                srd.Signature = Signature;
            }
            return srd;
        }
    }
}
