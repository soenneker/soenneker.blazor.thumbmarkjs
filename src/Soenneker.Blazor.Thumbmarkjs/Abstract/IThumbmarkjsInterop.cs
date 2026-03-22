using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

public interface IThumbmarkjsInterop : IAsyncDisposable
{
    ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default);

    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default);

    ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default);

    ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default);

    ValueTask Dispose(string elementId, CancellationToken cancellationToken = default);
}