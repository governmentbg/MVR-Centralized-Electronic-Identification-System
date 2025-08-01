#nullable disable
using eID.PDEAU.Contracts.Results;
using Newtonsoft.Json;

namespace eID.PDEAU.Service.Entities;

public class ProviderOffice : IProviderOffice
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public double Lat {  get; set; }
    public double Lon { get; set; }

    // Foreign key
    [JsonIgnore]
    public Guid ProviderId { get; set; }
    [JsonIgnore]
    public virtual Provider Provider { get; set; }
}
#nullable restore
