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
    private const string _modulePath = "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmarkjsinterop.js";
    private const string _cdnScript = "https://cdn.jsdelivr.net/npm/@thumbmarkjs/thumbmarkjs@1.10.1/dist/thumbmark.umd.js";
    private const string _localScript = "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmark.umd.js";
    private const string _globalVariable = "ThumbmarkJS";
    private const string _cdnIntegrity = "sha256-ooYNzSvQWQrvXa7FACA7i/RhXhzgtejhXzais2EaZAc=";

    private readonly IResourceLoader _resourceLoader;
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly CancellationScope _cancellationScope = new();

    private readonly AsyncInitializer<bool> _scriptInitializer;
    private bool _useCdn = true;
    private bool _moduleInitialized;

    public ThumbmarkjsInterop(IResourceLoader resourceLoader, IModuleImportUtil moduleImportUtil)
    {
        _resourceLoader = resourceLoader;
        _moduleImportUtil = moduleImportUtil;
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

        _ = await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    public async ValueTask Initialize(string elementId, DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true,
        CancellationToken cancellationToken = default)
    {
        _useCdn = useCdn;

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            try
            {
                await _scriptInitializer.Init(_useCdn, linked);
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
                _moduleInitialized = true;
                await module.InvokeVoidAsync("initialize", linked, elementId, dotNetReference);
            }
            catch (JSException exception)
            {
                throw new JSException($"JavaScript operation 'ThumbmarkjsInterop.Initialize' failed: {exception.Message}", exception);
            }
        }
    }

    public async ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            try
            {
                await _scriptInitializer.Init(_useCdn, linked);
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
                await module.InvokeVoidAsync("createObserver", linked, elementId);
            }
            catch (JSException exception)
            {
                throw new JSException($"JavaScript operation 'ThumbmarkjsInterop.CreateObserver' failed: {exception.Message}", exception);
            }
        }
    }

    public async ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            try
            {
                await _scriptInitializer.Init(_useCdn, linked);
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
                await module.InvokeVoidAsync("setOptions", linked, elementId, options);
            }
            catch (JSException exception)
            {
                throw new JSException($"JavaScript operation 'ThumbmarkjsInterop.SetOptions' failed: {exception.Message}", exception);
            }
        }
    }

    public async ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            try
            {
                await _scriptInitializer.Init(_useCdn, linked);
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
                return await module.InvokeAsync<string?>("get", linked, elementId);
            }
            catch (JSException exception)
            {
                throw new JSException($"JavaScript operation 'ThumbmarkjsInterop.Get' failed: {exception.Message}", exception);
            }
        }
    }

    public async ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            try
            {
                await _scriptInitializer.Init(_useCdn, linked);
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
                return await module.InvokeAsync<JsonElement?>("getData", linked, elementId);
            }
            catch (JSException exception)
            {
                throw new JSException($"JavaScript operation 'ThumbmarkjsInterop.GetData' failed: {exception.Message}", exception);
            }
        }
    }

    public async ValueTask Dispose(string elementId, CancellationToken cancellationToken = default)
    {
        if (!_moduleInitialized)
            return;

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            try
            {
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
                await module.InvokeVoidAsync("dispose", linked, elementId);
            }
            catch (JSException exception)
            {
                throw new JSException($"JavaScript operation 'ThumbmarkjsInterop.Dispose' failed: {exception.Message}", exception);
            }
        }
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_moduleInitialized)
        {
            try
            {
                IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath);
                await module.InvokeVoidAsync("dispose");
            }
            catch (JSDisconnectedException)
            {
            }
        }

        await _cancellationScope.DisposeAsync();
        await _scriptInitializer.DisposeAsync();
        await _moduleImportUtil.DisposeContentModule(_modulePath);
    }
}
