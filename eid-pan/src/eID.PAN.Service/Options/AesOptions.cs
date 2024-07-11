namespace eID.PAN.Service.Options;

public class AesOptions
{
    public string? Key { get; set; }
    public string? Vector { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new ArgumentNullException(nameof(Key));
        }

        if (string.IsNullOrWhiteSpace(Vector))
        {
            throw new ArgumentNullException(nameof(Vector));
        }
    }
}
