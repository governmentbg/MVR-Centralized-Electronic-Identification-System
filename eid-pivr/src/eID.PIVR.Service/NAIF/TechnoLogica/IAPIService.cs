using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Describes the common operations provided by each API service.
    /// </summary>
    public interface IAPIService
    {
        /// <summary>
        /// Adapter client for handling the requests
        /// </summary>
        IAdapterClient AdapterClient { get; }

        /// <summary>
        /// Executes a request
        /// </summary>
        /// <param name="request">Argument for the execution of the service</param>
        /// <returns>Result of the operation execution</returns>
        ServiceResultData Execute(ServiceRequestData request);

        /// <summary>
        /// Cehcks the result of an operation's execution (for asynchronous requests)
        /// </summary>
        /// <param name="checkResult">Argument containing the identifier of an asynchronouse request</param>
        /// <returns>Result of the execution of the asynchronous request</returns>
        ServiceResultData CheckResult(ServiceCheckResultArgument checkResult);

        /// <summary>
        /// Acknowledge that the result is received in the consumer
        /// </summary>
        /// <param name="checkResult">Argument containing the serviceCallID</param>
        void AcknowledgeResultReceived(ServiceCheckResultArgument checkResult);

        /// <summary>
        /// Retrieves the request XSD for the given operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved XSD</returns>
        string GetRequestXSD(string operationName);

        /// <summary>
        /// Retrieves the response XSD for the given operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved XSD</returns>
        string GetResponseXSD(string operationName);

        /// <summary>
        /// Retrieves the common XSD for the given operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved XSDs</returns>
        string[] GetCommonXSD(string operationName);

        /// <summary>
        /// Retrieves the common XML for the given operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved XMLs</returns>
        string GetMetaDataXML(string operationName);

        /// <summary>
        /// Retrieves all XSDs for the given operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved XSDs</returns>
        byte[] GetXSD(string operationName);

        /// <summary>
        /// Retrieves a sample request for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xml</returns>
        string GetSampleRequest(string operationName);

        /// <summary>
        /// Retrieves a sample response for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xml</returns>
        string GetSampleResponse(string operationName);

        /// <summary>
        /// Retrieves a sample xslt for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xml</returns>
        string GetRequestXslt(string operationName);

        /// <summary>
        /// Retrieves a xslt for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xslt</returns>
        string GetResponseXslt(string operationName);

        /// <summary>
        /// Retrieves a sample xsltFOP for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xsltFOP</returns>
        string GetRequestXsltFOP(string operationName);

        /// <summary>
        /// Retrieves a xsltFOP for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xsltFOP</returns>
        string GetResponseXsltFOP(string operationName);

        /// <summary>
        /// Retrieves a sample request/response for the operation
        /// </summary>
        /// <param name="operationName">Name of the operation</param>
        /// <returns>Retrieved sample xml</returns>
        byte[] GetSamples(string[] operationName);
    }
}
