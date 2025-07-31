using eID.Signing.Contracts.Enums;
using eID.Signing.Contracts.Results;

namespace eID.Signing.Service.Entity;

public class AccessToken : AccessTokenResult
{
    public int Id { get; set; }

    public string AccessTokenValue { get; set; } = null!;

    public DateOnly ExpirationDate { get; set; }

    public AccessTokenStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

