#nullable disable
namespace eID.PDEAU.Service.Entities;

public class ProviderDoneService
{
    public Guid Id { get; set; }
    
    public Guid ProviderId { get; set; }

    public virtual Provider Provider { get; set; }

    public Guid ServiceId { get; set; }

    public virtual ProviderService Service { get; set; }

    public int Count { get; set; }

    public DateTime CreatedOn { get; set; }
}
#nullable restore
