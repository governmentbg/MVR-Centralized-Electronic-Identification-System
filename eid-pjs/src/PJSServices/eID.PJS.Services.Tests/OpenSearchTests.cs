
using Microsoft.Extensions.Logging.Abstractions;

using OpenSearch.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using eID.PJS.Services.Verification;
using eID.PJS.Services.OpenSearch;

namespace eID.PJS.Services.Tests
{
    public class OpenSearchTests
    {
        private OpenSearchManager mgr;

        public OpenSearchTests()
        {
            mgr = new OpenSearchManager(new OpenSearchManagerSettings 
            {
                Hosts = new[] { "https://localhost:9200" },
                Password = "admin",
                UserName  = "admin",
            });
        }


        //[Fact]
        //public void DoSearchLogsTest()
        //{
        //    var result = mgr.GetAuditLogsForFile(OpenSearchManager.GetOpenSearchIndexName("eid-rei").LogsIndex, "eid-rei_920e4ce1-7398-4f4e-85e0-ce92ea164982_2023081611_157.audit");

        //    Assert.True(true);
        //}

        //[Fact]
        //public void DoSearchHashTest()
        //{
        //    var result = mgr.GetHashForFile(OpenSearchManager.GetOpenSearchIndexName("eid-rei").HashIndex, "eid-rei_920e4ce1-7398-4f4e-85e0-ce92ea164982_2023081611_157.audit");

        //    Assert.True(true);
        //}

        //[Fact]
        //public void GetLogFilesTest()
        //{
        //    var result = mgr.GetLogFiles("eid-audit-eid-rei");

        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task PingTestAsync()
        //{ 
        //    var result = await mgr.PingAsync();
        //}

    }
}
