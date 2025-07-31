using System;
using System.Collections.Generic;
using System.Text;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.Adapters.Common.Interfaces
{
    /// <summary>
    /// Processor for asynchronouse operations
    /// </summary>
    /// <typeparam name="T">Type of the adapter service </typeparam>
    public interface IAsynchronousProcessor<T> : IDisposable
        where T : IAdapterService
    {
        /// <summary>
        /// Adapter service instance
        /// </summary>
        T AdapterService { get; set; }

        /// <summary>
        /// Process the asynchronouse request
        /// </summary>
        /// <typeparam name="Req">Request type</typeparam>
        /// <typeparam name="Resp">Response type</typeparam>
        /// <param name="request">Request value</param>
        /// <param name="accessMatrix">Access matrix</param>
        /// <param name="additionalParameters">Additional parameters</param>
        /// <param name="response">Response value</param>
        /// <returns>CommonSignedResponse containing the result. (If the operation
        /// finishes synchronously this is the result, otherwise the isReady property 
        /// is set to false and the operation is executed asynchronously)</returns>
        CommonSignedResponse<Req, Resp> Process<Req, Resp>(Req request, AccessMatrix accessMatrix, AdapterAdditionalParameters additionalParameters, Resp response = null)
            where Req : class
            where Resp : class;
    }
}
