using MassTransit;

namespace eID.PIVR.Contracts.Commands
{
    public interface MVRGetPersonalIdentityV2 : CorrelatedBy<Guid>
    {
        public string? IdentityDocumentNumber { get; set; }
        public string? EGN { get; set; }
    }
}
