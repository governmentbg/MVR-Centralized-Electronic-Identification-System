using MassTransit;

namespace eID.MIS.Contracts.EP.Commands;

public interface GetClientsByEik : CorrelatedBy<Guid>
{
    public string Eik { get; set; }
}
