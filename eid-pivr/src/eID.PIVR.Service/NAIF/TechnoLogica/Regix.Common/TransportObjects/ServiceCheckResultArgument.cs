using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Request for retrieving the result of an asynchronous operation
    /// </summary>
    [Serializable]
    public class ServiceCheckResultArgument
    {
        /// <summary>
        /// Identifier of an asynchronous operation. This identifier is part of the elements 
        /// of the result returned by the execute operation
        /// </summary>
        [XmlElement]
        public decimal ServiceCallID { get; set; }

        /// <summary>
        /// If the ServiceCallID is specified
        /// </summary>
        [XmlIgnore]
        public bool ServiceCallIDSpecified { get; set; }

        /// <summary>
        /// Verification code used to verify the consumer
        /// </summary>
        [XmlElement]
        public string VerificationCode { get; set; }
    }
}
