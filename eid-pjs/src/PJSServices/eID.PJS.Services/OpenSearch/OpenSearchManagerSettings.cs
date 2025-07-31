using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eID.PJS.Services.OpenSearch;

#nullable disable

public class OpenSearchManagerSettings
{
    public string[] Hosts { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int? RequestTimeout { get; set; }
    public int? MaxRetryTimeout { get; set; }
    public int? PingTimeout { get; set; }
    public string EnvironmentName { get; set; }
}

