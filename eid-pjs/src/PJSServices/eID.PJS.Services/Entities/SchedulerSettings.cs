using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services
{
    public abstract class SchedulerSettings
    {
        /// <summary>
        /// Schedule period in seconds
        /// </summary>
        public int SchedulePeriod { get; set; }
    }
}
