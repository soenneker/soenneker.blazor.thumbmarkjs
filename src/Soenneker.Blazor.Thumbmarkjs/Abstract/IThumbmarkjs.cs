using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Blazor.Thumbmarkjs.Abstract;

public interface IThumbmarkjs : IAsyncDisposable
{
    ValueTask SetOptions(object options, CancellationToken cancellationToken = default);

    ValueTask<string?> Get(CancellationToken cancellationToken = default);

    ValueTask<JsonElement?> GetData(CancellationToken cancellationToken = default);
}