namespace eID.PIVR.Contracts.Results
{
    public interface MVRGetForeignIdentityV2Result
    {
        public dynamic Response { get; set; }
        public bool HasFailed { get; set; }
        public string? Error { get; set; }
    }
}
