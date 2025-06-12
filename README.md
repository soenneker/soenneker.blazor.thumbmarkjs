[![](https://img.shields.io/nuget/v/soenneker.blazor.thumbmarkjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.thumbmarkjs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.thumbmarkjs/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.thumbmarkjs/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.thumbmarkjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.thumbmarkjs/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.Thumbmarkjs
### A Blazor interop library for the javascript fingerprinting library, [thumbmarkjs](https://github.com/thumbmarkjs/thumbmarkjs)

[Demo](https://soenneker.github.io/soenneker.blazor.thumbmarkjs)

## Installation

```
dotnet add package Soenneker.Blazor.Thumbmarkjs
```

## Usage

### Basic Setup

1. Register the service in your `Program.cs`:

```csharp
builder.Services.AddThumbmarkjsInteropAsScoped();
```

2. Add the component to your page or component:

```razor
<Thumbmarkjs @ref="_thumbmarkjs" OnFingerprintGenerated="OnFingerprintGenerated"></Thumbmarkjs>
```

### Getting a Fingerprint

```csharp
private Thumbmarkjs _thumbmarkjs;

private async Task GetFingerprint()
{
    string fingerprint = await _thumbmarkjs.GetFingerprint();
    // Use the fingerprint...
}

public void OnFingerprintGenerated(string fingerprint)
{
    // Handle the fingerprint when it's generated
}

public void OnFingerprintDataGenerated(JsonElement data)
{
    // Handle the fingerprint when it's generated
}
```

### Getting Detailed Fingerprint Data

```csharp
private async Task GetFingerprintData()
{
    JsonElement? data = await _thumbmarkjs.GetFingerprintData();
    string jsonData = data?.ToString();
    // Use the detailed data...
}
```

### Configuring Options

```csharp
private async Task ConfigureOptions()
{
    var options = new ThumbmarkjsOptions
    {
        Exclude = ["webgl", "audio"], // Components to exclude
        Timeout = 1000, // Timeout in milliseconds
    };

    await _thumbmarkjs.SetOptions(options);
}
```