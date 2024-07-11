using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using Newtonsoft.Json;

namespace eID.RO.Service.Entities;

public class StatusHistoryRecord : StatusHistoryResult
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Guid EmpowermentStatementId { get; set; }
    public DateTime DateTime { get; set; }

    public EmpowermentStatementStatus Status { get; set; }
    [JsonIgnore]
    public EmpowermentStatement EmpowermentStatement { get; set; }
}
