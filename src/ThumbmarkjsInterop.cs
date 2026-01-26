using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Thumbmarkjs.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Utils.CancellationScopes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.Thumbmarkjs;

/// <inheritdoc cref="IThumbmarkjsInterop"/>
public sealed class ThumbmarkjsInterop : IThumbmarkjsInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncInitializer<bool> _scriptInitializer;

    private const string _module = "Soenneker.Blazor.Thumbmarkjs/js/thumbmarkjsinterop.js";
    private const string _moduleName = "ThumbmarkjsInterop";

    private readonly CancellationScope _cancellationScope = new();

    public ThumbmarkjsInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncInitializer<bool>(Initialize);
    }

    private async ValueTask Initialize(bool useCdn, CancellationToken token)
    {
        if (useCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(
                "https://cdn.jsdelivr.net/npm/@thumbmarkjs/thumbmarkjs@1.0.0/dist/thumbmark.umd.js",
                "ThumbmarkJS",
                "sha256-7ngQC8Zs8j/SJLg4IezN/uxMT4AHr2QOyWxPew/+trQ=",
                cancellationToken: token);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(
                "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmark.umd.js",
                "ThumbmarkJS",
                cancellationToken: token);
        }

        await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);
    }

    public async ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(useCdn, linked);
            await _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.initialize", linked, dotNetReference);
        }
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.createObserver", linked, elementId);
    }

    public async ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(true, linked);
            await _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.setOptions", linked, elementId, options);
        }
    }

    public async ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(true, linked);
            return await _jsRuntime.InvokeAsync<string?>("ThumbmarkjsInterop.get", linked, elementId);
        }
    }

    public async ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(true, linked);
            return await _jsRuntime.InvokeAsync<JsonElement?>("ThumbmarkjsInterop.getData", linked, elementId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}
