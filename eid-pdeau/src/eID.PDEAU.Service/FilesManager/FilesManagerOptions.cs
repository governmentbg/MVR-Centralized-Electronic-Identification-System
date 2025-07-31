namespace eID.PDEAU.Service.FilesManager;

public class FilesManagerOptions
{
    public string UploadDirectoryPath { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(UploadDirectoryPath))
        {
            throw new ArgumentNullException(nameof(UploadDirectoryPath));
        }
    }
}
