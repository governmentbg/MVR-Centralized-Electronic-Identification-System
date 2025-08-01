using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface ConfirmAdminPromotion : CorrelatedBy<Guid>
{
    public Guid AdministratorPromotionId { get; set; }
}
