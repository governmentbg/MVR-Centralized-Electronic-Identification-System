namespace eID.PJS.API.Options;

public class AesOptions
{
    public string Key { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
        {
            throw new ArgumentNullException(nameof(Key));
        }
    }
}
