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
    private const string _module = "Soenneker.Blazor.Thumbmarkjs/js/thumbmarkjsinterop.js";
    private const string _cdnScript = "https://cdn.jsdelivr.net/npm/@thumbmarkjs/thumbmarkjs@1.7.6/dist/thumbmark.umd.js";
    private const string _localScript = "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmark.umd.js";
    private const string _globalVariable = "ThumbmarkJS";
    private const string _cdnIntegrity = "sha256-ooYNzSvQWQrvXa7FACA7i/RhXhzgtejhXzais2EaZAc=";

    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;
    private readonly CancellationScope _cancellationScope = new();

    private readonly AsyncInitializer<bool> _scriptInitializer;
    private bool _useCdn = true;

    public ThumbmarkjsInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;
        _scriptInitializer = new AsyncInitializer<bool>(InitializeScript);
    }

    private async ValueTask InitializeScript(bool useCdn, CancellationToken cancellationToken)
    {
        if (useCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_cdnScript, _globalVariable, _cdnIntegrity, cancellationToken: cancellationToken);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_localScript, _globalVariable, cancellationToken: cancellationToken);
        }

        await _resourceLoader.ImportModule(_module, cancellationToken);
    }

    public async ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default)
    {
        _useCdn = useCdn;

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            await _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.initialize", linked, dotNetReference);
        }
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            await _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.createObserver", linked, elementId);
        }
    }

    public async ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            await _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.setOptions", linked, elementId, options);
        }
    }

    public async ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            return await _jsRuntime.InvokeAsync<string?>("ThumbmarkjsInterop.get", linked, elementId);
        }
    }

    public async ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            return await _jsRuntime.InvokeAsync<JsonElement?>("ThumbmarkjsInterop.getData", linked, elementId);
        }
    }

    public async ValueTask Dispose(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _jsRuntime.InvokeVoidAsync("ThumbmarkjsInterop.dispose", linked, elementId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}