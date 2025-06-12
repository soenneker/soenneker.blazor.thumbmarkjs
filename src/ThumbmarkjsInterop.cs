using Soenneker.Blazor.Thumbmarkjs.Abstract;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Extensions.ValueTask;
using System.Text.Json;

namespace Soenneker.Blazor.Thumbmarkjs;

/// <inheritdoc cref="IThumbmarkjsInterop"/>
public sealed class ThumbmarkjsInterop : IThumbmarkjsInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncSingleton _scriptInitializer;

    private const string _module = "Soenneker.Blazor.Thumbmarkjs/js/thumbmarkjsinterop.js";
    private const string _moduleName = "ThumbmarkjsInterop";

    public ThumbmarkjsInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncSingleton(async (token, arr) =>
        {
            var useCdn = true;

            if (arr.Length > 0)
                useCdn = (bool)arr[0];

            if (useCdn)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/@thumbmarkjs/thumbmarkjs@0.20.4/dist/thumbmark.umd.js",
                    "ThumbmarkJS",
                    "sha256-Tlokyjgdnumq7U6yVyscdfWr4YNiSaWrcqnAuWgwZSU=",
                    cancellationToken: token)
                    .NoSync();
            }
            else
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmark.umd.js",
                    "ThumbmarkJS",
                    cancellationToken: token)
                    .NoSync();
            }

            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token).NoSync();

            return new object();
        });
    }

    public async ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken, useCdn).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.initialize", cancellationToken, dotNetReference).NoSync();
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", cancellationToken, elementId);
    }

    public async ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.setOptions", cancellationToken, elementId, options).NoSync();
    }

    public async ValueTask<string?> GetFingerprint(string elementId, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getFingerprint", cancellationToken, elementId).NoSync();
    }

    public async ValueTask<JsonElement?> GetFingerprintData(string elementId, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        return await _jsRuntime.InvokeAsync<JsonElement?>($"{_moduleName}.getFingerprintData", cancellationToken, elementId).NoSync();
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module).NoSync();
        await _scriptInitializer.DisposeAsync().NoSync();
    }
}
