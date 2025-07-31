using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services
{
    public class CommandStateProviderSettings
    {
        public TimeSpan ResultTTL { get; set; } = TimeSpan.FromHours(1); // Keep the results from the command execution for 1 hour by default
        public TimeSpan CommandStatusTTL { get; set; } = TimeSpan.FromMinutes(20); // Keep the informationm for the command in progress for 20 minutes by default

    }

    public class InMemoryCommandStateProviderSettings: CommandStateProviderSettings
    { 
        
    }
}
