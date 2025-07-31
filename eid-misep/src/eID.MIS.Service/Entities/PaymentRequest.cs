using eID.MIS.Contracts.EP.Results;

namespace eID.MIS.Service.Entities;

public class PaymentRequest : PaymentRequestResult
{
    public static readonly int PaymentDeadlineLimitInDays = 3;
    public Guid Id { get; set; }
    public string EPaymentId { get; set; }
    public Guid CitizenProfileId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime PaymentDeadline { get; set; }
    public DateTime? PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string AccessCode { get; set; }
    public DateTime RegistrationTime { get; set; }
    public string InitiatorSystemName { get; set; }
    public string ReferenceNumber { get; set; }
    public string Reason { get; set; }
    public string Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime LastSync { get; set; }
}
