using System.Text.Json.Serialization;

namespace Soenneker.Blazor.Thumbmarkjs.Configuration;

/// <summary>
/// Configuration options for Thumbmark.js.
/// See https://github.com/thumbmarkjs/thumbmarkjs for more details.
/// </summary>
public sealed class ThumbmarkjsOptions
{
    /// <summary>
    /// Your API key from https://thumbmarkjs.com. 
    /// Setting this makes thumbmarks significantly more unique.
    /// Example: "ae8679607bf79f......"
    /// </summary>
    [JsonPropertyName("api_key")]
    public string? ApiKey { get; set; }

    /// <summary>
    /// Removes components from the fingerprint hash. An excluded top-level component improves performance.
    /// Example: ['webgl', 'system.browser.version']
    /// </summary>
    [JsonPropertyName("exclude")]
    public string[]? Exclude { get; set; }

    /// <summary>
    /// Only includes the listed components. `exclude` still excludes included components.
    /// Example: ['webgl', 'system.browser.version']
    /// </summary>
    [JsonPropertyName("include")]
    public string[]? Include { get; set; }

    /// <summary>
    /// Checks only selected permissions (e.g., 'gyroscope', 'accelerometer').
    /// Permissions take the longest to resolve, so use this to reduce delay.
    /// </summary>
    [JsonPropertyName("permissions_to_check")]
    public string[]? PermissionsToCheck { get; set; }

    /// <summary>
    /// Component timeout in milliseconds. Default is 5000.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 5000;

    /// <summary>
    /// Enables or disables anonymous logging (max 0.01%) to improve the library. Default is true.
    /// </summary>
    [JsonPropertyName("logging")]
    public bool Logging { get; set; } = true;

    /// <summary>
    /// If true, caches the API response for the current page load. Default is false.
    /// </summary>
    [JsonPropertyName("cache_api_call")]
    public bool CacheApiCall { get; set; } = false;

    /// <summary>
    /// If true, includes millisecond timing performance of component resolution. Default is false.
    /// </summary>
    [JsonPropertyName("performance")]
    public bool Performance { get; set; } = false;

    /// <summary>
    /// If true, loads Thumbmark.js from the CDN. Default is true.
    /// </summary>
    [JsonPropertyName("useCdn")]
    public bool UseCdn { get; set; } = true;
}