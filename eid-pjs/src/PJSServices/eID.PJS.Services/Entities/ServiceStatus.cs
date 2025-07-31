using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services
{
    public class ServiceStatus<TState, TConfig> : ServiceStatusBase<TState>
        where TState : class, IPerformanceMetrics
        where TConfig : StatefulServiceSettingsBase
    {
        public SchedulerSettings? SchedulerConfiguration { get; set; }
        public TConfig? ServiceConfiguration { get; set; }
    }
}
