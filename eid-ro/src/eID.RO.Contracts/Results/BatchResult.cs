using eID.RO.Contracts.Commands;

namespace eID.RO.Contracts.Results;

public interface BatchResult
{
    Guid Id { get; set; }
    string IdentificationNumber { get; set; }
    string Name { get; set; }
    bool IsExternal { get; }
    IISDABatchType Status { get; set; }
}
