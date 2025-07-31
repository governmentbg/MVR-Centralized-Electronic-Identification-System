using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLogica.RegiX.Common.DataContracts.Health
{
    /// <summary>
    /// Contains list of with health info function definitions
    /// </summary>
    [Serializable]
    public class HealthCheckFunctions
    {
        /// <summary>
        /// List of health info function
        /// </summary>
        public List<HealthInfo> HealthInfosList { get; set; }
    }
}
