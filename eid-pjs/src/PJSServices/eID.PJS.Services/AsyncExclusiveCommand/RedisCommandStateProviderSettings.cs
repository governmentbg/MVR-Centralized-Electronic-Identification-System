using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSearch.Client;

namespace eID.PJS.Services.AsyncExclusiveCommand;

public class RedisCommandStateProviderSettings: CommandStateProviderSettings
{
    public string ConnectionString { get; set; } = "localhost:6379";
}

