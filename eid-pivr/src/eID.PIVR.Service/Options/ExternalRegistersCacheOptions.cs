namespace eID.PIVR.Service.Options;


public class ExternalRegistersCacheOptions
{
    /// <summary>
    /// Cached register information will be expired after X hours or in midnight.
    /// </summary>
    public int ExpireAfterInHours { get; set; } = 0;
}
