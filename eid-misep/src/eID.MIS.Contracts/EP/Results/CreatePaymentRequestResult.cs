namespace eID.MIS.Contracts.EP.Results;

public interface CreatePaymentRequestResult
{
    public Guid Id { get; set; }
    public DateTime PaymentDeadline { get; set; }
    public string AccessCode { get; set; }
}
