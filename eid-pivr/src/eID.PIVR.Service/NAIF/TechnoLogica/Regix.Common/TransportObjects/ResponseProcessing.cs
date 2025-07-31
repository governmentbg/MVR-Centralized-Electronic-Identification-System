using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Defines possible type of response processing. Usage as flags is possible
    /// </summary>
    [Flags]
    public enum ResponseProcessing
    {
        /// <summary>
        /// The response should be encrypted
        /// </summary>
        Encrypt = 1,

        /// <summary>
        /// The response should be converted to PDF
        /// </summary>
        TransformToPDF = 2,

/*Too much changes needed...
        /// <summary>
        /// The response should be signed
        /// </summary>
        SignResult = 4,

        /// <summary>
        /// The response should include the access matrix
        /// </summary>
        ReturnAccessMatrix = 8
*/
    }
}
