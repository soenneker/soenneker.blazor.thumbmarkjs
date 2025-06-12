using Soenneker.Blazor.Thumbmarkjs.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Blazor.Thumbmarkjs.Tests;

[Collection("Collection")]
public sealed class ThumbmarkjsInteropTests : FixturedUnitTest
{
    private readonly IThumbmarkjsInterop _blazorlibrary;

    public ThumbmarkjsInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _blazorlibrary = Resolve<IThumbmarkjsInterop>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
