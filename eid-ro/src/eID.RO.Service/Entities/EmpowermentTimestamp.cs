using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class EmpowermentTimestamp
{
    public Guid Id { get; set; }
    /// <summary>
    /// Timestamping datetime (UTC)
    /// </summary>
    public DateTime DateTime { get; set; }
    /// <summary>
    /// Data storing timestamp information (Base64)
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// EmpowermentStatement FK
    /// </summary>
    [JsonIgnore]
    public EmpowermentStatement EmpowermentStatement { get; set; } = new EmpowermentStatement();
    [JsonIgnore]
    public Guid EmpowermentStatementId { get; set; }
}
