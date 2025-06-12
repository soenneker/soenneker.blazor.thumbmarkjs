using System.Text.Json.Serialization;

namespace Soenneker.Blazor.Thumbmarkjs.Configuration;

/// <summary>
/// Configuration options for Thumbmark.js
/// See https://github.com/thumbmarkjs/thumbmarkjs for more details
/// </summary>
public sealed class ThumbmarkjsOptions
{
    /// <summary>
    /// Removes components from the fingerprint hash. An excluded top-level component improves performance.
    /// Example: ['webgl', 'system.browser.version']
    /// </summary>
    [JsonPropertyName("exclude")]
    public string[]? Exclude { get; set; }

    /// <summary>
    /// Only includes the listed components. exclude still excludes included components.
    /// Example: ['webgl', 'system.browser.version']
    /// </summary>
    [JsonPropertyName("include")]
    public string[]? Include { get; set; }

    /// <summary>
    /// Component timeout in milliseconds. Default is 1000.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 1000;

    /// <summary>
    /// Setting to false disables the anonymous 0.01% log sampling that is used to improve the library. Default is true.
    /// </summary>
    [JsonPropertyName("logging")]
    public bool Logging { get; set; } = true;

    [JsonPropertyName("useCdn")]
    public bool UseCdn { get; set; } = true;
}