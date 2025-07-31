using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Serilog.Core;
using System.Configuration;


using System.Runtime;
using OpenSearch.Net;
using OpenSearch.Client;
using eID.PJS.AuditLogging;
using eID.PJS.Services.Verification;
using eID.PJS.Services.Signing;
using eID.PJS.Services.OpenSearch;

#nullable disable

namespace eID.PJS.Services.Tests
{
    public class VerificationTests : ServiceTestBase
    {
        private OpenSearchManager _osMgr;
        private VerificationServiceSettings _settings;
        private ICryptoKeyProvider _cryptoKeyProvider;
        private IFileChecksumAlgorhitm _fileChecksumAlgorhitm;
        private ILogger<VerificationService> _logger;
        private VerificationService _service;
        private IEnumerable<string> _files;
        private IEnumerable<AuditLogRecord> _records;
        public VerificationTests()
        {
            _logger = new NullLogger<VerificationService>();

            _osMgr = new OpenSearchManager(new OpenSearchManagerSettings
            {
                Hosts = new[] { "https://localhost:9200" },
                Password = "admin",
                UserName = "admin",
            });

            _settings = new VerificationServiceSettings
            {
                KeepStateHistory = true,
                StatePath = "..\\..\\..\\..\\logs\\VerificationService\\VerificationService_Tests.json",
                Systems = new Dictionary<string, SystemLocations> {
                    {
                        "eid-test", 
                        new SystemLocations 
                        {
                            AuditLogs =  "..\\..\\..\\..\\logs\\eid-test\\log", 
                            Hashes = "..\\..\\..\\..\\logs\\eid-test\\hash"
                        } 
                    }
                },
                AuditLogFileExtension = "*.audit",
                HashFileExtension = "*.hash",
                VerifyPeriod = VerificationCheckPeriod.All
            };

            _cryptoKeyProvider = new DummyCryptoKeyProvider();
            _fileChecksumAlgorhitm = new HMACSignAlgorhitm(_cryptoKeyProvider);

            //_service = new VerificationService(_logger, _settings, _osMgr, _fileChecksumAlgorhitm, _cryptoKeyProvider);

            //LoadData();
        }

        private void LoadData()
        {
            string indexName = _osMgr.GetOpenSearchIndexName("eid-test").LogsIndex;
            _files = _osMgr.GetLogFiles(indexName).Data;

            Assert.NotEmpty(_files);

            string file = _files.Skip(1).FirstOrDefault();

            _records = _osMgr.GetAuditLogsForFile(indexName, file).Data;

            Assert.NotEmpty(_records);
        }


        //[Fact]
        public void AuditLogRecordVerificationTest()
        {
            var record = _records.Skip(1).FirstOrDefault();
            var result = _service.VerifyLogRecordChecksum(record);

            Assert.True(result);

        }

        //[Fact]
        public void VerifyLogsForFileTest()
        {
            //eid-shit_38524f7b-5614-4f02-9db6-9c23a3098f80_2023090410_003.audit

            //var result = _service.VerifyFile("eid-test_38524f7b-5614-4f02-9db6-9c23a3098f80_2023090410_003.audit", new Exclusions());

        }

        //[Fact]
        public void VerifyAllLogsTest()
        {
            //eid-shit_38524f7b-5614-4f02-9db6-9c23a3098f80_2023090410_003.audit

            var result = _service.Process();


        }

        //[Fact]
        public void VerifyLogsFilterTest()
        {
            var result = _service.VerifyPeriod("eid-test", new DateTime(2022, 09, 10).ToLocalTime(), DateTime.Now);
        }

    }
}
