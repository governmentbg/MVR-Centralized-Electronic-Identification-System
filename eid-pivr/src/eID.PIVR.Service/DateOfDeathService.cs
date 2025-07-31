using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Database;
using eID.PIVR.Service.Entities;
using eID.PIVR.Service.Extensions;
using eID.PIVR.Service.Options;
using eID.PIVR.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eID.PIVR.Service;

public class DateOfDeathService : BaseService
{
    private readonly ILogger<DateOfDeathService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly ExternalRegistersCacheOptions _cacheOptions;

    public DateOfDeathService(
        ILogger<DateOfDeathService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IOptions<ExternalRegistersCacheOptions> cacheOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cacheOptions = (cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions))).Value;
    }

    public async Task<ServiceResult<DateOfDeathResult>> GetByPersonalIdAsync(GetDateOfDeath message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDateOfDeathCommandValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<DateOfDeathResult>(validationResult.Errors);
        }

        var cacheKey = DateOfDeath.GetCacheKey(message.PersonalId, message.UidType);
        DateOfDeathResult? dateOfDeathResult = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var deathResult = await _context.DatesOfDeath
                .OrderByDescending(d => d.CreatedOn)
                .FirstOrDefaultAsync(d => d.PersonalId.ToLower() == message.PersonalId.ToLower()
                    && d.UidType == message.UidType);

            await _cache.SetAsync(cacheKey, deathResult, _cacheOptions.ExpireAfterInHours);
            return deathResult;
        });

        // Result
        if (dateOfDeathResult is null)
        {
            _logger.LogInformation("{dateOfDeathResult} is null", nameof(dateOfDeathResult));
            dateOfDeathResult = new DateOfDeath { };
        }

        return Ok(dateOfDeathResult);
    }

    public async Task<ServiceResult<IEnumerable<DeceasedByPeriodResult>>> GetDeceasedByPeriodAsync(GetDeceasedByPeriod message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDeceasedByPeriodValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<DeceasedByPeriodResult>>(validationResult.Errors);
        }
        var from = message.From.ToUniversalTime().Date;
        var to = message.To.ToUniversalTime().Date.AddDays(1).AddMilliseconds(-1);
        // Execute
        IEnumerable<DeceasedByPeriodResult> deceasedByPeriodResult = await _context.DatesOfDeath
            .Where(d => d.CreatedOn >= from && d.CreatedOn <= to && d.Date != null)
            .OrderByDescending(d => d.CreatedOn)
            .ToArrayAsync();

        // If there are duplicated PersonalId, here we get the latest result
        var result = deceasedByPeriodResult
            .DistinctBy(d => d.PersonalId);

        // Result
        return Ok(result);
    }
}
