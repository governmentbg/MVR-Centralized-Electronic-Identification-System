using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Adapters.Common.Interfaces
{
    /// <summary>
    /// Response processor. Defines functionality for additonal processing of service results.
    /// </summary>
    public interface IResponseProcessor
    {
        /// <summary>
        /// Processes ServiceResultData object
        /// </summary>
        /// <param name="result">ServiceResultData object</param>
        /// <param name="additionalParameters">Additional parameters</param>
        /// <returns>Processed object</returns>
        ServiceResultData ProcessResponse(ServiceResultData result, AdapterAdditionalParameters additionalParameters);
    }
}
