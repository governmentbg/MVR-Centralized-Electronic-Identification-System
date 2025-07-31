using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Defines adapter service that contains asynchronous operations
    /// </summary>
    public interface IAsynchronousAdapterService : IAdapterService
    {
        /// <summary>
        /// Starts the asynchronous processing of request
        /// </summary>
        /// <typeparam name="Req">Type of the request</typeparam>
        /// <typeparam name="Resp">Type of hte response</typeparam>
        /// <param name="request">Request</param>
        /// <param name="accessMatrix">Access matrix</param>
        /// <param name="additionalParameters">Additional parameters</param>
        /// <param name="response">Response</param>
        /// <param name="callingMethodName">Caller method name</param>
        /// <returns>Result of the operation</returns>
        CommonSignedResponse<Req, Resp> ProcessAsynchronous<Req, Resp>(Req request, AccessMatrix accessMatrix, AdapterAdditionalParameters additionalParameters, Resp response = null, [CallerMemberName] string callingMethodName = "")
             where Req : class
             where Resp : class;

        /// <summary>
        /// Called when the asynchronouse processing of an operation is finished
        /// </summary>
        /// <typeparam name="Req">Request type</typeparam>
        /// <typeparam name="Resp">Response type</typeparam>
        /// <param name="response">Result of the operation</param>
        /// <param name="additionalParameters">Additional parameters</param>
        /// <returns>If the response is sent to the caller</returns>
        bool ProcessCallback<Req, Resp>(CommonSignedResponse<Req, Resp> response, AdapterAdditionalParameters additionalParameters)
             where Req : class
             where Resp : class;
    }
}
