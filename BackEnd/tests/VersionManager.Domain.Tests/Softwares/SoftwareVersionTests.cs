using FluentAssertions;
using VersionManager.Domain.Softwares;

namespace VersionManager.Domain.Tests.Softwares;

public sealed class SoftwareVersionTests
{
    [Fact]
    public void Constructor_ShouldSetDeprecatedAt_WhenCreatedDeprecated()
    {
        var version = new SoftwareVersion(
            softwareId: Guid.NewGuid(),
            version: "1.0.0",
            releaseDate: new DateOnly(2026, 1, 1),
            isDeprecated: true
        );

        version.IsDeprecated.Should().BeTrue();
        version.DeprecatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldSetDeprecatedAt_WhenDeprecatedBecomesTrue()
    {
        var version = new SoftwareVersion(Guid.NewGuid(), "1.0.0", new DateOnly(2026, 1, 1), false);

        version.Update("1.0.0", new DateOnly(2026, 1, 1), true);

        version.IsDeprecated.Should().BeTrue();
        version.DeprecatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldClearDeprecatedAt_WhenDeprecatedBecomesFalse()
    {
        var version = new SoftwareVersion(Guid.NewGuid(), "1.0.0", new DateOnly(2026, 1, 1), true);

        version.Update("1.0.0", new DateOnly(2026, 1, 1), false);

        version.IsDeprecated.Should().BeFalse();
        version.DeprecatedAt.Should().BeNull();
    }
}
