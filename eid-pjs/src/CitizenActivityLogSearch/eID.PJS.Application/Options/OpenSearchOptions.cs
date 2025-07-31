namespace eID.PJS.Application.Options;

public class OpenSearchOptions
{
    public string[] NodeUrls { get; set; } = Array.Empty<string>();

    public OpenSearchBasicAuthentication BasicAuthentiction { get; set; } = new OpenSearchBasicAuthentication();

    /// <summary>
    /// Default index. It supports wildcards
    /// </summary>
    public string Index { get; set; } = string.Empty;

    /// <summary>
    /// Maximum retries. Optional. Default 5
    /// </summary>
    public int MaximumRetries { get; set; } = 5;

    /// <summary>
    /// Request timeout in seconds. Optional. Default 60
    /// </summary>
    public int RequestTimeoutInSeconds { get; set; } = 60;

    /// <summary>
    /// Max retry timeout in seconds. Optional. Default 60
    /// </summary>
    public int MaxRetryTimeoutInSeconds { get; set; } = 60;

    public void Validate()
    {
        if (NodeUrls == null)
        {
            throw new ArgumentNullException(nameof(NodeUrls));
        }

        if (NodeUrls.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(NodeUrls));
        }

        foreach (var url in NodeUrls) 
        {
            var test = new Uri(url);
        }

        if (Index == null)
        {
            throw new ArgumentNullException(nameof(Index));
        }

        if (BasicAuthentiction == null)
        {
            throw new ArgumentNullException(nameof(BasicAuthentiction));
        }

        BasicAuthentiction.Validate();
    }
}

public class OpenSearchBasicAuthentication
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            throw new ArgumentNullException(nameof(Username));
        }

        if (string.IsNullOrWhiteSpace(Password)) 
        { 
            throw new ArgumentNullException(nameof(Password));
        }
    }
}
