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
                    "https://cdn.jsdelivr.net/npm/@thumbmarkjs/thumbmarkjs@1.0.0/dist/thumbmark.umd.js",
                    "ThumbmarkJS",
                    "sha256-7ngQC8Zs8j/SJLg4IezN/uxMT4AHr2QOyWxPew/+trQ=",
                    cancellationToken: token)
                    ;
            }
            else
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "_content/Soenneker.Blazor.Thumbmarkjs/js/thumbmark.umd.js",
                    "ThumbmarkJS",
                    cancellationToken: token)
                    ;
            }

            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);

            return new object();
        });
    }

    public async ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken, useCdn);
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.initialize", cancellationToken, dotNetReference);
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", cancellationToken, elementId);
    }

    public async ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken);
        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.setOptions", cancellationToken, elementId, options);
    }

    public async ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken);
        return await _jsRuntime.InvokeAsync<string?>($"{_moduleName}.get", cancellationToken, elementId);
    }

    public async ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken);
        return await _jsRuntime.InvokeAsync<JsonElement?>($"{_moduleName}.getData", cancellationToken, elementId);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
    }
}
