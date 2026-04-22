using Soenneker.Blazor.Thumbmarkjs.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Blazor.Thumbmarkjs.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class ThumbmarkjsInteropTests : HostedUnitTest
{
    private readonly IThumbmarkjsInterop _blazorlibrary;

    public ThumbmarkjsInteropTests(Host host) : base(host)
    {
        _blazorlibrary = Resolve<IThumbmarkjsInterop>(true);
    }

    [Test]
    public void Default()
    {

    }
}
