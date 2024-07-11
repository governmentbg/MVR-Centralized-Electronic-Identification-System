using MassTransit;

namespace eID.POD.Contracts.Commands;

public interface CreateDataset : CorrelatedBy<Guid>
{
    public string DatasetName { get; set; }
    public string CronPeriod { get; set; }
    public string DataSource { get; set; }
    public string CreatedBy { get; set; }
    public bool IsActive { get; set; }
}
