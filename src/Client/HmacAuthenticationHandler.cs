using System.Text;
using Microsoft.Extensions.Options;
using EffSln.HmacAuthentication.Shared;

namespace EffSln.HmacAuthentication.Client;

/// <summary>
/// Handles HMAC authentication for HTTP requests.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="HmacAuthenticationHandler"/> class.
/// </remarks>
/// <param name="options">The HMAC authentication options containing API key and secret.</param>
public class HmacAuthenticationHandler(IOptions<HmacAuthClientOptions> options) : DelegatingHandler
{

    /// <summary>
    /// Sends an HTTP request with HMAC authentication headers.
    /// </summary>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var apiKey = options.Value.ApiKey;
        var secret = options.Value.ApiSecret;

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(secret))
        {
            throw new Exception("API key or secret not configured");
        }

        var timestamp = DateTimeOffset.UtcNow.ToString("o");
        var method = request.Method.Method.ToUpper();
        var path = request.RequestUri?.AbsolutePath ?? "";
        var query = request.RequestUri?.Query ?? "";

        string body = "";
        if (request.Content != null)
        {
            body = await request.Content.ReadAsStringAsync(cancellationToken);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        var signatureData = timestamp + method + path + query + body;
        var signature = HmacSignatureGenerator.GenerateSignature(secret, signatureData);

        request.Headers.Add("X-API-Key", apiKey);
        request.Headers.Add("X-Timestamp", timestamp);
        request.Headers.Add("X-Signature", signature);

        return await base.SendAsync(request, cancellationToken);
    }
}