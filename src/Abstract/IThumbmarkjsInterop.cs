using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

/// <summary>
/// Interface for the Thumbmarkjs JavaScript interop service.
/// </summary>
public interface IThumbmarkjsInterop
{
    /// <summary>
    /// Initializes the Thumbmark.js library and sets up the interop.
    /// </summary>
    /// <param name="dotNetReference">The DotNetObjectReference for callbacks.</param>
    /// <param name="useCdn">Whether to use the CDN version of the library.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    ValueTask Initialize(DotNetObjectReference<Thumbmarkjs> dotNetReference, bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a mutation observer for the specified element.
    /// </summary>
    /// <param name="elementId">The ID of the element to observe.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets options for the Thumbmark.js instance.
    /// </summary>
    /// <param name="elementId">The ID of the element.</param>
    /// <param name="options">The options to set.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    ValueTask SetOptions(string elementId, object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the fingerprint hash for the current browser.
    /// </summary>
    /// <param name="elementId">The ID of the element.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The fingerprint hash string.</returns>
    ValueTask<string?> GetFingerprint(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the detailed fingerprint data for the current browser.
    /// </summary>
    /// <param name="elementId">The ID of the element.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The fingerprint data as a JsonElement.</returns>
    ValueTask<JsonElement?> GetFingerprintData(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disposes of the interop resources.
    /// </summary>
    ValueTask DisposeAsync();
}