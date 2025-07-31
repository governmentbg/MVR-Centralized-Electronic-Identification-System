using System.Diagnostics;
using eID.PJS.LocalLogsSearch.Service;
using eID.PJS.LocalLogsSearch.Service.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace eID.PJS.LocalLogsSearch.Tests;

#nullable disable
public class LogSearchServiceTests : ServiceTestBase
{
    private ILogger<AuditLogsFileService> _logger;
    private DateRange _maxDateRange;
    private string _samplesFolder;
    private AuditLogSearchSettings _settings;

    public LogSearchServiceTests()
    {
        _logger = new NullLogger<AuditLogsFileService>();

        _maxDateRange = DateRange.MaxRange();
        _maxDateRange.FromDate = _maxDateRange.FromDate.AddSeconds(1);

        _settings = new AuditLogSearchSettings { 
            MaxRecords = 100000,
        };

        _samplesFolder = LocateSamples();

        if (string.IsNullOrEmpty(_samplesFolder))
            throw new FileNotFoundException($"Cannot locate 'samples' directory starting from {Directory.GetCurrentDirectory()} and searching up to the root.");

        if (!Directory.GetFiles(_samplesFolder, "*.audit").Any())
            throw new FileNotFoundException($"Cannot find any audit log files in the samples folder: {_samplesFolder}");

    }

    private string LocateSamples()
    {
        DirectoryInfo curDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        DirectoryInfo foundDir = curDir.GetDirectories("samples").FirstOrDefault();


        if (foundDir != null)
            return foundDir.FullName;
        
        foundDir = curDir.Parent;

        while (foundDir != null)
        {
            var tmp = foundDir.GetDirectories("samples").FirstOrDefault();
            
            if(tmp != null)
                return tmp.FullName;

            foundDir = foundDir.Parent;
        }

        return null;
    }


    [Fact]
    public void SearchLogsFilterValidationTest1()
    {
        var filter = new SearchLogsRequest();
    }

    [Fact]
    public void ReadLogsTest()
    {
        Debug.WriteLine("==== ReadLogsTest ====");
        var result = GetFiles();

        Assert.NotNull(result);
        Assert.True(result.Records.Any());
    }

    [Fact]
    public void FilterByFileDateRangeTest()
    {
        Debug.WriteLine("==== FilterByFileDateRangeTest ====");
        Stopwatch sw = Stopwatch.StartNew();

        //var result = data.Files.SelectTokens("$[?(@.eventDate >'2023-06-11' &&  @.eventDate <'2023-06-15' )]", errorWhenNoMatch: true);

        var reader = new AuditLogsFileService(_logger, _settings);
        var logs = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = new DateRange(new DateTime(2023, 8, 1), new DateTime(2023, 8, 30)), DataQuery = null });

        sw.Stop();

        Debug.WriteLine($"Number of Records: {logs.Records.Count}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {logs.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {logs.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {logs.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(logs);
        Assert.True(logs.Records.Any());

    }

    [Fact]
    public void FilterByFileDateRange_Estimate_Test()
    {
        Debug.WriteLine("==== FilterByFileDateRange_Estimate_Test ====");
        Stopwatch sw = Stopwatch.StartNew();

        //var result = data.Files.SelectTokens("$[?(@.eventDate >'2023-06-11' &&  @.eventDate <'2023-06-15' )]", errorWhenNoMatch: true);

        var reader = new AuditLogsFileService(_logger, _settings);
        var estimate = reader.EstimateQueryScope(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = new DateRange(new DateTime(2023, 08, 1), new DateTime(2023, 08, 30)), DataQuery = null });

        sw.Stop();

        Debug.WriteLine($"Estimated Number of Records: {estimate.RecordsCount}");
        Debug.WriteLine($"Estimated Number of Files: {estimate.FilesCount}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {estimate.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {estimate.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {estimate.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(estimate);
        Assert.True(estimate.RecordsCount > 0);
    }

    [Fact]
    public void FilterByFileMaxDateRange_Estimate_Test()
    {
        Debug.WriteLine("==== FilterByFileMaxDateRange_Estimate_Test ====");
        Stopwatch sw = Stopwatch.StartNew();

        var reader = new AuditLogsFileService(_logger, _settings);
        var estimate = reader.EstimateQueryScope(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = null });

        sw.Stop();

        Debug.WriteLine($"Estimated Number of Records: {estimate.RecordsCount}");
        Debug.WriteLine($"Estimated Number of Files: {estimate.FilesCount}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {estimate.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {estimate.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {estimate.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(estimate);
        Assert.True(estimate.RecordsCount > 0);
    }

    [Fact]
    public void FilterByFileMaxDateRangeTest()
    {
        Debug.WriteLine("==== FilterByFileMaxDateRangeTest ====");
        Stopwatch sw = Stopwatch.StartNew();

        var reader = new AuditLogsFileService(_logger, _settings);
        var estimate = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = null });

        sw.Stop();

        Debug.WriteLine($"Estimated Number of Records: {estimate.RecordsCount}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {estimate.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {estimate.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {estimate.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(estimate);
        Assert.True(estimate.RecordsCount > 0);
    }


    [Fact]
    public void FilterBySystemIdTest()
    {
        Debug.WriteLine("==== FilterBySystemIdTest ====");

        Stopwatch sw = Stopwatch.StartNew();

        //var result = data.Files.SelectTokens("$[?(@.eventPayload.FieldString=='BRQXBXOOZTIMZDBIENNL' || @.eventPayload.FieldNum==651827900 || @.eventPayload.FieldSHIT == 0)]", errorWhenNoMatch: true).ToList();

        var json = @"{

                          ""type"": ""compare"",
                          ""operator"": ""Equal"",
                          ""field"": ""systemId"",
                          ""value"": ""eid-mpozei""
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        var reader = new AuditLogsFileService(_logger, _settings);
        var logs = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = query });

        sw.Stop();

        Debug.WriteLine($"Number of Records: {logs.Records.Count}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {logs.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {logs.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {logs.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(logs);
        Assert.True(logs.Records.Any());
    }

    [Fact]
    public void FilterByRecordDateRangeTest()
    {
        Debug.WriteLine("==== FilterByRecordDateRangeIdTest ====");

        Stopwatch sw = Stopwatch.StartNew();

        //var result = data.Files.SelectTokens("$[?(@.eventPayload.FieldString=='BRQXBXOOZTIMZDBIENNL' || @.eventPayload.FieldNum==651827900 || @.eventPayload.FieldSHIT == 0)]", errorWhenNoMatch: true).ToList();

        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThan"",
                          ""field"": ""eventDate"",
                          ""value"": ""2023-07-10T00:00:00""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThan"",
                          ""field"": ""eventDate"",
                          ""value"": ""2023-07-21T00:00:00""
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        var reader = new AuditLogsFileService(_logger, _settings);
        var logs = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = query });

        sw.Stop();

        Debug.WriteLine($"Number of Records: {logs.Records.Count}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {logs.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {logs.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {logs.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(logs);
        Assert.True(logs.Records.Any());
    }


    [Fact]
    public void FilterByPayloadFieldNumTest()
    {
        Debug.WriteLine("==== FilterByPayloadFieldNumTest ====");

        Stopwatch sw = Stopwatch.StartNew();

        var json = @"{

                          ""type"": ""compare"",
                          ""operator"": ""GreaterThanOrEqual"",
                          ""field"": ""eventPayload.FieldNum"",
                          ""value"": 200000000
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        var reader = new AuditLogsFileService(_logger, _settings);
        var logs = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = query });

        sw.Stop();

        Debug.WriteLine($"Number of Records: {logs.Records.Count}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {logs.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {logs.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {logs.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(logs);
        Assert.True(logs.Records.Any());
    }

    [Fact]
    public void FilterByPayload_FieldDate_RangeTest()
    {
        Debug.WriteLine("==== FilterByPayload_FieldDate_RangeTest ====");

        Stopwatch sw = Stopwatch.StartNew();

        var json = @"{
                      ""type"": ""logical"",
                      ""operator"": ""and"",
                      ""nodes"": [
                        {
                          ""type"": ""compare"",
                          ""operator"": ""GreaterThanOrEqual"",
                          ""field"": ""eventPayload.FieldDate"",
                          ""value"": ""2023-05-01T00:00:00""
                        },
                        {
                          ""type"": ""compare"",
                          ""operator"": ""LessThanOrEqual"",
                          ""field"": ""eventPayload.FieldDate"",
                          ""value"": ""2023-05-30T00:00:00""
                        }
                      ]
                    }";

        var query = JsonConvert.DeserializeObject<QueryNode>(json);

        var reader = new AuditLogsFileService(_logger, _settings);
        var logs = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = query });

        sw.Stop();

        Debug.WriteLine($"Number of Records: {logs.Records.Count}");
        Debug.WriteLine($"Elapsed Time: {sw.Elapsed}");
        Debug.WriteLine($"ProcessingTime: {logs.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {logs.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {logs.Metrics.PagedMemorySize.ToFileSize()}");

        Assert.NotNull(logs);
        Assert.True(logs.Records.Any());
    }

    private SearchLogsResult GetFiles()
    {
        var reader = new AuditLogsFileService(_logger, _settings);

        var logs = reader.FilterLogs(new SearchLogsRequest { Folders = new string[] { _samplesFolder }, FileExtensionFilter = "*.audit", FileDateRangeFilter = _maxDateRange, DataQuery = null });


        Debug.WriteLine("===== LOAD LOGS ======");
        Debug.WriteLine($"Number of Records: {logs.Records.Count}");
        Debug.WriteLine($"ProcessingTime: {logs.Metrics.ProcessingTime}");
        Debug.WriteLine($"PrivateMemorySize: {logs.Metrics.PrivateMemorySize.ToFileSize()}");
        Debug.WriteLine($"PagedMemorySize: {logs.Metrics.PagedMemorySize.ToFileSize()}");

        return logs;
    }

}
