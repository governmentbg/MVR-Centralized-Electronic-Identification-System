using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.OpenSearch;

#nullable disable
public class OpenSearchDistinctScriptResult
{
    [JsonProperty("aggregations")]
    public Aggregations Aggregations { get; set; }
}

public class Aggregations
{
    [JsonProperty("DistinctValues")]
    public DistinctValues DistinctValues { get; set; }
}

public class DistinctValues
{
    [JsonProperty("value")]
    public string[] Value { get; set; }
}


