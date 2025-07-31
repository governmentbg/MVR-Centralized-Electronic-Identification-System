namespace eID.PJS.Contracts.Results;

public interface IPaginatedData<T>
{
    public int PageIndex { get; }
    public int TotalItems { get; }
    public IEnumerable<T> Data { get; }
}
