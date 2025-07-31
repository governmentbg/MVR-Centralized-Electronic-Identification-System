using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaAddAccessToken : CorrelatedBy<Guid>
{
    public string AccessTokenValue { get; set; }
    public DateOnly ExpirationDate { get; set; }
}

