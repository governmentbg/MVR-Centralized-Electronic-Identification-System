namespace eID.PAN.Service.Entities;

public class DeactivatedUserEvent
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid SystemEventId { get; set; }
    /// <summary>
    /// Foreign key
    /// </summary>
    public SystemEvent Event { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
}
