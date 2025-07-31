using eID.PJS.AuditLogging;

using Microsoft.AspNetCore.Mvc.Formatters;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OpenSearch.Client;
using OpenSearch.Net;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eID.PJS.Services.Signing;

namespace eID.PJS.Services.OpenSearch;

public class OpenSearchManager
{
    private StaticConnectionPool _pool;
    private ConnectionSettings _conStrings;
    private OpenSearchClient _client;
    private OpenSearchManagerSettings _settings;

    private const string DISTINCT_QUERY_BY_SOURCE_FILE = @"
                                    {
                                        ""aggs"": {
                                            ""DistinctCount"": {
                                                ""scripted_metric"": {
                                                    ""params"": {
                                                        ""fieldName"": ""sourceFile""
                                                    },
                                                    ""init_script"": ""state.list = []"",
                                                    ""map_script"": ""if(params['_source'][params.fieldName] != null) state.list.add(params['_source'][params.fieldName]);"",
                                                    ""combine_script"": ""return state.list;"",
                                                    ""reduce_script"": ""Map uniqueValueMap = new HashMap();int count = 0;for(shardList in states){if(shardList != null){for(key in shardList){if(!uniqueValueMap.containsKey(key)){count +=1;uniqueValueMap.put(key, key);}}}}return count;""
                                                }
                                            },
                                            ""DistinctValues"": {
                                                ""scripted_metric"": {
                                                    ""params"": {
                                                        ""fieldName"": ""sourceFile""
                                                    },
                                                    ""init_script"": ""state.list = []"",
                                                    ""map_script"": ""if(params['_source'][params.fieldName] != null) state.list.add(params['_source'][params.fieldName]);"",
                                                    ""combine_script"": ""return state.list;"",
                                                    ""reduce_script"": ""Map uniqueValueMap = new HashMap();List uniqueValueList = new ArrayList();for(shardList in states){if(shardList != null){for(key in shardList){if(!uniqueValueMap.containsKey(key)){uniqueValueList.add(key);uniqueValueMap.put(key, key);}}}}return uniqueValueList;""
                                                }
                                            }
                                        }
                                    }";


    public OpenSearchManager(OpenSearchManagerSettings settings)
    {
        _settings = settings;

        if (_settings.Hosts == null || _settings.Hosts.Length == 0)
            throw new ArgumentNullException(nameof(_settings.Hosts));


        var hostList = _settings.Hosts.ToList();

        var nodes = hostList.Select(u => new Uri(u));

        _pool = new StaticConnectionPool(nodes);
        _conStrings = new ConnectionSettings(_pool);

        if (_settings.RequestTimeout != null)
        {
            _conStrings.RequestTimeout(TimeSpan.FromSeconds(_settings.RequestTimeout.Value));
        }

        if (_settings.MaxRetryTimeout != null)
        {
            _conStrings.MaxRetryTimeout(TimeSpan.FromSeconds(_settings.MaxRetryTimeout.Value));
        }

        //TODO: Update this according to how is configured the production OpenSearch server
        _conStrings.BasicAuthentication(_settings.UserName, _settings.Password);

        // TODO: Do we verify the certificates?
        _conStrings.ServerCertificateValidationCallback((o, a, b, c) =>
        {
            return true;
        });

        _client = new OpenSearchClient(_conStrings);
    }

    public AuditLogsForFileResult GetAuditLogsForFile(string indexName, string fileName)
    {
        // TODO: How to get all possible matches without the size limitaiton.
        var body = _client.Search<AuditLogRecord>(sd => sd.Index(indexName).Query(q => q.Match(m => m.Field(f => f.SourceFile).Query(fileName))).Size(10000));

        return new AuditLogsForFileResult
        {
            Data = body.Documents,
            IsValid = body.IsValid,
            Error = body.OriginalException,
            DebugInformation = body.IsValid == false ? body.DebugInformation : null
        };
    }

    public async Task<AuditLogsForFileResult> GetAuditLogsForFileAsync(string indexName, string fileName, CancellationToken cancelToken)
    {
        var body = await _client.SearchAsync<AuditLogRecord>(sd => sd.Index(indexName).Query(q => q.Match(m => m.Field(f => f.SourceFile).Query(fileName))).Size(10000), cancelToken);

        return new AuditLogsForFileResult
        {
            Data = body.Documents,
            IsValid = body.IsValid,
            Error = body.OriginalException,
            DebugInformation = body.IsValid == false ? body.DebugInformation : null
        };

    }

    public async Task<AuditLogsForFileResult> GetAllAuditLogsForFileAsync(string indexName, string fileName, CancellationToken cancelToken)
    {
        const int batchSize = 10000; // Adjust batch size if needed
        List<AuditLogRecord> allRecords = new List<AuditLogRecord>();

        bool hasMore = true;
        bool isValid = true;
        string? debugInfo = null;
        Exception? error = null;
        int startIndex = 0;

        while (hasMore)
        {
            var response = await _client.SearchAsync<AuditLogRecord>(s => s
                .Index(indexName)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.SourceFile)
                        .Query(fileName)))
                .From(startIndex)
                .Size(batchSize)
            , cancelToken);

            if (!response.IsValid)
            {
                isValid = false;

                debugInfo = response.DebugInformation;
                error = response.OriginalException;
                break;
            }

            if (!response.IsValid || response.Documents == null || !response.Documents.Any())
            {
                hasMore = false;
            }
            else
            {
                allRecords.AddRange(response.Documents);
                startIndex += response.Documents.Count;
                hasMore = response.Documents.Count == batchSize;
            }

            if (cancelToken.IsCancellationRequested)
            {
                break;
            }
        }

        return new AuditLogsForFileResult
        {
            Data = allRecords,
            IsValid = isValid,
            Error = error,
            DebugInformation = isValid == false ? debugInfo : null
        };
    }

    public AuditLogsForFileResult GetAuditLogsForFile2(string indexName, string fileName)
    {
        try
        {
            var searchResponseLow = _client.LowLevel.DoRequest<StringResponse>(global::OpenSearch.Net.HttpMethod.POST, "_plugins/_sql",
                                     PostData.Serializable(
                                         new
                                         {
                                             query = $"select * from {indexName} where sourceFile='{fileName}' order by eventId desc"
                                         }), null);


            var data = JsonConvert.DeserializeObject<OpenSearchSqlResult>(searchResponseLow.Body);

            if (data == null)
                return new AuditLogsForFileResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };

            if (data.datarows.Count == 0)
                return new AuditLogsForFileResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };

            var result = new List<AuditLogRecord>();

            var schema = new Dictionary<string, int>();

            for (var index = 0; index < data.schema.Count; index++)
            {
                schema[data.schema[index].name] = index;
            }

            var rowNum = 0;
            foreach (var dataRow in data.datarows)
            {
                rowNum++;
                var record = new AuditLogRecord()
                {
                    EventId = dataRow[schema["eventId"]].ToString(),
                    SystemId = dataRow[schema["systemId"]].ToString(),
                    Checksum = dataRow[schema["checksum"]].ToString(),
                    EventPayload = (SortedDictionary<string, object>)dataRow[schema["eventPayload"]],
                    EventType = dataRow[schema["eventType"]].ToString(),
                    Message = dataRow[schema["message"]].ToString(),
                    SourceFile = dataRow[schema["sourceFile"]].ToString(),
                    CorrelationId = dataRow[schema["correlationId"]].ToString(),
                    ModuleId = dataRow[schema["moduleId"]].ToString(),
                    RequesterSystemId = dataRow[schema["requesterSystemId"]].ToString(),
                    RequesterUserId = dataRow[schema["requesterUserId"]].ToString(),
                    TargetUserId = dataRow[schema["targetUserId"]].ToString(),
                };

                if (!DateTime.TryParseExact(dataRow[schema["eventDate"]].ToString(), "yyyy-MM-dd HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp))
                {
                    throw new InvalidCastException($"Cannot convert the string '{dataRow[schema["eventDate"]]}' to DateTime in log file '{fileName}' for EventId '{record.EventId}' on row number {rowNum}.");
                }

                record.EventDate = timestamp;

                result.Add(record);
            }


            return new AuditLogsForFileResult
            {
                Data = result,
                IsValid = searchResponseLow.Success,
                Error = searchResponseLow.OriginalException,
                DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Reading of logs in index '{indexName}' for log file '{fileName}' failed.", ex);
        }
    }

    public HashForFileResult GetHashForFile(string indexName, string fileName)
    {
        var body = _client.Search<AuditChecksumRecord>(sd => sd.Index(indexName).Query(q => q.Match(m => m.Field(f => f.SourceFile).Query(fileName))));

        return new HashForFileResult
        {
            Data = body.Documents.FirstOrDefault(),
            IsValid = body.IsValid,
            Error = body.OriginalException,
            DebugInformation = body.IsValid == false ? body.DebugInformation : null
        };
    }

    public async Task<HashForFileResult> GetHashForFileAsync(string indexName, string fileName, CancellationToken cancelToken)
    {
        var body = await _client.SearchAsync<AuditChecksumRecord>(sd => sd.Index(indexName).Query(q => q.Match(m => m.Field(f => f.SourceFile).Query(fileName))), cancelToken);

        return new HashForFileResult
        {
            Data = body.Documents.FirstOrDefault(),
            IsValid = body.IsValid,
            Error = body.OriginalException,
            DebugInformation = body.IsValid == false ? body.DebugInformation : null
        };

    }

    public HashForFileResult GetHashForFile2(string indexName, string fileName)
    {
        try
        {



            var searchResponseLow = _client.LowLevel.DoRequest<StringResponse>(global::OpenSearch.Net.HttpMethod.POST, "_plugins/_sql",
                                     PostData.Serializable(
                                         new
                                         {
                                             query = $"select * from {indexName} where sourceFile='{fileName}'"
                                         }), null);


            var data = JsonConvert.DeserializeObject<OpenSearchSqlResult>(searchResponseLow.Body);

            if (data == null)
                return new HashForFileResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };

            if (data.datarows.Count == 0)
                return new HashForFileResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };


            if (data.datarows.Count > 1)
                throw new Exception($"Query to find hashes in index '{indexName}' for file '{fileName}' must return exactly one row but it returns {data.datarows.Count} rows");

            var schema = new Dictionary<string, int>();

            for (var index = 0; index < data.schema.Count; index++)
            {
                schema[data.schema[index].name] = index;
            }

            var dataRow = data.datarows[0];
            var record = new AuditChecksumRecord()
            {
                SystemId = dataRow[schema["systemId"]].ToString(),
                Checksum = dataRow[schema["checksum"]].ToString(),
                SourceFile = dataRow[schema["sourceFile"]].ToString(),
                SourcePath = dataRow[schema["sourcePath"]].ToString(),
                Events = dataRow[schema["events"]].ToString(),
            };

            if (!DateTime.TryParseExact(dataRow[schema["dateCreated"]].ToString(), "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp))
            {
                throw new InvalidCastException($"Cannot convert the string '{dataRow[schema["dateCreated"]]}' to DateTime in hash file '{fileName}'.");
            }

            record.DateCreated = timestamp;

            return new HashForFileResult
            {
                Data = record,
                IsValid = searchResponseLow.Success,
                Error = searchResponseLow.OriginalException,
                DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Reading of hash in index '{indexName}' for log file '{fileName}' failed.", ex);
        }
    }

    public async Task<HashForFileResult> GetHashForFile2Async(string indexName, string fileName, CancellationToken cancelToken)
    {
        try
        {

            var searchResponseLow = await _client.LowLevel.DoRequestAsync<StringResponse>(global::OpenSearch.Net.HttpMethod.POST, "_plugins/_sql", cancelToken,
                                     PostData.Serializable(
                                         new
                                         {
                                             query = $"select * from {indexName} where sourceFile='{fileName}'"
                                         }), null);


            var data = JsonConvert.DeserializeObject<OpenSearchSqlResult>(searchResponseLow.Body);

            if (data == null)
                return new HashForFileResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };

            if (data.datarows.Count == 0)
                return new HashForFileResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };

            if (data.datarows.Count > 1)
                throw new Exception($"Query to find hashes in index '{indexName}' for file '{fileName}' must return exactly one row but it returns {data.datarows.Count} rows");

            var schema = new Dictionary<string, int>();

            for (var index = 0; index < data.schema.Count; index++)
            {
                schema[data.schema[index].name] = index;
            }

            var dataRow = data.datarows[0];
            var record = new AuditChecksumRecord()
            {
                SystemId = dataRow[schema["systemId"]].ToString(),
                Checksum = dataRow[schema["checksum"]].ToString(),
                SourceFile = dataRow[schema["sourceFile"]].ToString(),
                SourcePath = dataRow[schema["sourcePath"]].ToString(),
                Events = dataRow[schema["events"]].ToString(),
            };

            if (!DateTime.TryParseExact(dataRow[schema["dateCreated"]].ToString(), "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var timestamp))
            {
                throw new InvalidCastException($"Cannot convert the string '{dataRow[schema["dateCreated"]]}' to DateTime in hash file '{fileName}'.");
            }

            record.DateCreated = timestamp;

            return new HashForFileResult
            {
                Data = record,
                IsValid = searchResponseLow.Success,
                Error = searchResponseLow.OriginalException,
                DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Reading of hash in index '{indexName}' for log file '{fileName}' failed.", ex);
        }
    }

    public (string LogsIndex, string HashIndex) GetOpenSearchIndexName(string? systemId)
    {
        if (string.IsNullOrWhiteSpace(systemId)) throw new ArgumentException(nameof(systemId));
        if (string.IsNullOrWhiteSpace(_settings.EnvironmentName))
        {
            return ($"eid-audit-{systemId}", $"eid-hash-{systemId}");
        }
        return ($"eid-audit-{_settings.EnvironmentName.ToLowerInvariant()}-{systemId}", $"eid-hash-{_settings.EnvironmentName.ToLowerInvariant()}-{systemId}");
    }

    public LogFilesResult GetLogFiles(string indexName)
    {
        try
        {
            var searchResponseLow = _client.LowLevel.DoRequest<StringResponse>(global::OpenSearch.Net.HttpMethod.GET, $"{indexName}/_search", PostData.String(DISTINCT_QUERY_BY_SOURCE_FILE));

            var data = JsonConvert.DeserializeObject<OpenSearchDistinctScriptResult>(searchResponseLow.Body);

            if (data == null || data.Aggregations == null)
            {
                return new LogFilesResult
                {
                    IsValid = searchResponseLow.Success,
                    Error = searchResponseLow.OriginalException,
                    DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
                };
            }

            return new LogFilesResult
            {
                Data = data.Aggregations.DistinctValues.Value,
                IsValid = searchResponseLow.Success,
                Error = searchResponseLow.OriginalException,
                DebugInformation = searchResponseLow.Success == false ? searchResponseLow.DebugInformation : null
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Getting the distinct log file names in index {indexName} failed.", ex);
        }
    }

    public async Task<bool> PingAsync()
    {
        PingResponse result;

        if (_settings.PingTimeout != null)
        {
            result = await _client.PingAsync(p => p.RequestConfiguration(c => c.PingTimeout(TimeSpan.FromSeconds(_settings.PingTimeout.Value))));
        }
        else
        {
            result = await _client.PingAsync();
        }

        return result.IsValid;
    }

    public bool Ping()
    {
        PingResponse result;

        if (_settings.PingTimeout != null)
        {
            result = _client.Ping(p => p.RequestConfiguration(c => c.PingTimeout(TimeSpan.FromSeconds(_settings.PingTimeout.Value))));
        }
        else
        {
            result = _client.Ping();
        }

        return result.IsValid;
    }

    public abstract class OpenSearchManagerResult<T>
    {
        public T? Data { get; set; }
        public bool IsValid { get; set; }
        public Exception? Error { get; set; }
        public string? DebugInformation { get; set; }
    }

    public class AuditLogsForFileResult : OpenSearchManagerResult<IEnumerable<AuditLogRecord>?> { }

    public class HashForFileResult : OpenSearchManagerResult<AuditChecksumRecord?> { }

    public class LogFilesResult : OpenSearchManagerResult<IEnumerable<string>?> { }
}

