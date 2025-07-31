using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.OpenSearch;

#nullable disable
public class OpenSearchSqlResult
{
    public OpenSearchSqlResult()
    {

    }

    public List<SchemaItem> schema { get; set; }
    public List<List<object>> datarows { get; set; }
    public int total { get; set; }
    public int size { get; set; }
    public int status { get; set; }
}

public class SchemaItem
{
    public string name { get; set; }
    public string type { get; set; }
}

