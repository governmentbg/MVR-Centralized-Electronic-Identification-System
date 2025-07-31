namespace eID.MIS.Contracts.SEV.External;

public class SearchUserProfileQuery
{
    public Guid EIdentityId { get; set; }
    public string Identifier { get; set; }
    public string TargetGroupId { get; set; }
}
