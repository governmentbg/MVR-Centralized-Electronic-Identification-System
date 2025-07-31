namespace eID.PJS.Contracts.Results;

/// <summary>
/// This class represent Open Search cursor result
/// </summary>
public class CursorResult<T>
{
    public IEnumerable<object> SearchAfter { get; set; }
    public IEnumerable<T> Data { get; set; }
}
