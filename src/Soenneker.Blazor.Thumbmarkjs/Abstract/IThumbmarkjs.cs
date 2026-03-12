using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Soenneker.Quark;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

/// <summary>
/// Interface for the Thumbmarkjs Blazor component that provides browser fingerprinting capabilities.
/// This component wraps the Thumbmark.js library to provide browser fingerprinting functionality in Blazor applications.
/// </summary>
public interface IThumbmarkjs : ICoreCancellableComponent
{
    /// <summary>
    /// Sets configuration options for the Thumbmark.js instance.
    /// </summary>
    /// <param name="options">The options to configure the Thumbmark.js instance.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method allows you to configure various aspects of the fingerprinting process,
    /// such as excluding specific components, setting timeouts, and enabling logging.
    /// </remarks>
    ValueTask SetOptions(object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the fingerprint hash for the current browser.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask containing the fingerprint hash string, or null if the operation fails.</returns>
    /// <remarks>
    /// The fingerprint hash is a unique identifier generated based on various browser characteristics.
    /// This hash can be used to identify returning visitors or track browser instances.
    /// </remarks>
    ValueTask<string?> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the detailed fingerprint data for the current browser.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>A ValueTask containing the detailed fingerprint data as a JsonElement, or null if the operation fails.</returns>
    /// <remarks>
    /// This method returns the raw fingerprint data including all collected browser characteristics.
    /// The data is returned as a JsonElement that can be parsed for detailed analysis.
    /// </remarks>
    ValueTask<JsonElement?> GetData(CancellationToken cancellationToken = default);


    /// <summary>
    /// Event callback that is triggered when a fingerprint is generated.
    /// </summary>
    /// <remarks>
    /// This event can be used to react to fingerprint generation events,
    /// such as updating UI elements or triggering other operations.
    /// </remarks>
    EventCallback<string> OnGenerated { get; set; }

    EventCallback<JsonElement> OnDataGenerated { get; set; }
} 
