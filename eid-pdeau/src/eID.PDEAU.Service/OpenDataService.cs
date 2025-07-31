using System.Text;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Extensions;
using eID.PDEAU.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eID.PDEAU.Service;

public class OpenDataService : BaseService
{
    private readonly ILogger<OpenDataService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _dbContext;

    public OpenDataService(
        ILogger<OpenDataService> logger,
        IDistributedCache cache,
        ApplicationDbContext dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ServiceResult<OpenDataResult>> GetActiveProvidersAsync()
    {
        // Action
        var result = await _cache.GetOrAddDefAsync(CacheKeyHelper.OpenDataActiveProvidersData, async () =>
            {
                var dbData = await _dbContext.Providers
                    .Include(p => p.Details)
                    .Where(p => p.Status == ProviderStatus.Active)
                    .Select(p => new List<object> { p.Name, p.Address, p.Phone, p.Email,
                        ProcessGeneralInformation(p.GeneralInformation, p.Details.WebSiteUrl, p.Details.WorkingTimeStart, p.Details.WorkingTimeEnd) })
                    .ToListAsync();

                var csvResult = new OpenDataResult
                {
                    new List<object> { "Име", "Адрес", "Телефон", "Имейл", "Обща информация" }
                };
                csvResult.AddRange(dbData);
                return csvResult;
            },
            new OpenDataResult(),
            absoluteExpirationRelativeToNow: TimeSpan.FromHours(1)
        );

        // Result
        return Ok(result);
    }

    private static string ProcessGeneralInformation(string generalInforamation, string webAddress, string workingTimeStart, string workingTimeEnd)
    {
        var giIsEmpty = string.IsNullOrWhiteSpace(generalInforamation);
        var waIsEmpty = string.IsNullOrWhiteSpace(webAddress);
        var wtSIsEmpty = string.IsNullOrWhiteSpace(workingTimeStart);
        var wtEIsEmpty = string.IsNullOrWhiteSpace(workingTimeEnd);

        var sb = new StringBuilder();
        if (!giIsEmpty)
        {
            sb.Append(generalInforamation);

            if (!waIsEmpty || !wtSIsEmpty || !wtEIsEmpty)
            {
                sb.Append(", ");
            }
        }
        if (!waIsEmpty)
        {
            sb.Append("Уеб адрес: ");
            sb.Append(webAddress);

            if (!wtSIsEmpty || !wtEIsEmpty)
            {
                sb.Append(", ");
            }
        }
        if (!wtSIsEmpty || !wtEIsEmpty)
        {
            sb.AppendFormat("Работно време: {0} - {1}", workingTimeStart, workingTimeEnd);
        }

        return sb.ToString();
    }

    public async Task<ServiceResult<OpenDataResult>> GetDoneServicesByYearAsync(GetDoneServicesByYear message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetDoneServicesByYearValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetDoneServicesByYear), validationResult);
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

        var yearResults = new Dictionary<int, DoneServicesPerMonth[]>();
        // Collect months result
        var month = 1;
        while (month <= requestedMonth)
        {
            var cacheKey = $"eID:PDEAU:{nameof(GetDoneServicesByYearAsync)}:{requestedYear}:{month}";

            var monthResult = await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var startDate = new DateTime(requestedYear, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = startDate.AddMonths(1).AddMilliseconds(-1);
                var dbResult = await _dbContext.ProviderDoneServices
                    .Join(_dbContext.Providers,
                        usKey => usKey.ProviderId,
                        pKey => pKey.Id,
                        (us, p) => new { ProviderName = p.Name, us }) // pus.Count, pus.CreatedOn
                    .Join(_dbContext.ProvidersDetailsServices,
                        usKey => usKey.us.ServiceId,
                        psKey => psKey.Id,
                        (us, ps) => new { us.ProviderName, ServiceName = ps.Name, ServiceNo = ps.ServiceNumber, us.us.CreatedOn, us.us.Count })
                    .Where(d => d.CreatedOn >= startDate && d.CreatedOn <= endDate)
                    .GroupBy(d => new { d.ProviderName, d.ServiceName })
                    .OrderBy(ges => ges.Key.ProviderName)
                    .ThenBy(ges => ges.Key.ServiceName)
                    .Select(ges => new DoneServicesPerMonth
                    {
                        ProviderName = ges.Key.ProviderName,
                        ServiceName = ges.Key.ServiceName,
                        ServiceNo = ges.First().ServiceNo,
                        Count = ges.Sum(d => d.Count)
                    })
                    .ToArrayAsync();

                await _cache.SetAsync(cacheKey, dbResult, absoluteExpirationRelativeToNow: TimeSpan.FromDays(550)); // ~1,5 year
                return dbResult;
            })
                ?? Array.Empty<DoneServicesPerMonth>();

            yearResults.Add(month, monthResult);

            month++;
        }

        if (yearResults.All(yr => yr.Value?.Length == 0))
        {
            // There is no data for the period
            return NoContent<OpenDataResult>();
        }

        // Year results to month result
        var aggregatedData = yearResults
            .SelectMany(yr =>
                yr.Value
                    .Select(yrv => new
                    {
                        Month = yr.Key,
                        Data = yrv
                    }))
            .GroupBy(d => new { d.Data.ProviderName, d.Data.ServiceName, d.Month })
            .Select(gr => new { gr.Key.ProviderName, gr.Key.ServiceName, gr.Key.Month, gr.First().Data.ServiceNo, gr.First().Data.Count })
            .OrderBy(d => d.ProviderName)
            .ThenBy(d => d.ServiceName);


        var header = new List<object>
        {
            "Име на доставчик на административни електронни услуги", "Номер", "Услуга",
            "Януари", "Февруари", "Март", "Април", "Май", "Юни", "Юли", "Август", "Септември", "Октомври", "Ноември", "Декември"
        };

        var result = new OpenDataResult { { header } };

        // Unwind data. Rows will appear as columns
        var lastProvider = string.Empty;
        var lastService = string.Empty;
        var row = new List<object>();
        const int padding = 3; // Skip first 3 rows with the service information
        foreach (var ad in aggregatedData)
        {
            if (ad.ProviderName != lastProvider)
            {
                row = new List<object>
                {
                    ad.ProviderName,
                    ad.ServiceNo ?? 0,
                    ad.ServiceName
                };

                // Add placeholder for every month
                for (int i = 0; i < 12; i++)
                {
                    row.Add(null);
                }
                ;
                result.Add(row);
                lastProvider = ad.ProviderName;
                lastService = ad.ServiceName;
            }
            else
            {
                if (ad.ServiceName != lastService)
                {
                    row = new List<object>
                    {
                        null,
                        ad.ServiceNo ?? 0,
                        ad.ServiceName
                    };

                    // Add placeholder for every month
                    for (int i = 0; i < 12; i++)
                    {
                        row.Add(null);
                    }
                    ;
                    result.Add(row);
                    lastService = ad.ServiceName;
                }
            }

            row[ad.Month + padding - 1] = ad.Count;
        }

        return Ok(result);
    }

    private class DoneServicesPerMonth
    {
        public string ProviderName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public long? ServiceNo { get; set; }
        public int Count { get; set; }
    }
}
