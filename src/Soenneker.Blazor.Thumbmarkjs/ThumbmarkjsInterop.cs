using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Thumbmarkjs.Abstract;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Utils.CancellationScopes;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.Thumbmarkjs;

/// <inheritdoc cref="IThumbmarkjsInterop"/>
public sealed class ThumbmarkjsInterop : IThumbmarkjsInterop
{
    private const string _modulePath = "/_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmarkjsinterop.js";
    private const string _cdnScript = "https://cdn.jsdelivr.net/npm/@thumbmarkjs/thumbmarkjs@1.7.6/dist/thumbmark.umd.js";
    private const string _localScript = "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmark.umd.js";
    private const string _globalVariable = "ThumbmarkJS";
    private const string _cdnIntegrity = "sha256-ooYNzSvQWQrvXa7FACA7i/RhXhzgtejhXzais2EaZAc=";

    private readonly IResourceLoader _resourceLoader;
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly CancellationScope _cancellationScope = new();

    private readonly AsyncInitializer<bool> _scriptInitializer;
    private bool _useCdn = true;

    public ThumbmarkjsInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader, IModuleImportUtil moduleImportUtil)
    {
        _resourceLoader = resourceLoader;
        _moduleImportUtil = moduleImportUtil;
        _scriptInitializer = new AsyncInitializer<bool>(InitializeScript);
    }

    private static string NormalizeContentUri(string uri)
    {
        if (string.IsNullOrEmpty(uri) || uri.Contains("://", StringComparison.Ordinal))
            return uri;

        return uri[0] == '/' ? uri : "/" + uri;
    }

    private async ValueTask InitializeScript(bool useCdn, CancellationToken cancellationToken)
    {
        if (useCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_cdnScript, _globalVariable, _cdnIntegrity, cancellationToken: cancellationToken);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(NormalizeContentUri(_localScript), _globalVariable, cancellationToken: cancellationToken);
        }

        _ = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default)
    {
        _useCdn = useCdn;

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            await module.InvokeVoidAsync("initialize", linked, dotNetReference);
        }
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            await module.InvokeVoidAsync("createObserver", linked, elementId);
        }
    }

    public async ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            await module.InvokeVoidAsync("setOptions", linked, elementId, options);
        }
    }

    public async ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            return await module.InvokeAsync<string?>("get", linked, elementId);
        }
    }

    public async ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await _scriptInitializer.Init(_useCdn, linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            return await module.InvokeAsync<JsonElement?>("getData", linked, elementId);
        }
    }

    public async ValueTask Dispose(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            await module.InvokeVoidAsync("dispose", linked, elementId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}
