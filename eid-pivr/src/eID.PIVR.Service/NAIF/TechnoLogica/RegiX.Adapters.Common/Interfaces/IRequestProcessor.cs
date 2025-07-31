using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Adapters.Common.Interfaces
{
    /// <summary>
    /// Request processor. Used for additional processing of requests.
    /// </summary>
    public interface IRequestProcessor
    {
        /// <summary>
        /// Process the request argument
        /// </summary>
        /// <param name="request">Request argument</param>
        /// <param name="additionalParameters">Additional paramters</param>
        /// <returns>Processed request</returns>
        ServiceRequestData ProcessRequest(
            ServiceRequestData request, 
            AdapterAdditionalParameters additionalParameters);
    }
}
