using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.Entities
{
    public class SimplePerformanceMetrics: IPerformanceMetrics
    {
        public PerformanceMetric Metrics {  get; set; } = new PerformanceMetric();
    }
}
