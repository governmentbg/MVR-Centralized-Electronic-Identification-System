#nullable disable
using eID.PAN.Contracts.Enums;
using eID.PAN.Contracts.Results;

namespace eID.PAN.Service.Entities;

public class SmtpConfiguration : IAuditableEntity, SmtpConfigurationResult
{
    public Guid Id { get; set; }
    public string Server { get; set; }
    public int Port { get; set; }
    public SmtpSecurityProtocol SecurityProtocol { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public DateTime? CreatedOn { get; set; }
    public string CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }

    public DateTime? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }

    DateTime SmtpConfigurationResult.CreatedOn { get => CreatedOn.GetValueOrDefault(); set => throw new NotImplementedException(); }

    private static string _cacheBaseKey = "eID:PAN:SmtpConfiguration";
    public static string BuildSmtpConfigurationCacheKey(Guid id) => $"{_cacheBaseKey}:{id}";
    public static string AllSmtpConfigurationCacheKey = $"{_cacheBaseKey}:All";
}
#nullable restore
