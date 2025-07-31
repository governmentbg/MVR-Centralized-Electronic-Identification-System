using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Common
{
    /// <summary>
    /// Indicates that the adapter is hosted in .NET Core host
    /// </summary>
    public interface IAdapterServiceNETCore : IAdapterService
    {
        /// <summary>
        /// Operation needed for the proper working of an adapter hosted in .NET Core host
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        ServiceResultData Execute(RequestWrapper request);
    }
}
