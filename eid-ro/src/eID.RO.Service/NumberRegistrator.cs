using eID.RO.Service.Database;
using eID.RO.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eID.RO.Service;

public interface INumberRegistrator
{
    Task<int> GetEmpowermentNextNumberAsync(DateTime currentDate);
}

public class NumberRegistrator : INumberRegistrator
{
    private readonly ILogger<NumberRegistrator> _logger;
    private readonly ApplicationDbContext _context;

    public NumberRegistrator(
        ILogger<NumberRegistrator> logger,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> GetEmpowermentNextNumberAsync(DateTime currentDate)
    {
        var dateAsString = currentDate.ToString("yyyyMMdd");
        // Update LastChange and Current + 1 and return Current if: the year is the same as date now
        // Update LastChange and Current = 1 and return Current if: the year is not the same as date now
        var sql =
            "WITH updated AS (" +
                $"UPDATE \"{NumberRegister.TableName}\" " +
                $"SET \"{nameof(NumberRegister.Current)}\" = CASE " +
                    $"WHEN EXTRACT(YEAR FROM \"{nameof(NumberRegister.LastChange)}\") = EXTRACT(YEAR FROM TO_DATE('{dateAsString}','YYYYMMDD')) " +
                        $"THEN \"{nameof(NumberRegister.Current)}\" + 1 " +
                    "ELSE 1 " +
                "END, " +
                $"\"{nameof(NumberRegister.LastChange)}\" = TO_DATE('{dateAsString}','YYYYMMDD') " +
                $"WHERE \"{nameof(NumberRegister.Id)}\" = '{NumberRegister.EmpowermentNumberId}' " +
                $"RETURNING \"{nameof(NumberRegister.Current)}\" " +
            ") " +
            $"SELECT \"{nameof(NumberRegister.Current)}\" FROM updated";

        var dbResult = await _context.Database
                        .SqlQueryRaw<int>(sql)
                        .ToArrayAsync();

        return dbResult.FirstOrDefault();
    }
}
