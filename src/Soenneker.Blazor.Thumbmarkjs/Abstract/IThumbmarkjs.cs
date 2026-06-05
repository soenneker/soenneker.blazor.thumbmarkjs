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
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SetOptions(object options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string?> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<JsonElement?> GetData(CancellationToken cancellationToken = default);
}