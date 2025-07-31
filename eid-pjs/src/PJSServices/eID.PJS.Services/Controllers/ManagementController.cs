using System.Security.Cryptography.X509Certificates;
using eID.PJS.Services.Signing;
using eID.PJS.Services.TimeStamp;
using eID.PJS.Services.Verification;
using Microsoft.AspNetCore.Mvc;

namespace eID.PJS.Services.Controllers;

public class ManagementController : BaseV1Controller
{
    private ILogger<ManagementController> _logger;
    private GlobalStatus _status;
    private VerificationService _service;
    private ILoggerFactory _loggerFactory;
    private readonly ICommandStateProvider<VerificationServiceStateShort> _commandStateProvider;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagementController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="status">The status.</param>
    /// <param name="service">The service.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="commandStateProvider">The command state provider.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <exception cref="System.ArgumentNullException">
    /// logger
    /// or
    /// status
    /// or
    /// service
    /// or
    /// loggerFactory
    /// or
    /// serviceProvider
    /// </exception>
    /// <exception cref="System.ArgumentException">commandStateProvider</exception>
    public ManagementController(ILogger<ManagementController> logger,
        GlobalStatus status,
        VerificationService service,
        ILoggerFactory loggerFactory,
        ICommandStateProvider<VerificationServiceStateShort> commandStateProvider,
        IServiceProvider serviceProvider
        ) : base(logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _status = status ?? throw new ArgumentNullException(nameof(status));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _commandStateProvider = commandStateProvider ?? throw new ArgumentException(nameof(commandStateProvider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); ;
    }


    [HttpGet("verification-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetVerificationStatusAsync()
    {

        return await Task.FromResult(StatusCode(StatusCodes.Status200OK, _status.VerificationServiceStatus ));
    }

    [HttpGet("signing-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetSigningStatusAsync()
    {

        var signStatus = new ServiceStatusBase<SigningServiceStateRecord>();

        signStatus.CurrentStatus = _status.SigningServiceStatus.CurrentStatus;
        signStatus.ServiceName = _status.SigningServiceStatus.ServiceName;
        signStatus.LastProcessingStart = _status.SigningServiceStatus.LastProcessingStart;


        if (_status.SigningServiceStatus.LastState != null)
        {
            signStatus.LastState = new SigningServiceStateRecord
            {
                NumErrors = _status.SigningServiceStatus.LastState.NumErrors,
                HasErrors = _status.SigningServiceStatus.LastState.HasErrors,
                NumberOfThreadsUsed = _status.SigningServiceStatus.LastState.NumberOfThreadsUsed,
                NumProcessedFiles = _status.SigningServiceStatus.LastState.NumProcessedFiles,
                NumSkippedFiles = _status.SigningServiceStatus.LastState.NumSkippedFiles,
                Metrics = _status.SigningServiceStatus.LastState.Metrics,
            };

            if (_status.SigningServiceStatus.LastState.HasErrors)
            {
                CloneMonitoredFolders(_status.SigningServiceStatus.LastState, signStatus);
            }

        }

        return await Task.FromResult(StatusCode(StatusCodes.Status200OK, signStatus));
    }


    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetFullStatusAsync()
    {
        return await Task.FromResult(StatusCode(StatusCodes.Status200OK, _status));
    }


    [HttpPost("verify/file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> VerifyFileAsync([FromBody] VerifyFileRequest request)
    {
        var errors = request.Validate();
        if (!errors.IsValid)
        {
            return BadRequest(errors);
        }

        var tsProvider = _serviceProvider.GetRequiredService<ITimeStampProvider>();
        var tsCertChain = _serviceProvider.GetRequiredService<X509Certificate2>();

        return await Task.FromResult(Ok(_service.VerifyFile(request.FileName, new Exclusions(), tsProvider, tsCertChain)));
    }


    [HttpPost("verify/period")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> VerifyPeriodAsync([FromBody] VerifyPeriodRequest request)
    {
        var errors = request.Validate();
        if (!errors.IsValid)
        {
            return BadRequest(errors);
        }

        var cmd = new VerifyPeriodCommand(_commandStateProvider, _service, request, _loggerFactory.CreateLogger<VerifyPeriodCommand>());

        return Ok(await cmd.ExecuteAsync());
    }

    [HttpGet("verify/all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> VerifyAllAsync()
    {
        var cmd = new VerifyAllCommand(_commandStateProvider, _service, _loggerFactory.CreateLogger<VerifyAllCommand>());

        cmd.TaskFinished += (sender, e) =>
        {
            if (e.Error != null)
            {
                _logger.LogInformation($"Task {e.TaskId} encountered an error: {e.Error.Message}");
            }
            else
            {
                _logger.LogInformation($"Task {e.TaskId} finished with result: {e.Result}");
            }
        };

        return Ok(await cmd.ExecuteAsync());
    }


    [HttpGet("verify/result")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetVerifyResultAsync(Guid taskId)
    {
        if (taskId == Guid.Empty)
            return BadRequest();

        var result = _commandStateProvider.GetResult(taskId);

        if (result == null)
            return NotFound();

        return await Task.FromResult(Ok(result));
    }

    private void CloneMonitoredFolders(SigningServiceState state, ServiceStatusBase<SigningServiceStateRecord> signStatus)
    {
        foreach (var item in state.Folders)
        {
            if (item.HasErrors)
            {
                var folder = new MonitoredFolder();
                foreach (var file in item.Files)
                {
                    if (file.Status == MonitoredResourceStatus.Error)
                    {
                        folder.Files.Add(file);
                    }
                }

                signStatus!.LastState!.Folders.Add(folder);
            }
        }
    }

    private List<SystemVerificationResult> CloneErrorSystems(List<SystemVerificationResult> items)
    {
        var result = new List<SystemVerificationResult>();

        foreach (var item in items)
        {
            if (!item.IsValid)
            {
                var newSystem = new SystemVerificationResult
                {
                    IndexName = item.IndexName,
                    LocalLogsLocation = item.LocalLogsLocation,
                    Status = item.Status,
                    SystemId = item.SystemId,
                };

                item.Files.Where(s => s.IsValid == false).ToList().ForEach(f => newSystem.Files.Add(f));

                newSystem.Errors.AddRange(item.Errors);
                result.Add(newSystem);
            }
        }

        return result;
    }

    private const long DirectFileDownloadThreshold = 20_000_000;

    /// <summary>
    /// Downloads the file.
    /// Format of the Range header: "Range: bytes=start-end" e.g. to get bytes from 100 to 199: "Range: bytes=100-199"
    /// Requesting a Specific Range:
    /// 1. To request bytes 100 to 199, you would use: Range: bytes=100-199. This requests a block of 100 bytes from the 101st byte to the 200th byte of the file.
    /// Open-Ended Ranges:
    /// 2. To request all bytes from byte 100 to the end of the file: Range: bytes=100-. 
    /// This does not specify an end range, so the server should send bytes from 100 to the end of the file.
    /// To request the last 50 bytes of a file: Range: bytes=-50. This is useful for getting the end of a file, especially if you don't know the total file size.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns></returns>
    [HttpPost("verify/result-file")]
    public IActionResult DownloadFile([FromBody] string filePath)
    {
        try
        {
            // Validate and locate the file
            var file = new FileInfo(Path.GetFullPath(filePath));
            if (!file.Exists)
            {
                _logger.LogDebug(Path.GetFullPath(filePath));
                return NotFound(Path.GetFullPath(filePath));
            }

            // Check if file size is below the direct download threshold
            if (file.Length <= DirectFileDownloadThreshold)
            {
                return PhysicalFile(file.FullName, "application/octet-stream", file.Name);
            }
            else
            { 
                return PhysicalFile(file.FullName, "application/octet-stream", file.Name, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(Path.GetFullPath(filePath));
            return BadRequest(ex);
        }

    }
}
