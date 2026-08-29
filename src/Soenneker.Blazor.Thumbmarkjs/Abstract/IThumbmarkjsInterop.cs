using Microsoft.JSInterop;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

/// <summary>
/// Defines the thumbmarkjs interop contract.
/// </summary>
public interface IThumbmarkjsInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the thumbmarkjs so it is ready for use.
    /// </summary>
    /// <param name="elementId">ID used to isolate this component's browser state and callbacks.</param>
    /// <param name="dotNetReference">JavaScript-invokable reference to the .NET component instance.</param>
    /// <param name="useCdn">Whether cdn.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the thumbmarkjs is ready for use.</returns>
    ValueTask Initialize(string elementId, DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates observer.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the observer creation is complete.</returns>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets options.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="options">Options to configure for the thumbmarkjs.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the options has been stored.</returns>
    ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the configured resulting text used by the thumbmarkjs.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get.</returns>
    ValueTask<string?> Get(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets data.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested JSON Element.</returns>
    ValueTask<JsonElement?> GetData(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases resources used by the current instance.
    /// </summary>
    /// <param name="elementId">ID of the DOM element to target.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the dispose operation is complete.</returns>
    ValueTask Dispose(string elementId, CancellationToken cancellationToken = default);
}
