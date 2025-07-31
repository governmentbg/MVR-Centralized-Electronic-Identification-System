using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.Adapters.Common.Interfaces
{
    /// <summary>
    /// Defines functionality for persisting ServiceResultData objects. Used 
    /// in asynchronous processing.
    /// </summary>
    public interface IPersistanceProvider
    {
        /// <summary>
        /// Marks that the service call with the provided peristanceID is currently beeing processed
        /// </summary>
        /// <param name="persistanceID">Peristance ID</param>
        void PersistProcessing(decimal persistanceID, string verificationCode);

        /// <summary>
        /// Persists ServiceResultData object with the given persistanceID
        /// </summary>
        /// <param name="persistanceID">Persitance ID</param>
        /// <param name="result">Object ot persist</param>
        void PersistResult(decimal persistanceID, ServiceResultData result);

        /// <summary>
        /// Retrieves persisted object
        /// </summary>
        /// <param name="persistanceID">Persitance ID</param>
        /// <returns>Retrieved object</returns>
        ServiceResultData RetrieveResult(decimal persistanceID, string verificationCode);

        /// <summary>
        /// Removes information for persisted service result
        /// </summary>
        /// <param name="persistanceID">Peristance ID</param>
        void Remove(decimal persistanceID, string verificationCode);
    }
}
