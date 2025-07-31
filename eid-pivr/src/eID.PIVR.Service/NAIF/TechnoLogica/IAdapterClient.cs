using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Typed interface for a helper class calling an adapter
    /// </summary>
    /// <typeparam name="T">IAPIService interface</typeparam>
    public interface IAdapterClient<T>: IAdapterClient
        where T : IAPIService
    {
    }

    /// <summary>
    /// Interface for a helper class calling an adapter
    /// </summary>
    public interface IAdapterClient
    {
        /// <summary>
        /// Executes an operation. Used for direct calls to adapter service interfaces
        /// </summary>
        /// <typeparam name="I">Interface of the adapter</typeparam>
        /// <typeparam name="R">Request type</typeparam>
        /// <typeparam name="T">Response type</typeparam>
        /// <param name="func">Expression of the call to the interface's operation</param>
        /// <param name="serviceRequest">Request argument</param>
        /// <param name="operationName">Operation name</param>
        /// <returns>Operation response</returns>
        ServiceResultDataSigned<R, T> Execute<I, R, T>(
            Expression<Func<I, R, AccessMatrix, AdapterAdditionalParameters, CommonSignedResponse<R, T>>> func, 
            ServiceRequestData<R> serviceRequest,
            [CallerMemberName] string operationName = "")
            where T : class
            where R : class
            where I : IAdapterService;

        /// <summary>
        /// Executes an operation. Used for generic calls to adapter service
        /// </summary>
        /// <param name="apiService">Instance of an API Service</param>
        /// <param name="request">Request argument</param>
        /// <returns>Operation response</returns>
        ServiceResultData Execute(
            IAPIService apiService, 
            ServiceRequestData request);
        
        /// <summary>
        /// Executes a check operation of adapter.
        /// </summary>
        /// <param name="apiService">Instance of an API Service</param>
        /// <param name="checkResult">Request argument containing APIServiceCallID</param>
        /// <returns>Operation response</returns>
        ServiceResultData CheckResult(
            IAPIService apiService, 
            ServiceCheckResultArgument checkResult);

        /// <summary>
        /// Acknowledge that the result is received in the consumer
        /// </summary>
        /// <param name="apiService">Instance of an API Service</param>
        /// <param name="checkResult">Argument containing the serviceCallID</param>
        void AcknowledgeResultReceived(
            IAPIService apiService, 
            ServiceCheckResultArgument checkResult);
    }
}
