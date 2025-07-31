namespace eID.PJS.API.Public.Options;

public class FilterOptions
{
    /// <summary>
    /// Describe excluded event types. Optional
    /// </summary>
    public List<string> ExcludedEventTypes { get; set; } = new List<string>();
}
