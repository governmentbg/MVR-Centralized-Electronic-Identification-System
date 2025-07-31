using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.DataContracts;
using TechnoLogica.RegiX.Adapters.Common.ExportExtension;
using TechnoLogica.RegiX.Common.DataContracts.Parameter;

namespace TechnoLogica.RegiX.Adapters.Common
{
    public class ParametersInfoDefinitions
    {            
        /// <summary>
        /// Адрес на ядрото използван за асинхронното изпълнение чрез използването на callback
        /// </summary>
        [Export(typeof(ParameterInfo))]
        [ExportFullName(typeof(BaseAdapterService), typeof(ParameterInfo))]
        public static ParameterInfo<string> RegiXCallbackEndpoint =
            new ParameterInfo<string>("http://localhost/RegiX.Workflows/RegiXReceiveResult.svc")
            {
                Key = "RegiXCallbackService",
                Description = "Адрес на ядрото използван за асинхронното изпълнение чрез използването на callback",
                OwnerAssembly = typeof(BaseAdapterService).Assembly
            };
    }
}
