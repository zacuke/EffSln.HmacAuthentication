namespace EffSln.HmacAuthentication.Server;
/// <summary>
/// Configuration options for HMAC authentication.
/// </summary>
public class HmacAuthenticationOptions
{
    /// <summary>
    /// Gets or sets the list of path patterns that should be excluded from HMAC authentication.
    /// </summary>
    public List<string> ExcludedPaths { get; set; } = new();
}