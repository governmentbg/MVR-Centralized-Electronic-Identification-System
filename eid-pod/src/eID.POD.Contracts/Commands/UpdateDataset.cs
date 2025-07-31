using MassTransit;

namespace eID.POD.Contracts.Commands;

public interface UpdateDataset : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
    public string DatasetName { get; set; }
    public string Description { get; set; }
    public string CronPeriod { get; set; }
    public string DataSource { get; set; }
    public string LastModifiedBy { get; set; }
    public bool IsActive { get; set; }
}
