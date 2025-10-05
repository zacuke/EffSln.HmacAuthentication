using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
    /// <param name="configureOptions">Optional configuration action for HMAC authentication options.</param>
    /// <returns>The HTTP client builder for chaining.</returns>
    public static IHttpClientBuilder AddHmacAuthenticationHandler(this IHttpClientBuilder builder, Action<HmacAuthClientOptions>? configureOptions = null)
    {
        // builder.Services.AddOptions<HmacAuthClientOptions>();

        // if (configureOptions != null)
        // {
        //     builder.Services.Configure(configureOptions);
        // }

        builder.Services.AddTransient<HmacAuthenticationHandler>();

        return builder.AddHttpMessageHandler<HmacAuthenticationHandler>();
    }
}