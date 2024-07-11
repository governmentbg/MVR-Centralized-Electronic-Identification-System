using eID.RO.Service.Interfaces;

namespace eID.RO.Service;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
