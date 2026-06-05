using System.Collections.Generic;

namespace Soenneker.Blazor.Thumbmarkjs.Configuration;

/// <summary>
/// Represents the thumbmarkjs options.
/// </summary>
public sealed class ThumbmarkjsOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether use cdn.
    /// </summary>
    public bool UseCdn { get; set; } = true;

    /// <summary>
    /// Gets or sets api key.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets api endpoint.
    /// </summary>
    public string? ApiEndpoint { get; set; }

    /// <summary>
    /// Gets or sets include.
    /// </summary>
    public List<string>? Include { get; set; }

    /// <summary>
    /// Gets or sets exclude.
    /// </summary>
    public List<string>? Exclude { get; set; }

    /// <summary>
    /// Gets or sets permissions to check.
    /// </summary>
    public List<string>? PermissionsToCheck { get; set; }

    /// <summary>
    /// Gets or sets stabilize.
    /// </summary>
    public List<string>? Stabilize { get; set; }

    /// <summary>
    /// Gets or sets timeout.
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether logging.
    /// </summary>
    public bool? Logging { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether cache api call.
    /// </summary>
    public bool? CacheApiCall { get; set; }

    /// <summary>
    /// Gets or sets cache lifetime in ms.
    /// </summary>
    public int? CacheLifetimeInMs { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether performance.
    /// </summary>
    public bool? Performance { get; set; }

    /// <summary>
    /// Gets or sets metadata.
    /// </summary>
    public object? Metadata { get; set; }
}