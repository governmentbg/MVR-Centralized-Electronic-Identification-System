using MassTransit;

namespace eID.PIVR.Contracts.Commands
{
    public interface RegiXSearchCommand : CorrelatedBy<Guid>
    {
        public string Command { get; set; }
        public double CacheTimeInMinutes { get; set; }
    }
}
