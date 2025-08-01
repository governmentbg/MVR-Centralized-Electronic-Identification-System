using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Service.Entities;

public class AdministratorPromotion
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public Guid IssuerId { get; set; }
    public Guid PromotedUserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public AdministratorPromotionStatus Status { get; set; }
}
