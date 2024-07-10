using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using eID.RO.Service.Database;
using eID.RO.Service.Extensions;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eID.RO.Service;

public class OpenDataService : BaseService
{
    private readonly ILogger<OpenDataService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public OpenDataService(
        ILogger<OpenDataService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<ServiceResult<OpenDataResult>> GetActivatedEmpowermentsByYearAsync(GetActivatedEmpowermentsByYear message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetActivatedEmpowermentsByYearValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetActivatedEmpowermentsByYear), validationResult);
            return BadRequest<OpenDataResult>(validationResult.Errors);
        }

        var now = _dateTimeProvider.UtcNow;
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

        var yearResults = new Dictionary<int, EmpowermentsPerMonth[]>();
        // Collect months result
        var month = 1;
        while (month <= requestedMonth)
        {
            var cacheKey = $"eID:RO:{nameof(GetActivatedEmpowermentsByYearAsync)}:{requestedYear}:{month}";

            var monthResult = await _cache.GetOrCreateAsync(cacheKey, async () =>
            {
                var startDate = new DateTime(requestedYear, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1).AddMilliseconds(-1);
                var dbResult = await _context.EmpowermentStatements
                    .Join(_context.EmpowermentStatusHistory,
                        esKey => esKey.Id,
                        shKey => shKey.EmpowermentStatementId,
                        (e, s) => new { e.ServiceId, e.ServiceName, s.Status, s.DateTime })
                    .Where(sh => sh.Status == Contracts.Enums.EmpowermentStatementStatus.Active
                            && sh.DateTime >= startDate && sh.DateTime <= endDate)
                    .GroupBy(es => new { es.ServiceId, es.ServiceName })
                    .OrderBy(ges => ges.Key.ServiceId)
                    .Select(ges => new EmpowermentsPerMonth
                    {
                        ServiceId = ges.Key.ServiceId,
                        ServiceName = ges.Key.ServiceName,
                        Count = ges.Count()
                    })
                    .ToArrayAsync();

                await _cache.SetAsync(cacheKey, dbResult, absoluteExpirationRelativeToNow: TimeSpan.FromDays(550)); // ~1,5 year

                return dbResult;
            })
                ?? Array.Empty<EmpowermentsPerMonth>();

            yearResults.Add(month, monthResult);

            month++;
        }

        if (yearResults.All(yr => yr.Value?.Length == 0))
        {
            // There is no data for the period
            return NoContent<OpenDataResult>();
        }

        // Aggregate data
        var aggregatedData = yearResults
            .SelectMany(yr =>
                yr.Value
                    .Select(yrv => new
                    {
                        Month = yr.Key,
                        yrv.ServiceId,
                        yrv.ServiceName,
                        yrv.Count
                    }))
            .GroupBy(yrv => new { yrv.ServiceId, yrv.ServiceName, yrv.Month })
            .Select(gr => new { gr.Key.ServiceId, gr.Key.ServiceName, gr.Key.Month, Count = gr.Sum(v => v.Count) })
            .OrderBy(r => r.ServiceId);

        var header = new List<object>
        {
            "Номер", "Услуга", "Януари", "Февруари", "Март", "Април", "Май", "Юни", "Юли", "Август",
            "Септември", "Октомври", "Ноември", "Декември"
        };

        var result = new OpenDataResult { { header } };

        // Unwind data. Rows will appear as columns
        var lastServiceId = (int.MinValue, "");
        var row = new List<object>();
        const int padding = 2; // Skip first 2 rows with the service information
        foreach (var ad in aggregatedData)
        {
            if ((ad.ServiceId, ad.ServiceName) != lastServiceId)
            {
                row = new List<object>
                {
                    ad.ServiceId,
                    ad.ServiceName
                };

                // Add placeholder for every month
                for (int i = 0; i < 12; i++)
                {
                    row.Add(null);
                };
                result.Add(row);
                lastServiceId = (ad.ServiceId, ad.ServiceName);
            }

            row[ad.Month + padding - 1] = ad.Count;
        }

        return Ok(result);
    }

    private class EmpowermentsPerMonth
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
