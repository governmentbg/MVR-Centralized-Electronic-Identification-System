using eID.PAN.Contracts.Enums;
using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface UpdateSmtpConfiguration : CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
    public string Server { get; set; }
    public int? Port { get; set; }
    public SmtpSecurityProtocol SecurityProtocol { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public string UserId { get; set; }
}
