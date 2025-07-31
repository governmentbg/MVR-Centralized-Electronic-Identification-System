using System.Dynamic;

namespace eID.PIVR.Contracts.Results
{
    public class RegixSearchResultDTO : RegiXSearchResult
    {
        public IDictionary<string, object?> Response { get; set; } = new ExpandoObject();
        public virtual bool HasFailed { get; set; }
        public virtual string? Error { get; set; }
    }
    public interface RegiXSearchResult
    {
        public IDictionary<string, object?> Response { get; set; }
        public bool HasFailed { get; set; }
        public string? Error { get; set; }
    }
}
