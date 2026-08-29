[![](https://img.shields.io/nuget/v/soenneker.blazor.thumbmarkjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.thumbmarkjs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.thumbmarkjs/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.thumbmarkjs/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.thumbmarkjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.thumbmarkjs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.thumbmarkjs/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.thumbmarkjs/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.Thumbmarkjs

A Blazor component and JS interop wrapper for computing browser fingerprints with [ThumbmarkJS](https://github.com/thumbmarkjs/thumbmarkjs).

[Live demo](https://soenneker.github.io/soenneker.blazor.thumbmarkjs)

Browser fingerprinting has privacy, consent, and regulatory consequences. Establish a lawful purpose and gate collection behind the consent required for your users and jurisdictions before rendering or calling this component.

## Installation

```bash
dotnet add package Soenneker.Blazor.Thumbmarkjs
```

Register the interop service in `Program.cs`:

```csharp
using Soenneker.Blazor.Thumbmarkjs.Registrars;

builder.Services.AddThumbmarkjsInteropAsScoped();
```

Add the component namespace to `_Imports.razor`:

```razor
@using Soenneker.Blazor.Thumbmarkjs
```

## Usage

Render the component only after your application has decided fingerprinting is permitted, then wait for `OnReady` before calling it:

```razor
@using Soenneker.Blazor.Thumbmarkjs.Configuration

@if (_fingerprintingAllowed)
{
    <Thumbmarkjs @ref="_thumbmark"
                 Options="_options"
                 OnReady="HandleReady" />

    <button type="button" disabled="@(!_ready)" @onclick="GenerateAsync">
        Generate browser identifier
    </button>
}

@if (_identifier is not null)
{
    <p>Identifier: @_identifier</p>
}

@code {
    private Thumbmarkjs? _thumbmark;
    private string? _identifier;
    private bool _ready;
    private bool _fingerprintingAllowed;

    private readonly ThumbmarkjsOptions _options = new()
    {
        Logging = false,
        Exclude = ["audio", "permissions"]
    };

    private void HandleReady() => _ready = true;

    private async Task GenerateAsync()
    {
        _identifier = await _thumbmark!.Get();
    }
}
```

`Get()` returns the fingerprint hash. `GetData()` returns the detailed result as a `JsonElement`, including the hash and the browser components used to derive it:

```csharp
JsonElement? result = await _thumbmark!.GetData();
```

Detailed results can contain browser, hardware, locale, screen, permission-state, canvas, audio, WebGL, WebRTC, font, and similar signals depending on the configured include/exclude lists and browser support. Avoid collecting or retaining the detailed payload unless it is genuinely required.

## Caching and callbacks

The first `Get()` or `GetData()` call computes a result and caches it for that component. Later calls reuse the same result. Calling `SetOptions()` replaces the options and invalidates the cached result:

```csharp
await _thumbmark!.SetOptions(new ThumbmarkjsOptions
{
    Logging = false,
    Exclude = ["audio", "canvas", "webgl"]
});

string? recomputed = await _thumbmark.Get();
```

`OnGenerated` runs when either method returns a hash. `OnDataGenerated` runs only for `GetData()`. Multiple component instances keep independent options, cached results, observers, and callbacks.

## Network behavior

`UseCdn` controls where the ThumbmarkJS library itself is loaded from:

- `true` (default) loads the pinned library from jsDelivr with subresource integrity validation;
- `false` loads the bundled copy from this package's `_content` assets.

Fingerprint computation remains local unless network-related ThumbmarkJS options are enabled or your application transmits the result:

- Setting `ApiKey` causes ThumbmarkJS to post component data to `ApiEndpoint`, or to ThumbmarkJS's default API when no endpoint is supplied.
- `CacheApiCall` and `CacheLifetimeInMs` control caching for that API response; ThumbmarkJS can use browser storage for visitor and cache data.
- `Logging` is disabled by default by this wrapper. Explicitly enabling it permits ThumbmarkJS's sampled diagnostic logging, which can send fingerprint data to the ThumbmarkJS service.
- `Metadata` is included in API requests when API mode is active. Do not place secrets or unnecessary personal data in it.

Your Content Security Policy must permit jsDelivr when `UseCdn` is enabled and must permit any configured fingerprint API endpoint.

## Reliability and security boundaries

- A thumbmark is probabilistic and can change after browser, device, privacy-setting, network, or library changes. Different users can also collide.
- Do not use a fingerprint as authentication, authorization, proof of identity, or the sole basis for blocking a user.
- Treat it as one risk or analytics signal with documented retention and deletion rules. Hashing fingerprint components does not make the result anonymous.
- Browser interop is unavailable during static rendering or prerendering. Call methods only after `OnReady` in an interactive render.
