using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

/// <summary>
/// Defines the thumbmarkjs contract.
/// </summary>
public interface IThumbmarkjs : IAsyncDisposable
{
    /// <summary>
    /// Invoked after the browser library, component options, and removal observer have been initialized.
    /// </summary>
    EventCallback OnReady { get; set; }

    /// <summary>
    /// Replaces the fingerprint options and invalidates the cached result for this component.
    /// </summary>
    /// <param name="options">Options to configure for the thumbmarkjs.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the options has been stored.</returns>
    ValueTask SetOptions(object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the cached or newly computed browser fingerprint hash.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get.</returns>
    ValueTask<string?> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the cached or newly computed detailed fingerprint result.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested JSON Element.</returns>
    ValueTask<JsonElement?> GetData(CancellationToken cancellationToken = default);
}
