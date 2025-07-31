using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLogica.RegiX.Common.DataContracts.Health
{
    /// <summary>
    /// Health info function
    /// </summary>
    [Serializable]
    public class HealthInfo
    {
        /// <summary>
        /// Executes health check
        /// </summary>
        /// <returns>Result of the executed health check</returns>
        public virtual string Check(IAdapterService adapterService)
        {
            return "Check function not defined!";
        }

        /// <summary>
        /// Name of the health function
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the health funciton
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Key of the health function
        /// </summary>
        public string Key { get; set; }
    }
}
