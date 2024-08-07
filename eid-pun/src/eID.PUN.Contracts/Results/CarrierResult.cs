﻿namespace eID.PUN.Contracts.Results;

public interface CarrierResult
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; }
    public string Type { get; set; }
    public Guid CertificateId { get; set; }
    public Guid EId { get; set; }
    public Guid UserId { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
}
