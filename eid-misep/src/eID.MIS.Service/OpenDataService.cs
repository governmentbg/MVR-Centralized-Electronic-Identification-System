using eID.MIS.Contracts.Results;
using eID.MIS.Contracts.SEV.Commands;
using eID.MIS.Service.Database;
using eID.MIS.Service.Extensions;
using eID.MIS.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eID.MIS.Service.SEV;

public class OpenDataService : BaseService
{
    private readonly ILogger<OpenDataService> _logger;
    private readonly IDistributedCache _cache;
    private readonly DeliveriesDbContext _context;

    public OpenDataService(
        ILogger<OpenDataService> logger,
        IDistributedCache cache,
        DeliveriesDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceResult<OpenDataResult>> GetDeliveredMessagesByYearAsync(GetDeliveredMessagesByYear message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDeliveredMessagesByYearValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetDeliveredMessagesByYear), validationResult);
            return BadRequest<OpenDataResult>(validationResult.Errors);
        }

        var now = DateTime.UtcNow;
        var requestedMonth = 12;
        var requestedYear = message.Year;
        if (now.Year == requestedYear)
        {
            // When it's January we need be going through
            // the previous year because there is no data for the current month
            if (now.Month == 1)
            {
                requestedYear--;
            }
            else // Otherwise we calculate the data for the requested year until the past month
            {
                requestedMonth = now.Month - 1;
            }
        }

        var yearResults = new SortedDictionary<int, int>();
        // Collect months result
        var month = 1;
        while (month <= requestedMonth)
        {
            var cacheKey = $"eID:MISSEV:{nameof(GetDeliveredMessagesByYearAsync)}:{requestedYear}:{month}";

            var monthResult = await _cache.GetOrCreateAsync(cacheKey, async () =>
            {
                var startDate = new DateTime(requestedYear, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1).AddMilliseconds(-1);
                var dbResult = await _context.Deliveries
                    .Where(d => d.SentOn >= startDate && d.SentOn <= endDate)
                    .CountAsync();

                await _cache.SetAsync(cacheKey, dbResult, absoluteExpirationRelativeToNow: TimeSpan.FromDays(550)); // ~1,5 year

                return dbResult;
            });

            yearResults.Add(month, monthResult);

            month++;
        }

        if (yearResults.All(yr => yr.Value == 0))
        {
            // There is no data for the period
            return NoContent<OpenDataResult>();
        }

        var header = new List<object>
        {
            "Дата","Брой връчени удостоверения за електронна идентичност"
        };

        var result = new OpenDataResult { { header } };

        // Unwind data. Rows will appear as columns
        var row = new List<object>();
        foreach (var ad in yearResults)
        {
            row = new List<object>
            {
                $"{requestedYear}-{ad.Key:00}",
                ad.Value
            };

            result.Add(row);
        }

        return Ok(result);
    }
}
