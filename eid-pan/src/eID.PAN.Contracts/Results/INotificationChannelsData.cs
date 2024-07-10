namespace eID.PAN.Contracts.Results;
public interface INotificationChannelsData<T>
{
    public IEnumerable<T> Approved { get; }
    public IEnumerable<T> Archived { get; }
    public IEnumerable<T> Pending { get; }
    public IEnumerable<T> Rejected { get; }
}

