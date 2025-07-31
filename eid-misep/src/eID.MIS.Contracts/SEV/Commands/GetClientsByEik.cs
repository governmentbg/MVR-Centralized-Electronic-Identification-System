using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface GetClientsByEik : CorrelatedBy<Guid>
{
    public string Eik { get; set; }
}
