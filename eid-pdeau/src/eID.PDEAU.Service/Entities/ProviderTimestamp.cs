#nullable disable
namespace eID.PDEAU.Service.Entities;

public class ProviderTimestamp
{
    // Foreign key
    public Guid ProviderId { get; set; }
    public Provider Provider { get; set; }

    public string Signature { get; set; }
    public DateTime DateTime { get; set; }
}
#nullable restore
