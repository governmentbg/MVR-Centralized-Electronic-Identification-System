using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Database;
using eID.PIVR.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eID.PIVR.Service;

public class OpenDataService : BaseService
{
    private readonly ILogger<OpenDataService> _logger;
    private readonly ApplicationDbContext _context;

    public OpenDataService(
        ILogger<OpenDataService> logger,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceResult<OpenDataResult>> GetApiUsageByYearAsync(GetApiUsageByYear message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetApiUsageByYearValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("{CommandName} validation failed. {Errors}", nameof(GetApiUsageByYear), validationResult);
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

        // Aggregate data
        var aggregatedData = await _context.ApiUsageStats
            .Where(q => q.Date.Year == requestedYear && q.Date.Month <= requestedMonth)
            .AsNoTracking()
            .GroupBy(x => new
            {
                x.RegistryKey,
                x.Date.Year,
                x.Date.Month
            })
            .Select(g => new
            {
                g.Key.RegistryKey,
                g.Key.Year,
                g.Key.Month,
                TotalCount = g.Sum(x => x.Count)
            })
            .OrderBy(r => r.RegistryKey)
            .ToListAsync();
        if (!aggregatedData.Any())
        {
            return NoContent<OpenDataResult>();
        }

        var header = new List<object>
        {
            "Достъп до първичен регистър", "Януари", "Февруари", "Март", "Април", "Май", "Юни", "Юли", "Август",
            "Септември", "Октомври", "Ноември", "Декември"
        };

        var result = new OpenDataResult { { header } };

        // Unwind data. Rows will appear as columns
        var lastRegistryName = "";
        var row = new List<object>();
        foreach (var ad in aggregatedData)
        {
            if (!ad.RegistryKey.Equals(lastRegistryName))
            {
                row = new List<object>
                {
                    ad.RegistryKey
                };

                // Add placeholder for every month
                for (int i = 0; i < 12; i++)
                {
                    row.Add(null);
                }
                ;
                result.Add(row);
                lastRegistryName = ad.RegistryKey;
            }
            // Luckily for us, months' number match the exact index as 1 must be skipped for the name
            row[ad.Month] = ad.TotalCount;
        }

        return Ok(result);
    }

    private class ApiUsagePerMonth
    {
        public string RegistryName { get; set; }
        public int Count { get; set; }
    }
}
