using eID.POD.Contracts.Results;

namespace eID.POD.Service.Entities;

public class Dataset : DatasetResult
{
    public Guid Id { get; set; }
    public string DatasetName { get; set; }
    public string CronPeriod { get; set; } 
    public string DataSource { get; set; }
    public string DatasetUri { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? LastRun { get; set; }
    public string CreatedBy { get; set; }
    public string LastModifiedBy { get; set; }
}
