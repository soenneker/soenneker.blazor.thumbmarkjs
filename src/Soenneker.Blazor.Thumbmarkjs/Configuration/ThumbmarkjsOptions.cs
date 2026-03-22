using System.Collections.Generic;

namespace Soenneker.Blazor.Thumbmarkjs.Configuration;

public sealed class ThumbmarkjsOptions
{
    public bool UseCdn { get; set; } = true;

    public string? ApiKey { get; set; }

    public string? ApiEndpoint { get; set; }

    public List<string>? Include { get; set; }

    public List<string>? Exclude { get; set; }

    public List<string>? PermissionsToCheck { get; set; }

    public List<string>? Stabilize { get; set; }

    public int? Timeout { get; set; }

    public bool? Logging { get; set; }

    public bool? CacheApiCall { get; set; }

    public int? CacheLifetimeInMs { get; set; }

    public bool? Performance { get; set; }

    public object? Metadata { get; set; }
}