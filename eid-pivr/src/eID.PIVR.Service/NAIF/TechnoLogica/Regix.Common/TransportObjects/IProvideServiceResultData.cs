
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Defines functionality for converstion to ServiceResultData object
    /// </summary>
    public interface IProvideServiceResultData
    {
        /// <summary>
        /// Convert to ServiceResultData
        /// </summary>
        /// <returns>The ServiceResultData object</returns>
        ServiceResultData ToServiceResultDataSigned();
    }
}
