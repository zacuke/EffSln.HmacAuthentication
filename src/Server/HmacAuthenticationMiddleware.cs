using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using EffSln.HmacAuthentication.Shared;
namespace EffSln.HmacAuthentication.Server;
/// <summary>
/// Middleware for HMAC authentication that validates requests using API keys and signatures.
/// </summary>
public class HmacAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HmacAuthenticationMiddleware> _logger;
    private readonly HmacAuthenticationOptions _options;

    /// <summary>
    /// Initializes a new instance of the HmacAuthenticationMiddleware class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="configuration">The configuration containing API keys.</param>
    /// <param name="logger">The logger for diagnostic messages.</param>
    /// <param name="options">The HMAC authentication options.</param>
    public HmacAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<HmacAuthenticationMiddleware> logger, HmacAuthenticationOptions options)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
        _options = options;
    }

    /// <summary>
    /// Processes the HTTP request by validating HMAC authentication.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<SkipHmacAuthenticationAttribute>() != null)
        {
            await _next(context);
            return;
        }

        if (_options.ExcludedPaths.Any(path => context.Request.Path.StartsWithSegments(path)))
        {
            await _next(context);
            return;
        }

        if (!await ValidateHmacSignature(context))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid HMAC signature");
            return;
        }

        await _next(context);
    }

    private async Task<bool> ValidateHmacSignature(HttpContext context)
    {
        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        var timestamp = context.Request.Headers["X-Timestamp"].FirstOrDefault();
        var signature = context.Request.Headers["X-Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Missing authentication headers");
            return false;
        }

        if (!ValidateTimestamp(timestamp))
        {
            _logger.LogWarning("Invalid timestamp: {Timestamp}", timestamp);
            return false;
        }

        var secret = _configuration.GetValue<string>($"ApiKeys:{apiKey}");
        if (string.IsNullOrEmpty(secret))
        {
            _logger.LogWarning("Invalid API key: {ApiKey}", apiKey);
            return false;
        }

        var computedSignature = await ComputeSignature(context, timestamp, secret);
        if (!string.Equals(signature, computedSignature, StringComparison.Ordinal))
        {
            _logger.LogWarning("Signature mismatch for API key: {ApiKey}", apiKey);
            return false;
        }

        return true;
    }

    private bool ValidateTimestamp(string timestamp)
    {
        if (!DateTime.TryParse(timestamp, out var requestTime))
            return false;

        var timeDifference = DateTime.UtcNow - requestTime.ToUniversalTime();
        return timeDifference.Duration() <= TimeSpan.FromSeconds(30);
    }

    private async Task<string> ComputeSignature(HttpContext context, string timestamp, string secret)
    {
        var payload = await BuildSignaturePayload(context, timestamp);
        return HmacSignatureGenerator.GenerateSignature(secret, payload);
    }

    private async Task<string> BuildSignaturePayload(HttpContext context, string timestamp)
    {
        var builder = new StringBuilder();
        builder.Append(timestamp);
        builder.Append(context.Request.Method.ToUpper());
        builder.Append(context.Request.Path);
        builder.Append(context.Request.QueryString);

        if (context.Request.ContentLength > 0 &&
            (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
             context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
             context.Request.Method.Equals("PATCH", StringComparison.OrdinalIgnoreCase)))
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            builder.Append(body);
        }

        return builder.ToString();
    }
}