namespace EffSln.HmacAuthentication.Server;
/// <summary>
/// Attribute to skip HMAC authentication for specific controllers or methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SkipHmacAuthenticationAttribute : Attribute
{
}