using eID.PAN.Contracts.Results;
using Microsoft.EntityFrameworkCore;

namespace eID.PAN.Service.Entities
{
    public class NotificationChannelsData<T> : INotificationChannelsData<T>
    {
        public IEnumerable<T> Pending { get; private set; } = Enumerable.Empty<T>();
        public IEnumerable<T> Approved { get; private set; } = Enumerable.Empty<T>();
        public IEnumerable<T> Rejected { get; private set; } = Enumerable.Empty<T>();
        public IEnumerable<T> Archived { get; private set; } = Enumerable.Empty<T>();

        private NotificationChannelsData(List<T> pending, List<T> approved, List<T> rejected, List<T> archive)
        {
            Pending = pending ?? Enumerable.Empty<T>();
            Approved = approved ?? Enumerable.Empty<T>();
            Rejected = rejected ?? Enumerable.Empty<T>();
            Archived = archive ?? Enumerable.Empty<T>();
        }

        public static async Task<INotificationChannelsData<T>> CreateAsync(IQueryable<T> pending, IQueryable<T> approved, IQueryable<T> rejected, IQueryable<T> archive)
        {
            if (pending is null)
            {
                throw new ArgumentNullException(nameof(pending));
            }
            if (approved is null)
            {
                throw new ArgumentNullException(nameof(approved));
            }
            if (rejected is null)
            {
                throw new ArgumentNullException(nameof(rejected));
            }
            if (archive is null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var dataP = await pending.ToListAsync();
            var dataA = await approved.ToListAsync();
            var dataR = await rejected.ToListAsync();
            var dataD = await archive.ToListAsync();

            return new NotificationChannelsData<T>(dataP, dataA, dataR, dataD);
        }
    }
}
