using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

/// <summary>
/// Defines the thumbmarkjs contract.
/// </summary>
public interface IThumbmarkjs : IAsyncDisposable
{
    /// <summary>
    /// Sets options.
    /// </summary>
    /// <param name="options">Options to configure for the thumbmarkjs.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the options has been stored.</returns>
    ValueTask SetOptions(object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the configured resulting text used by the thumbmarkjs.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by get.</returns>
    ValueTask<string?> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets data.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested JSON Element.</returns>
    ValueTask<JsonElement?> GetData(CancellationToken cancellationToken = default);
}
