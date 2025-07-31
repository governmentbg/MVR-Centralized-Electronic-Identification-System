
using eID.PJS.Services.Entities;
using eID.PJS.Services.Verification;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace eID.PJS.Services.Controllers;

public class ExclusionsController : BaseV1Controller
{
    private ILogger<ManagementController> _logger;
    private readonly IVerificationExclusionProvider _exclusions;

    public ExclusionsController(ILogger<ManagementController> logger, IVerificationExclusionProvider exclusions) : base(logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _exclusions = exclusions ?? throw new ArgumentException(nameof(exclusions));
    }

    /// <summary>
    /// Gets all exclusions asynchronous.</summary>
    /// <returns>
    ///   <br />
    /// </returns>
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllExclusionsAsync()
    {
        var result = await _exclusions.GetAllAsync();

        return Ok(result);
    }

    /// <summary>
    /// Gets all exclusions asynchronous.</summary>
    /// <returns>
    ///   <br />
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetExclusionByIdAsync(Guid id)
    {
        var result = await _exclusions.GetAsync(id);

        return Ok(result);
    }

    /// <summary>Adds the exclusion asynchronous.</summary>
    /// <param name="request">The request.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    [HttpPost("add-path")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> AddPathExclusionAsync([FromBody] FileOrFolderExclusionRequest request)
    {
        if (request == null || !request.IsValid())
        {
            return BadRequest(request);
        }

        var data = new FileORFolderExclusion
        {
            CreatedBy = GetUserId(),
            DateCreated = DateTime.UtcNow,
            ExcludedPath = request.ExcludedPath,
            ReasonForExclusion = request.ReasonForExclusion,
            Id = Guid.NewGuid(),
        };

        var result = await _exclusions.AddAsync(data);

        return Ok(result);
    }

    /// <summary>Adds the exclusion asynchronous.</summary>
    /// <param name="request">The request.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    [HttpPost("add-date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> AddDateExclusionAsync([FromBody] DateRangeExclusionRequest request)
    {
        if (request == null || !request.IsValid())
        {
            return BadRequest(request);
        }

        var data = new DateRangeExclusion
        {
            CreatedBy = GetUserId(),
            DateCreated = DateTime.UtcNow,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ReasonForExclusion = request.ReasonForExclusion,
            Id = Guid.NewGuid(),
        };

        var result = await _exclusions.AddAsync(data);

        return Ok(result);
    }

    [HttpDelete("remove")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> RemoveExclusionAsync([FromBody] Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new ValidationResult
            {
                Errors = new List<ValidationFailure> { new ValidationFailure {
                PropertyName = nameof(id),
                ErrorMessage = "Id is required",
                } }
            });
        }

        var result = await _exclusions.RemoveAsync(id);

        return Ok(result);
    }

    [HttpDelete("remove-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> RemoveAllExclusionAsync()
    {
        var result = await _exclusions.RemoveAllAsync();

        return Ok(result);
    }
}


