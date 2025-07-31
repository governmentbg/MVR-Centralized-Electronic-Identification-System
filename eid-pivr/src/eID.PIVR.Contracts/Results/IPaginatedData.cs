namespace eID.PIVR.Contracts.Results;

public interface IPaginatedData<out T>
{
    public int PageIndex { get; }
    public int TotalItems { get; }
    public IEnumerable<T> Data { get; }
}
