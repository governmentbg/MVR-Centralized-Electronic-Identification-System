using eID.PAN.Contracts.Results;
using Microsoft.EntityFrameworkCore;

namespace eID.PAN.Service.Entities
{
    internal class PaginatedData<T> : IPaginatedData<T>
    {
        public int PageIndex { get; private set; }

        public int TotalItems { get; private set; }

        public IEnumerable<T> Data { get; private set; } = Enumerable.Empty<T>();

        private PaginatedData(int pageIndex, int totalItems, List<T> data)
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
    }
}
