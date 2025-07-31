using System;
using System.Collections.Generic;
using System.ServiceModel;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.DataContracts.Health;
using TechnoLogica.RegiX.Common.DataContracts.Parameter;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Common interface for all adapter services
    /// </summary>
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IAdapterService
    {
        /// <summary>
        /// Type of the adapter's interface
        /// </summary>
        Type AdapterServiceInterface { get; }

        /// <summary>
        /// Type of the adapter (useful for mock scenarios
        /// </summary>
        Type AdapterServiceType { get; }

        /// <summary>
        /// Name of the adapter service
        /// </summary>
        string AdapterServiceName { get; }

        /// <summary>
        /// Executes operation
        /// </summary>
        /// <param name="request">Request argument for the operation</param>
        /// <param name="accessMatrix">Access matrix to be applied to the result</param>
        /// <param name="additionalParameters">Additional context paramters</param>
        /// <returns></returns>
        [OperationContract]
        ServiceResultData Execute(ServiceRequestData request, AccessMatrix accessMatrix, AdapterAdditionalParameters additionalParameters);

        /// <summary>
        /// Checks the execution of an asynchronous operation
        /// </summary>
        /// <param name="checkResult">Argument containing the request's id (ServiceCallID)</param>
        /// <returns>Result of the operation's execution</returns>
        [OperationContract]
        ServiceResultData CheckResult(ServiceCheckResultArgument checkResult);

        /// <summary>
        /// Acknowledge that the result is received in the consumer
        /// </summary>
        /// <param name="checkResult">Argument containing the serviceCallID</param>
        [OperationContract]
        void AcknowledgeResultReceived(ServiceCheckResultArgument checkResult);

        /// <summary>
        /// Checks the accessibility of the adapter by accepting and returning a data argument
        /// </summary>
        /// <param name="data">Data to be returned</param>
        /// <returns>The data response</returns>
        [OperationContract]
        byte[] Ping(byte[] data);

        /// <summary>
        /// Gets the version of the adapter
        /// </summary>
        /// <returns>Version of the adapter</returns>
        [OperationContract]
        string GetAdapterVersion();

        /// <summary>
        /// Gets the adapter's uptime
        /// </summary>
        /// <returns>Adapter's uptime</returns>
        [OperationContract]
        uint GetAdapterUptime();

        /// <summary>
        /// Gets the system's uptime
        /// </summary>
        /// <returns>System's uptime</returns>
        [OperationContract]
        uint GetSystemUptime();

        /// <summary>
        /// Gets information for the host machine
        /// </summary>
        /// <returns>Host machine information</returns>
        [OperationContract]
        string GetHostMachineInfo();

        /// <summary>
        /// Checks the connection to the register
        /// </summary>
        /// <returns>Result of the check</returns>
        [OperationContract]
        string CheckRegisterConnection();
        
        /// <summary>
        /// Gets the definition of all the arguments
        /// </summary>
        /// <returns>All arguments' definitions</returns>
        [OperationContract]
        Parameters GetParameters();

        /// <summary>
        /// Sets the value of a single parameter
        /// </summary>
        /// <param name="key">Key of the parameter</param>
        /// <param name="value">String representation of the parameter</param>
        [OperationContract]
        void SetParameter(string key, string value);

        /// <summary>
        /// Sets multiple parameters
        /// </summary>
        /// <param name="parameters">Parameters list</param>
        [OperationContract]
        void SetParameters(List<ParameterInfo> parameters);

        /// <summary>
        /// Get all health check function definitions
        /// </summary>
        /// <returns>The health check definitions</returns>
        [OperationContract]
        HealthCheckFunctions GetHealthCheckFunctions();

        /// <summary>
        /// Checks a single health check function
        /// </summary>
        /// <param name="key">Key of the health check function</param>
        /// <returns>result of the health check</returns>
        [OperationContract]
        string CheckHealthFunction(string key);

        /// <summary>
        /// Gets the public key of the adapter used for signing responses
        /// </summary>
        /// <returns>The current public key of the certificate used for signing responses</returns>
        [OperationContract]
        string GetPublicKeyString();

        /// <summary>
        /// Tries to send the prepared result to RegiX.Core
        /// </summary>
        /// <param name="result">Result of operation</param>
        /// <param name="callbackURL">CallbackURL</param>
        /// <returns>If the operation is successfull</returns>
        bool SendResultToCore(ServiceResultData result, string callbackURL);
    }
}
