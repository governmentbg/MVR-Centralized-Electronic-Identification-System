namespace eID.PAN.Contracts.Results;
public interface INotificationChannelsData<T, TRejected>
{
    public IEnumerable<T> Approved { get; }
    public IEnumerable<T> Archived { get; }
    public IEnumerable<T> Pending { get; }
    public IEnumerable<TRejected> Rejected { get; }
}

