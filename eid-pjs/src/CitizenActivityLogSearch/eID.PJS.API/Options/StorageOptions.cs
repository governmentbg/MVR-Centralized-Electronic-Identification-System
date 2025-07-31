namespace eID.PJS.API.Options;

public class StorageOptions
{
    public string ExportAuditLogsCsvFilesLocation { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ExportAuditLogsCsvFilesLocation))
        {
            throw new ArgumentNullException(nameof(ExportAuditLogsCsvFilesLocation));
        }
    }
}
