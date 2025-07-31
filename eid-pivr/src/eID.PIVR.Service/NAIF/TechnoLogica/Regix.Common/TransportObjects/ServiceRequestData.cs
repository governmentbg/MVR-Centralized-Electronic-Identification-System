using System;
using System.Xml;
using System.Xml.Serialization;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.Utils;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Request data for executing an operation part of RegiX
    /// </summary>
    [XmlRoot("ServiceRequestData", Namespace = "http://egov.bg/RegiX/SignedData")]
    [XmlType(Namespace = "http://egov.bg/RegiX/SignedData")]
    [Serializable]
    public class ServiceRequestData : IServiceRequestDataWithOperation
    {
        /// <summary>
        /// Type of request processing
        /// </summary>
        [XmlElement]
        public RequestProcessing RequestProcessing { get; set; }

        /// <summary>
        /// Type of response processing
        /// </summary>
        [XmlElement]
        public ResponseProcessing ResponseProcessing { get; set; }

        /// <summary>
        /// Name of the oepration. For more information visit http://regixaisweb.egov.bg/regixinfo/. 
        /// Example: "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.ValidPersonSearch"
        /// </summary>
        [XmlElement]
        public string Operation { get; set; }

        /// <summary>
        /// Fullname of the contract's interface describing the operations part of a given register
        /// </summary>
        [XmlIgnore]
        public string Contract
        {
            get
            {
                if (!String.IsNullOrEmpty(Operation))
                {
                    try
                    {
                        return Operation.Substring(0, Operation.LastIndexOf('.'));
                    }
                    catch
                    {
                        //TODO: Exception Manager
                        throw new ArgumentException("");// ExceptionManager.GetEntryPointExceptionArgumentExceptionNameByCode("Operation"), "Operation");
                    }
                }
                else
                {
                    return null;
                    //TODO: Uncomment
                  //  throw new ArgumentException("The element request.Operation is required", "Operation");
                }
            }
        }

        /// <summary>
        /// Name of the operation. Defined by the Operation property
        /// </summary>
        [XmlIgnore]
        public string OperationName
        {
            get
            {
                try
                {
                    return Operation.Substring(Operation.LastIndexOf('.') + 1);
                }
                catch
                {
                    //TODO: Exception Manager
                    // throw new ArgumentException(""); //throw new ArgumentException(string.Format(StringResources.OperationNameRequired, Operation));
                    //TODO: Comment
                    return null;
                }
            }
        }

        /// <summary>
        /// XML value of the request. For more information regarding the structure (XSD) of the request of the different operations visit: http://regixaisweb.egov.bg/regixinfo/.
        /// Its important that the elements should be qualified (include the namespace for the requested operation)
        /// </summary>
        [XmlElement]
        public XmlElement Argument { get; set; }

        /// <summary>
        /// Electronic ientifier (received from http://eid.egov.bg/)
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string EIDToken { get; set; }

        /// <summary>
        /// Caller context. This context is stored in the RegiX's log
        /// </summary>
        [XmlElement]
        public CallContext CallContext { get; set; }

        /// <summary>
        /// Callback URL - used to call the client in case of asynchronous execution
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string CallbackURL { get; set; }

        /// <summary>
        /// Identifier of the employee executing the request. Not stored in RegiX's log - directly sent to the Administration's information system
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string EmployeeEGN { get; set; }

        /// <summary>
        /// Identifier of the citizen targeted by the operation. Not stored in RegiX's Log, directly forwarded to the Adminsistraiont's register
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string CitizenEGN { get; set; }

        //TODO: Remove SignResult? Move it to responseProcessing?
        /// <summary>
        /// If the result should be signed
        /// </summary>
        [XmlElement]
        public bool SignResult { get; set; }

        //TODO: Remove ReturnAccessMatrix? Move it to responseProcessing?
        /// <summary>
        /// If the access matrix should be returned as part of the result
        /// </summary>
        [XmlElement]
        public bool ReturnAccessMatrix { get; set; }

        /// <summary>
        /// Sets the value of the argument proeprty. Used for simplified conversion between the generic and the XMLElement variants of the ServiceRequestData object
        /// </summary>
        /// <param name="argument">XML value of the argument</param>
        public void SetArgument(XmlElement argument)
        {
            Argument = argument;
        }
    }

    /// <summary>
    /// Request data (generic) for executing an operation part of RegiX
    /// </summary>
    /// <typeparam name="T">Type of the operation request argument</typeparam>
    [Serializable]
    public class ServiceRequestData<T> : IServiceRequestData
    where T : class
    {
        /// <summary>
        /// Object argument for the operation
        /// </summary>
        [XmlElement]
        public T Argument { get; set; }

        /// <summary>
        /// Electronic ientifier (received from http://eid.egov.bg/)
        /// </summary>
        [XmlElement]
        public string EIDToken { get; set; }

        /// <summary>
        /// Caller context. This context is stored in the RegiX's log
        /// </summary>
        [XmlElement]
        public CallContext CallContext { get; set; }

        /// <summary>
        /// Identifier of the employee executing the request. Not stored in RegiX's log - directly sent to the Administration's information system
        /// </summary>
        [XmlElement]
        public string EmployeeEGN { get; set; }

        /// <summary>
        /// Identifier of the citizen targeted by the operation. Not stored in RegiX's Log, directly forwarded to the Adminsistraiont's register
        /// </summary>
        [XmlElement]
        public string CitizenEGN { get; set; }

        /// <summary>
        /// Callback URL - used to call the client in case of asynchronous execution
        /// </summary>
        [XmlElement]
        public string CallbackURL { get; set; }

        //TODO: Remove SignResult? Move it to responseProcessing?
        /// <summary>
        /// If the result should be signed
        /// </summary>
        [XmlElement]
        public bool SignResult { get; set; }

        //TODO: Remove ReturnAccessMatrix? Move it to responseProcessing?
        /// <summary>
        /// If the access matrix should be returned as part of the result
        /// </summary>
        [XmlElement]
        public bool ReturnAccessMatrix { get; set; }

        /// <summary>
        /// Sets the value of the argument proeprty. Used for simplified conversion between the generic and the XMLElement variants of the ServiceRequestData object
        /// </summary>
        /// <param name="argument">XML value of the argument</param>
        public void SetArgument(XmlElement argument)
        {
            if (argument != null)
            {
                try
                {
                    object deserializedArgument = argument.Deserialize(typeof(T));
                    Argument = deserializedArgument as T;
                }
                catch
                {
                    //TODO: Exception Manager
                    throw new ArgumentException("");// GetEntryPointExceptionArgumentExceptionNameByCode("Argument"), "Argument");
                }
            }
            else
            {
                throw new ArgumentException("Element request.Argument is required and it must contain XML content", "Argument");
            }
        }

        /// <summary>
        /// Creates a ServiceRequestData with asynchronous request processing
        /// </summary>
        /// <returns>The created ServiceRequestData object</returns>
        public ServiceRequestData CreateAsyncServiceRequest()
        {
            ServiceRequestData asyncRequest =
                new ServiceRequestData()
                {
                    RequestProcessing = RequestProcessing.Asynchronous,
                    Argument = Argument.XmlSerialize().ToXmlElement(),
                    CallbackURL = CallbackURL,
                    CallContext = CallContext,
                    CitizenEGN = CitizenEGN,
                    EIDToken = EIDToken,
                    EmployeeEGN = EmployeeEGN,
                    ReturnAccessMatrix = ReturnAccessMatrix,
                    SignResult = SignResult
                };
            return asyncRequest;
        }
    }
}
