namespace eID.RO.Service.Responses;

public class ActualState<T>
{
    public T? Response { get; set; }
    public bool HasFailed { get; set; } = true;

    public object? Error { get; set; }
}
