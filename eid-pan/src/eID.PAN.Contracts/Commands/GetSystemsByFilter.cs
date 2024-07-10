using eID.PAN.Contracts.Enums;
using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetSystemsByFilter : CorrelatedBy<Guid>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public RegisteredSystemState RegisteredSystemState { get; set; }
}
