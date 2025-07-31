namespace eID.MIS.Contracts.EP.Results;
public interface PaymentRequestResult
{
    public string EPaymentId { get; set; }
    public Guid CitizenProfileId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime PaymentDeadline { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string AccessCode { get; set; }
    public DateTime RegistrationTime { get; set; }
    public string ReferenceNumber { get; set; }
    public string Reason { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastSync { get; set; }
}

public enum PaymentStatus
{
    None = 0,
    Pending = 1,
    TimedOut = 2,
    Paid = 3,
}
