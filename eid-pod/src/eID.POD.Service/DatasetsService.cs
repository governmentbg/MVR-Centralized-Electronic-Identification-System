using System.Net;
using System.Text;
using eID.POD.Contracts.Commands;
using eID.POD.Contracts.Results;
using eID.POD.Service.Database;
using eID.POD.Service.Entities;
using eID.POD.Service.Jobs;
using eID.POD.Service.Options;
using eID.POD.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Quartz;

namespace eID.POD.Service;

public class DatasetsService : BaseService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _openDataHttpClient;
    private readonly ILogger<DatasetsService> _logger;
    private readonly OpenDataSettings _openDataSettings;
    private readonly ApplicationDbContext _context;
    private readonly ISchedulerFactory _schedulerFactory;

    public DatasetsService(
        ILogger<DatasetsService> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<OpenDataSettings> openDataSettings,
        ApplicationDbContext context,
        ISchedulerFactory schedulerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClientFactory.CreateClient();
        _openDataHttpClient = httpClientFactory.CreateClient("OpenData");
        _openDataSettings = (openDataSettings ?? throw new ArgumentNullException(nameof(openDataSettings))).Value;
        _openDataSettings.Validate();
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
    }

    public async Task<ServiceResult<Dataset>> GetDatasetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
        }
        if (!Guid.TryParse(id, out Guid datasetId))
        {
            throw new ArgumentException($"'{nameof(id)}' is invalid.", nameof(id));
        }
        if (datasetId == Guid.Empty)
        {
            throw new ArgumentException($"'{nameof(id)}' cannot be empty guid.", nameof(id));
        }
        var dbResult = await _context.Datasets.FirstOrDefaultAsync(od => !od.IsDeleted && od.Id == datasetId);
        if (dbResult is null)
        {
            return NotFound<Dataset>(nameof(Dataset.Id), datasetId);
        }
        return Ok(dbResult);
    }

    public async Task<ServiceResult<IEnumerable<DatasetResult>>> GetAllDatasetsAsync(GetAllDatasets message)
    {
        // Validation
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        IEnumerable<DatasetResult> datasets = await _context.Datasets
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.DatasetName)
            .ToListAsync();

        return Ok(datasets);
    }

    public async Task<ServiceResult<IEnumerable<Dataset>>> GetActiveDatasetsAsync()
    {
        var datasets = await _context.Datasets.Where(ds => ds.IsActive && !ds.IsDeleted)
            .ToListAsync();

        return Ok(datasets.AsEnumerable());
    }

    public async Task<ServiceResult> UpdateDatasetAsync(UpdateDataset message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new UpdateDatasetValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(UpdateDatasetValidator), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var dataset = await _context.Datasets.FirstOrDefaultAsync(od => !od.IsDeleted && od.Id == message.Id);
        if (dataset is null)
        {
            _logger.LogInformation("Cannot find dataset with Id: {DatasetId}", message.Id);
            return NotFound(nameof(message.Id), message.Id);
        }

        if (!string.IsNullOrWhiteSpace(message.DatasetName))
        {
            // Name change should result into creation of a new dataset in OpenData portal
            if (dataset.DatasetName != message.DatasetName)
            {
                _logger.LogInformation("Dataset {DatasetId} name changed. Next data upload will create and use new OpenData portal dataset.", dataset.Id);
                dataset.DatasetUri = string.Empty;
            }
            dataset.DatasetName = message.DatasetName;
        }

        if (!string.IsNullOrWhiteSpace(message.CronPeriod))
        {
            dataset.CronPeriod = message.CronPeriod;
        }

        if (!string.IsNullOrWhiteSpace(message.DataSource))
        {
            dataset.DataSource = message.DataSource;
        }

        dataset.IsActive = message.IsActive;
        dataset.LastModifiedBy = message.LastModifiedBy;

        if (_openDataSettings.AutomaticStart)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();
            var job = await scheduler.GetJobDetail(new JobKey(dataset.Id.ToString()));
            if (dataset.IsActive)
            {
                // Job not found and we need to create new job
                if (job is null)
                {
                    IJobDetail newJob = JobBuilder.Create<UploadDataJob>()
                       .WithIdentity(dataset.Id.ToString())
                       .Build();

                    // Trigger for the new job
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(dataset.Id.ToString())
                        .WithCronSchedule(dataset.CronPeriod)
                        .Build();

                    // Tell Quartz to schedule the job using our trigger
                    _logger.LogInformation("Scheduling job for {DatasetId}", dataset.Id);
                    var nextExecutionTime = await scheduler.ScheduleJob(newJob, trigger);
                    if (nextExecutionTime == default)
                    {
                        _logger.LogWarning("Failed scheduling job with key: {DatasetId}.", dataset.Id);
                        return new ServiceResult
                        {
                            StatusCode = System.Net.HttpStatusCode.InternalServerError,
                            Error = "Failed scheduling job. Try again."
                        };
                    }
                }
                // Job found and we need reschedule it with updated information
                else
                {
                    var runningJobTrigger = await scheduler.GetTrigger(new TriggerKey(dataset.Id.ToString()));
                    if (runningJobTrigger is null)
                    {
                        _logger.LogInformation("Trigger for schedule job: {DatasetId} not found.", dataset.Id);
                        return NotFound<bool>(nameof(dataset.Id), dataset.Id);
                    }

                    // Create new trigger with the updated Cron Expression
                    ITrigger newTrigger = TriggerBuilder.Create()
                        .WithIdentity(dataset.Id.ToString())
                        .WithCronSchedule(dataset.CronPeriod)
                        .Build();

                    var nextExecutionTime = await scheduler.RescheduleJob(runningJobTrigger.Key, newTrigger);
                    _logger.LogInformation("Rescheduling job for {DatasetId}", dataset.Id);
                    if (nextExecutionTime == default)
                    {
                        _logger.LogWarning("Failed rescheduling job with key: {DatasetId}.", dataset.Id);
                        return new ServiceResult
                        {
                            StatusCode = System.Net.HttpStatusCode.InternalServerError,
                            Error = "Failed rescheduling job. Try again."
                        };
                    }
                }
            }
            else
            {
                // Trying to remove the job only if it exists
                if (job != null)
                {
                    // returns true if the job was found and deleted
                    if (!await scheduler.DeleteJob(job.Key))
                    {
                        _logger.LogWarning("Failed removing job with key: {JobKey}.", job.Key);
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Dataset {DatasetId} was updated.", dataset.Id);

        return NoContent();
    }

    public async Task<ServiceResult> DeleteDatasetAsync(DeleteDataset message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DeleteDatasetValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(DeleteDatasetValidator), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var dataset = await _context.Datasets.FirstOrDefaultAsync(d => d.Id == message.Id);
        if (dataset is null)
        {
            _logger.LogInformation("Cannot find dataset with Id: {DatasetId}", message.Id);
            return NotFound(nameof(message.Id), message.Id);
        }

        dataset.IsDeleted = true;
        dataset.LastModifiedBy = message.LastModifiedBy;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Dataset {DatasetId} was deleted.", dataset.Id);

        IScheduler scheduler = await _schedulerFactory.GetScheduler();
        // returns true if the job was found and deleted
        var deleted = await scheduler.DeleteJob(new JobKey(dataset.Id.ToString()));
        if (!deleted)
        {
            _logger.LogWarning("Failed removing scheduled job with key: {DatasetId}.", dataset.Id);
        }

        return NoContent();
    }

    public async Task<ServiceResult<bool>> ExecuteUploadDatasetAsync(Dataset dataset)
    {
        if (dataset is null)
        {
            throw new ArgumentNullException(nameof(dataset));
        }

        if (string.IsNullOrWhiteSpace(dataset.DataSource))
        {
            throw new ArgumentNullException(nameof(dataset.DataSource));
        }

        if (!Uri.TryCreate(dataset.DataSource, UriKind.Absolute, out var dataSource))
        {
            throw new ArgumentNullException(nameof(dataset.DataSource), $"${nameof(dataset.DataSource)} doesn't contain absolute uri.");
        }
        // First we set LastRun property and then we execute the job
        await SetLastRunAsync(dataset.Id);

        // Get data from internal systems
        var response = await _httpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = dataSource
        });

        var dataSourceData = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Failed getting data for {DatasetId}. ({StatusCode}) {Response}", dataset.Id, response.StatusCode.ToString(), dataSourceData);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.BadGateway,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(response.StatusCode.ToString(), $"Failed getting data for {dataset.Id}")
                }

            };
        }

        if (string.IsNullOrWhiteSpace(dataSourceData))
        {
            _logger.LogInformation("Empty data received. No data will be uploaded to OpenData portal for {DatasetId}.", dataset.Id);
            return new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.BadGateway,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("No data", $"No data will be uploaded to OpenData portal for {dataset.Id}")
                }
            };
        }

        // Publishing data in OpenData takes three steps
        // 1. Create dataset if needed
        if (string.IsNullOrWhiteSpace(dataset.DatasetUri))
        {
            var createdDatasetUri = await CreateOpenDataPortalDatasetAsync(dataset);
            await UpdateDatasetUriAsync(dataset.Id, createdDatasetUri);
        }

        // 2. Create resource metadata to newly created dataset
        var metadataUri = await AddResourceMetadataAsync(dataset);

        // 3. Add resource data to resource metadata
        return Ok(await TryAddResourceDataAsync(metadataUri, dataSourceData));
    }

    public async Task<ServiceResult<Guid>> CreateDatasetAsync(CreateDataset message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new CreateDatasetValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(CreateDatasetAsync), validationResult);
            return BadRequest<Guid>(validationResult.Errors);
        }

        // We create dataset in DB without set DatsetUri. When it is executed, it will create Dataset in OpenData and set DatasetUri
        var newDataset = new Dataset
        {
            Id = Guid.NewGuid(),
            DatasetName = message.DatasetName,
            CronPeriod = message.CronPeriod,
            DataSource = message.DataSource,
            IsActive = message.IsActive,
            CreatedBy = message.CreatedBy,
            LastModifiedBy = message.CreatedBy
        };

        await _context.Datasets.AddAsync(newDataset);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Dataset {DatasetName} was created with id {DatasetId}.", newDataset.DatasetName, newDataset.Id);

        if (_openDataSettings.AutomaticStart && newDataset.IsActive)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();

            // Create job details
            IJobDetail job = JobBuilder.Create<UploadDataJob>()
                .WithIdentity(newDataset.Id.ToString())
                .Build();

            // Trigger the job to run now
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(newDataset.Id.ToString())
                .WithCronSchedule(newDataset.CronPeriod)
                .Build();

            // Tell Quartz to schedule the job using our trigger
            _logger.LogInformation("Scheduling job for {DatasetId}", newDataset.Id);
            var nextExecutionTime = await scheduler.ScheduleJob(job, trigger);
            if (nextExecutionTime == default)
            {
                // We won't make the client to retry the request.
                // Everything else completed successfully. He has the ability to manually run the job
                // or update it in order to trigger the schedule creation once again.
                _logger.LogWarning("Failed scheduling job with key: {DatasetId}.", newDataset.Id);
            }
        }

        return Created(newDataset.Id);
    }

    public async Task<ServiceResult> ManualUploadDatasetAsync(ManualUploadDataset message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ManualUploadDatasetValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(ManualUploadDatasetAsync), validationResult);
            return BadRequest(validationResult.Errors);
        }

        var serviceRequest = await GetDatasetByIdAsync(message.Id.ToString());
        if (serviceRequest is null)
        {
            return new ServiceResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Error = "Failed getting dataset."
            };
        }
        if (serviceRequest.StatusCode != HttpStatusCode.OK)
        {
            return serviceRequest;
        }
        var dataset = serviceRequest.Result;
        if (dataset is null)
        {
            _logger.LogInformation("Cannot find dataset {DatasetId}", message.Id);
            return NotFound(nameof(message.Id), message.Id);
        }

        _logger.LogInformation("Manual uploading dataset {DatasetId} started.", message.Id);

        IScheduler scheduler = await _schedulerFactory.GetScheduler();
        var alreadyRunningJob = (await scheduler.GetCurrentlyExecutingJobs())
            .FirstOrDefault(j => j.JobDetail.Key.Name == dataset.Id.ToString());

        if (alreadyRunningJob != null)
        {
            _logger.LogInformation("Job for dataset {DatasetId} is currently in progress. Try again later.", message.Id);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.Conflict,
                Errors = new List<KeyValuePair<string, string>>
                {
                    new ("In progress", $"Job for dataset {message.Id} is currently in progress. Try again later.")
                }
            };
        }

        var alreadyExistingJob = await scheduler.GetJobDetail(new JobKey(dataset.Id.ToString()));

        try
        {
            // Before manual triggering job, we have to be sure there is a schedule job for trigger. If it doesn't exists we create it and then we trigger it.
            if (alreadyExistingJob == null)
            {
                // Create job details
                IJobDetail newJob = JobBuilder.Create<UploadDataJob>()
                    .WithIdentity(dataset.Id.ToString())
                    .UsingJobData(UploadDataJob.ManualExecutionJobDataKey, true)
                    .Build();

                var newTrigger = TriggerBuilder.Create()
                   .WithIdentity(dataset.Id.ToString())
                   .StartNow()
                   .Build();

                await scheduler.ScheduleJob(newJob, newTrigger);
            }
            else
            {
                IDictionary<string, object> jobData = new Dictionary<string, object>()
                {
                    { UploadDataJob.ManualExecutionJobDataKey, true }
                };

                await scheduler.TriggerJob(alreadyExistingJob.Key, new JobDataMap(jobData));
            }

            _logger.LogInformation("Started uploading dataset job: {DatasetId}", message.Id);
            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Something went wrong when trying to upload dataset with Id: {DatasetId}", message.Id);
            return new ServiceResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Error = $"Something went wrong when trying to upload dataset with Id: {message.Id}."
            };
        }
    }


    private async Task<bool> UpdateDatasetUriAsync(Guid datasetId, string datasetUri)
    {
        if (Guid.Empty == datasetId)
        {
            throw new ArgumentException($"'{nameof(datasetId)}' cannot be empty.", nameof(datasetId));
        }

        if (string.IsNullOrWhiteSpace(datasetUri))
        {
            throw new ArgumentException($"'{nameof(datasetUri)}' cannot be null or whitespace.", nameof(datasetUri));
        }

        var dataset = await _context.Datasets
            .FirstOrDefaultAsync(d => d.Id == datasetId);

        if (dataset == null)
        {
            _logger.LogInformation("Cannot find dataset with Uri: {DatasetUri}", datasetUri);
            return false;
        }

        dataset.DatasetUri = datasetUri;
        await _context.SaveChangesAsync();
        _logger.LogInformation("{DatasetId} dataset uri updated.", datasetId);
        return true;
    }

    private async Task<bool> SetLastRunAsync(Guid datasetId)
    {
        if (Guid.Empty == datasetId)
        {
            throw new ArgumentException($"'{nameof(datasetId)}' cannot be empty.", nameof(datasetId));
        }

        var dataset = await _context.Datasets
            .FirstOrDefaultAsync(d => d.Id == datasetId);

        if (dataset == null)
        {
            _logger.LogInformation("Cannot find dataset {DatasetId}", datasetId);
            return false;
        }

        dataset.LastRun = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        _logger.LogInformation("{DatasetId} last run updated.", datasetId);

        return true;
    }

    private async Task<string> CreateOpenDataPortalDatasetAsync(Dataset dataset)
    {
        _logger.LogInformation("Begins creating Dataset in OpenData for {DatasetId}", dataset.Id);

        var response = await _openDataHttpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/addDataset", UriKind.Relative),
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                api_key = _openDataSettings.OpenDataApiKey,
                data = new
                {
                    locale = "bg",
                    name = dataset.DatasetName,
                    org_id = _openDataSettings.OrganizationId,
                    category_id = _openDataSettings.CategoryId,
                    visibility = 1
                }
            }), Encoding.UTF8, "application/json")
        });

        var responseStr = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("OpenData portal dataset creation for {DatasetId} failed. ({StatusCode}) {Response}", dataset.Id, response.StatusCode.ToString(), responseStr);
            return string.Empty;
        }

        var createDatasetResponse = JsonConvert.DeserializeObject<JObject>(responseStr, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        if (createDatasetResponse is null)
        {
            _logger.LogInformation("Failed getting response from OpenData portal dataset creation for {DatasetId}.", dataset.Id);
            return string.Empty;
        }
        var datasetUri = createDatasetResponse["uri"]?.ToString();
        if (string.IsNullOrWhiteSpace(datasetUri))
        {
            _logger.LogInformation("Failed getting uri from OpenData portal dataset creation response for {DatasetId}.", dataset.Id);
            return string.Empty;
        }
        _logger.LogInformation("Dataset: {DatasetUri} created for {DatasetId}.", datasetUri, dataset.Id);
        return datasetUri;
    }

    private async Task<string> AddResourceMetadataAsync(Dataset dataset)
    {
        if (dataset is null)
        {
            throw new ArgumentNullException(nameof(dataset));
        }

        _logger.LogInformation("Adding resource metadata for {DatasetId} in OpenData", dataset.Id);
        var response = await _openDataHttpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/addResourceMetadata", UriKind.Relative),
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                api_key = _openDataSettings.OpenDataApiKey,
                dataset_uri = dataset.DatasetUri,
                data = new
                {
                    locale = "bg",
                    name = $"{dataset.DatasetName} към {DateTime.UtcNow:MM/yyyy}",
                    type = 1, // файл
                    category_id = _openDataSettings.CategoryId,
                }
            }), Encoding.UTF8, "application/json")
        });

        var responseStr = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Failed adding resource metadata for {DatasetId} in OpenData portal. ({StatusCode}) {Response}", dataset.Id, response.StatusCode.ToString(), responseStr);
            return string.Empty;
        }

        var createdMetadataResponse = JsonConvert.DeserializeObject<JObject>(responseStr, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        var metadataUri = createdMetadataResponse?["data"]?["uri"]?.ToString() ?? string.Empty;
        _logger.LogInformation("Created resource metadata {ResourceMetadataUri} for {DatasetId} in OpenData portal.", metadataUri, dataset.Id);
        return metadataUri;
    }

    private async Task<bool> TryAddResourceDataAsync(string resourceMetadataDataUri, string dataJson)
    {
        if (string.IsNullOrWhiteSpace(resourceMetadataDataUri))
        {
            throw new ArgumentException($"'{nameof(resourceMetadataDataUri)}' cannot be null or whitespace.", nameof(resourceMetadataDataUri));
        }

        if (string.IsNullOrWhiteSpace(dataJson))
        {
            throw new ArgumentException($"'{nameof(dataJson)}' cannot be null or whitespace.", nameof(dataJson));
        }

        _logger.LogInformation("Adding resource data in OpenData portal for {DatasetUri}", resourceMetadataDataUri);
        var response = await _openDataHttpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"api/addResourceData", UriKind.Relative),
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                api_key = _openDataSettings.OpenDataApiKey,
                resource_uri = resourceMetadataDataUri,
                extension_format = "csv",
                data = JsonConvert.DeserializeObject(dataJson)
            }), Encoding.UTF8, "application/json")
        });

        if (!response.IsSuccessStatusCode)
        {
            var responseStr = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Failed adding data to OpenData portal for {DatasetUri}. ({StatusCode}) {Response}", resourceMetadataDataUri, response.StatusCode.ToString(), responseStr);
            return false;
        }

        _logger.LogInformation("Resource data added in OpenData portal for {DatasetUri}", resourceMetadataDataUri);
        return true;
    }
}
