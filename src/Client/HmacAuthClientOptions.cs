namespace EffSln.HmacAuthentication.Client;

/// <summary>
/// Configuration options for HMAC authentication client.
/// </summary>
public class HmacAuthClientOptions
{
    /// <summary>
    /// Gets or sets the API key for HMAC authentication.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API secret for HMAC authentication.
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;
}