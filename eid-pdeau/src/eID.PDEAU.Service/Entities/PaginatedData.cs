using eID.PDEAU.Contracts.Results;
using Microsoft.EntityFrameworkCore;

namespace eID.PDEAU.Service.Entities;
internal class PaginatedData<T> : IPaginatedData<T>
{
    public int PageIndex { get; private set; }

    public int TotalItems { get; private set; }

    public IEnumerable<T> Data { get; private set; } = Enumerable.Empty<T>();

    public PaginatedData(int pageIndex, int totalItems, IEnumerable<T> data)
    {
        PageIndex = pageIndex;
        TotalItems = totalItems;
        Data = data ?? Enumerable.Empty<T>();
    }

    public static async Task<IPaginatedData<T>> CreateAsync(IOrderedQueryable<T> source, int pageIndex, int pageSize)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var totalItems = await source.CountAsync();
        var data = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedData<T>(pageIndex, totalItems, data);
    }

    public static async Task<IPaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var totalItems = await source.CountAsync();
        var data = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedData<T>(pageIndex, totalItems, data);
    }

    public static async Task<PaginatedData<T>> CreatePaginatedDataAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var totalItems = await source.CountAsync();
        var data = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedData<T>(pageIndex, totalItems, data);
    }

    public static IPaginatedData<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var totalItems = source.Count();
        var data = source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        return new PaginatedData<T>(pageIndex, totalItems, data);
    }

    public static IPaginatedData<T> Create<TSource>(IPaginatedData<TSource> source)
        where TSource : class, T
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new PaginatedData<T>(source.PageIndex, source.TotalItems, source.Data);
    }

    public static IPaginatedData<T> Empty(int pageIndex)
    {
        return new PaginatedData<T>(pageIndex, 0, Enumerable.Empty<T>());
    }

    public static PaginatedData<T> EmptyPaginatedData(int pageIndex)
    {
        return new PaginatedData<T>(pageIndex, 0, Enumerable.Empty<T>());
    }
}
