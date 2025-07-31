using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Common.TransportObjects
{

    /// <summary>
    /// Object returned as a result of the operation's execution
    /// </summary>
    [Serializable]
    [XmlRoot("ServiceResultData", Namespace = "http://egov.bg/RegiX/SignedData")]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    public class ServiceResultData : IErrorInfo
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
        /// Data returned by the oepration. This property has value if HasError=false and IsReady=true
        /// </summary>
        [XmlElement(IsNullable = true)]
        public DataContainer Data { get; set; }

        /// <summary>
        /// Signature of the DataContainer
        /// </summary>
        [XmlAnyElement]
        public XmlElement Signature { get; set; }

        /// <summary>
        /// If there was an error during the execution of the operation
        /// </summary>
        [XmlElement]
        public bool HasError { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [XmlElement(IsNullable = true)]
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
        /// Converts XML variant of the ServiceResultData to the generic ServiceResultDataSigned implementation
        /// </summary>
        /// <typeparam name="R">Request type</typeparam>
        /// <typeparam name="T">Response type</typeparam>
        /// <returns>The converted object</returns>
        public ServiceResultDataSigned<R, T> ToServiceResultDataSigned<R, T>()
            where T : class
            where R : class
        {
            ServiceResultDataSigned<R, T> result = new ServiceResultDataSigned<R, T>
            {
                HasError = HasError,
                Error = Error,
                IsReady = IsReady,
                Matrix = Data?.Matrix,
                Request = Data?.Request?.Request?.Deserialize<R>(),
                Result = Data?.Response?.Response?.Deserialize<T>(),
                Signature = Signature,
                VerificationCode = VerificationCode,
                ServiceCallID = ServiceCallID
            };
            return result;
        }
    }
}
