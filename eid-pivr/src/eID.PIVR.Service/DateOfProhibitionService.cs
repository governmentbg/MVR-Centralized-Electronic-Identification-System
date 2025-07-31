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

public class DateOfProhibitionService : BaseService
{
    private readonly ILogger<DateOfProhibitionService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly ExternalRegistersCacheOptions _cacheOptions;

    public DateOfProhibitionService(
        ILogger<DateOfProhibitionService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IOptions<ExternalRegistersCacheOptions> cacheOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cacheOptions = (cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions))).Value;
    }

    public async Task<ServiceResult<DateOfProhibitionResult>> GetByPersonalIdAsync(GetDateOfProhibition message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDateOfProhibitionCommandValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<DateOfProhibitionResult>(validationResult.Errors);
        }

        var cacheKey = DateOfProhibition.GetCacheKey(message.PersonalId, message.UidType);
        DateOfProhibitionResult? dateOfProhibitionResult = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var prohibitionResult = await _context.DatesOfProhibition
                .OrderByDescending(p => p.CreatedOn)
                .FirstOrDefaultAsync(p => p.PersonalId.ToLower() == message.PersonalId.ToLower()
                    && p.UidType == message.UidType);
            
            await _cache.SetAsync(cacheKey, prohibitionResult, _cacheOptions.ExpireAfterInHours);
            
            return prohibitionResult;
        });

        // Result
        if (dateOfProhibitionResult is null)
        {
            _logger.LogInformation("{dateOfProhibitionResult} is null", nameof(dateOfProhibitionResult));
            dateOfProhibitionResult = new DateOfProhibition { };
        }

        return Ok(dateOfProhibitionResult);
    }
}
