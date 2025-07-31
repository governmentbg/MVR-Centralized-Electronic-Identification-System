using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoLogica.RegiX.Common.TransportObjects
{
    /// <summary>
    /// Defines possible type of request processing. Usage as flags is possible
    /// </summary>
    [Flags]
    public enum RequestProcessing
    {
        /// <summary>
        /// The request should be verified (for signature validitiy)
        /// </summary>
        Verify = 1,

        /// <summary>
        /// The request should be decrypted
        /// </summary>
        Decrypt = 2,

        /// <summary>
        /// The request should be transformed from PDF
        /// </summary>
        TransformFromPDF = 4,

        /// <summary>
        /// The request should be executed as an asynchronous operation
        /// </summary>
        Asynchronous = 8
    }
}
