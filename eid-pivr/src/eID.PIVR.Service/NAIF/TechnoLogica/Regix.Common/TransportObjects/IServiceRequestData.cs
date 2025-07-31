using System;
using System.Xml;
using TechnoLogica.RegiX.Common.DataContracts;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Defines functionality for simpler conversion between the generic and the XmlElemenet versions of the ServiceRequestData objecct
    /// </summary>
    public interface IServiceRequestData
    {
        /// <summary>
        /// Electronic identifier (received from http://eid.egov.bg/). 
        /// </summary>
        [Obsolete]
        string EIDToken { get; set; }

        /// <summary>
        /// Caller context - stored as part of RegiX's logs
        /// </summary>
        CallContext CallContext { get; set; }

        /// <summary>
        /// Identifier of the employee executing the request. Not stored in RegiX's log - directly sent to the Administration's information system
        /// </summary>
        string EmployeeEGN { get; set; }

        /// <summary>
        /// Identifier of the citizen targeted by the operation. Not stored in RegiX's Log, directly forwarded to the Adminsistraiont's register
        /// </summary>
        string CitizenEGN { get; set; }

        /// <summary>
        /// Callback URL used for calling consumer in asynchronous scenario
        /// </summary>
        string CallbackURL { get; set; }

        /// <summary>
        /// If the result should be signed
        /// </summary>
        bool SignResult { get; set; }

        /// <summary>
        /// If the access matrix should be returned as part of the result
        /// </summary>
        bool ReturnAccessMatrix { get; set; }

        /// <summary>
        /// Sets the value of the argument proeprty. Used for simplified conversion between the generic and the XMLElement variants of the ServiceRequestData object
        /// </summary>
        /// <param name="argument">XML value of the argument</param>
        void SetArgument(XmlElement argument);
    }
}
