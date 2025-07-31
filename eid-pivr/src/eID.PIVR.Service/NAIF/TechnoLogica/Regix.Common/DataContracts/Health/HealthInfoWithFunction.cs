using System;

namespace TechnoLogica.RegiX.Common.DataContracts.Health
{

    /// <summary>
    /// Health function defined in Func
    /// </summary>
    public class HealthInfoWithFunction : HealthInfo
    {
        /// <summary>
        /// Function containing the logic for the execution of the health check
        /// </summary>
        public Func<IAdapterService, string> CheckFunction;


        /// <summary>
        /// Executes health check
        /// </summary>
        /// <returns>Result of the executed health check</returns>
        public override string Check(IAdapterService adapterService)
        {
            return CheckFunction.Invoke(adapterService);
        }
    }
}
