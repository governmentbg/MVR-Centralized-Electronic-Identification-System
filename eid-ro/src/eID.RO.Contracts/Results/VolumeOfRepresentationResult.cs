namespace eID.RO.Contracts.Results;

/// <summary>
/// Describe volume of representation
/// </summary>
public interface VolumeOfRepresentationResult
{
    /// <summary>
    /// Code of volume of representation
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Volume of representation description
    /// </summary>
    public string Name { get; set; }
}
