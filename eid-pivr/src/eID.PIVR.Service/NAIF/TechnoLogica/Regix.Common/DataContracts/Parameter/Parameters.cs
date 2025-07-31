using System;
using System.Collections.Generic;

namespace TechnoLogica.RegiX.Common.DataContracts.Parameter
{
    /// <summary>
    /// Contains list of parameters
    /// </summary>
    [Serializable]
    public class Parameters
    {
        /// <summary>
        /// List of parameters
        /// </summary>
        public List<ParameterInfo> ParameterList { get; set; }
    }
}
