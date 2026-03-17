using FluentAssertions;
using IamPlatform.Domain;
using Xunit;

namespace IamPlatform.UnitTests;

public sealed class DomainAssemblyMarkerTests
{
    [Fact]
    public void DomainAssemblyMarker_Should_Be_Accessible_From_Test_Project()
    {
        var markerType = typeof(DomainAssemblyMarker);

        markerType.Assembly.GetName().Name.Should().Be("IamPlatform.Domain");
    }
}
