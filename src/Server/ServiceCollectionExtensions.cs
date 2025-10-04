using Microsoft.Extensions.DependencyInjection;

namespace EffSln.HmacAuthentication.Server;

/// <summary>
/// Extension methods for configuring HMAC authentication services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds HMAC authentication services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional configuration action for HMAC authentication options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddHmacAuthentication(this IServiceCollection services, Action<HmacAuthenticationOptions>? configureOptions = null)
    {
        var options = new HmacAuthenticationOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton(options);
        return services;
    }
}