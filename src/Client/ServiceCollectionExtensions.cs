using Microsoft.Extensions.DependencyInjection;

namespace EffSln.HmacAuthentication.Client;

/// <summary>
/// Extension methods for configuring HMAC authentication handler.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the HMAC authentication handler to the HTTP client builder.
    /// </summary>
    /// <param name="builder">The HTTP client builder.</param>
    /// <returns>The HTTP client builder for chaining.</returns>
    public static IHttpClientBuilder AddHmacAuthenticationHandler(this IHttpClientBuilder builder)
    {
        return builder.AddHttpMessageHandler<HmacAuthenticationHandler>();
    }
}