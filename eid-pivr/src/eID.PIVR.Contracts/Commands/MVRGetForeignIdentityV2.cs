using eID.PIVR.Contracts.Enums;
using MassTransit;

namespace eID.PIVR.Contracts.Commands
{
    public interface MVRGetForeignIdentityV2 : CorrelatedBy<Guid>
    {
        public IdentifierType IdentifierType { get; set; }
        public string Identifier { get; set; }
    }
}
