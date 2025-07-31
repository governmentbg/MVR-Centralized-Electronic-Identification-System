using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using TechnoLogica.RegiX.Adapters.Common.ExportExtension;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Common.DataContracts.Health;

namespace TechnoLogica.RegiX.Adapters.Common
{
    internal class HealthInfoFunctions
    {
        /// <summary>
        /// Проверява версията на инсталирания адаптер
        /// </summary>
        [Export(typeof(HealthInfo))]
        [ExportFullName(typeof(BaseAdapterService), typeof(HealthInfo))]
        public static HealthInfoWithFunction GetAdapterVersionHC =
            new HealthInfoWithFunction()
            {
                Key = "GetAdapterVersion",
                Name = "Проверява версията на инсталирания адаптер",
                Description = "Проверява версията на инсталирания адаптер",
                CheckFunction =
                (IAdapterService theAdapter) =>
                {
                    return theAdapter.GetAdapterVersion();
                }
            };

        /// <summary>
        /// Проверява връзката на адаптера със Source-a си
        /// </summary>
        [Export(typeof(HealthInfo))]
        [ExportFullName(typeof(BaseAdapterService), typeof(HealthInfo))]
        public static HealthInfoWithFunction CheckRegisterConnectionHC =
            new HealthInfoWithFunction()
            {
                Key = "CheckRegisterConnection",
                Name = "Проверява връзката на адаптера със Source-a си",
                Description = "Проверява връзката на адаптера със Source-a си",
                CheckFunction =
                (IAdapterService theAdapter) =>
                {
                    return theAdapter.CheckRegisterConnection();
                }
            };
        /// <summary>
        /// Дава информация за машината на която е host-нат адаптера
        /// </summary>
        [Export(typeof(HealthInfo))]
        [ExportFullName(typeof(BaseAdapterService), typeof(HealthInfo))]
        public static HealthInfoWithFunction GetHostMachineInfoHC =
            new HealthInfoWithFunction()
            {
                Key = "GetHostMachineInfo",
                Name = "Дава информация за машината на която е host-нат адаптера",
                Description = "Дава информация за машината на която е host-нат адаптера",
                CheckFunction =
                (IAdapterService theAdapter) =>
                {
                    return theAdapter.GetHostMachineInfo();
                }
            };

        /// <summary>
        /// Дава информация за Adapter Uptim
        /// </summary>
        [Export(typeof(HealthInfo))]
        [ExportFullName(typeof(BaseAdapterService), typeof(HealthInfo))]
        public static HealthInfoWithFunction GetAdapterUptimeHC =
            new HealthInfoWithFunction()
            {
                Key = "GetAdapterUptime",
                Name = "Дава информация за Adapter Uptime",
                Description = "Дава информация за Adapter Uptime",
                CheckFunction =
                (IAdapterService theAdapter) =>
                {
                    var uptimeSeconds = theAdapter.GetAdapterUptime();
                    var uptime = TimeSpan.FromSeconds(uptimeSeconds);
                    return string.Format("{0} days, {1} hours, {2} minutes, {3} seconds; TimeSpanValue:{4};",
                        uptime.Days,
                        uptime.Hours,
                        uptime.Minutes,
                        uptime.Seconds,
                        uptime.ToString());
                }
            };
    }
}
