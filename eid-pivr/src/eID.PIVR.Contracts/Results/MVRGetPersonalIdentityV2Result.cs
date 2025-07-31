namespace eID.PIVR.Contracts.Results
{
    public interface MVRGetPersonalIdentityV2Result
    {
        public dynamic Response { get; set; }
        public bool HasFailed { get; set; }
        public string? Error { get; set; }
    }
}
