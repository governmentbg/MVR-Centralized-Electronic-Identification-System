using eID.Signing.Contracts.Enums;

namespace eID.Signing.Contracts.Results;

public interface AccessTokenResult
{
    public int Id { get; set; }

    public string AccessTokenValue { get; set; }
    public AccessTokenStatus Status { get; set; }

    public DateOnly ExpirationDate { get; set; }


    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
