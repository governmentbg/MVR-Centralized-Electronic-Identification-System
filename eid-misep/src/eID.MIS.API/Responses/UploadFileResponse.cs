namespace eID.MIS.API.Responses;

public class UploadFileResponse
{
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
    public string Name { get; set; }
    public string Size { get; set; }
    public string HashAlgorithm { get; set; }
    public string Hash { get; set; }
    public long BlobId { get; set; }
    public string MalwareScanStatus { get; set; }
    public string SignatureStatus { get; set; }
    public string ErrorStatus { get; set; }
}
