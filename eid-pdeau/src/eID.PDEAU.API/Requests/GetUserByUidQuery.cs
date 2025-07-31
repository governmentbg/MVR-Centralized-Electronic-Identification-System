using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.API.Requests;

public class GetUserByUidQuery
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
}
