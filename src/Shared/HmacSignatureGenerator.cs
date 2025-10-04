using System.Security.Cryptography;
using System.Text;

namespace EffSln.HmacAuthentication.Shared;

/// <summary>
/// Generates HMAC signatures for authentication.
/// </summary>
public static class HmacSignatureGenerator
{
    /// <summary>
    /// Generates an HMAC signature using the provided secret and data.
    /// </summary>
    /// <param name="secret">The secret key for HMAC computation.</param>
    /// <param name="data">The data to sign.</param>
    /// <returns>The base64-encoded HMAC signature.</returns>
    public static string GenerateSignature(string secret, string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }
}