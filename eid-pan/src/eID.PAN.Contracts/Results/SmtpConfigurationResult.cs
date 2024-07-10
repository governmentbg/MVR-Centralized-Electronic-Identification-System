using eID.PAN.Contracts.Enums;

namespace eID.PAN.Contracts.Results;

public interface SmtpConfigurationResult
{
    public Guid Id { get; set; }
    public string Server { get; set; }
    public int Port { get; set; }
    public SmtpSecurityProtocol SecurityProtocol { get; set; }
    public string UserName { get; set; }

    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
    public string DeletedBy { get; set; }
}
