namespace eID.RO.Contracts.Results;

public interface ProviderServiceResult
{
    Guid Id { get; set; }
    long ServiceNumber { get; set; }
    string Name { get; set; }
    string? Description { get; set; }
    decimal? PaymentInfoNormalCost { get; set; }
    /// <summary>
    /// Shows whether or not the service can be empowered
    /// </summary>
    bool IsEmpowerment { get; set; }
    /// <summary>
    /// Mark record if it is added from IISDA
    /// This field is ignored in EF
    /// </summary>
    bool IsExternal { get ; }
    /// <summary>
    /// Soft delete flag
    /// </summary>
    bool IsDeleted { get; set; }
    Guid ProviderDetailsId { get; }
    Guid ProviderSectionId { get; }
}
