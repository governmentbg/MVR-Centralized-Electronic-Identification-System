namespace eID.PDEAU.API.Public.Responses;

public class ProviderRegistrationResponse
{
    public bool RegistrationSucceeded { get; set; }
    public bool AllFilesUploadSucceeded { get; set; }
    public Dictionary<string, bool> FileUploads { get; set; }
}
