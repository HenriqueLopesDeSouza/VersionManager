using FluentAssertions;
using VersionManager.Domain.Common;
using VersionManager.Domain.Softwares;

namespace VersionManager.Domain.Tests.Softwares;

public sealed class SoftwareTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsInvalid()
    {
        var act = () => new Software(" ", "desc");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddVersion_ShouldAddVersion()
    {
        var software = new Software("Sistema X", null);

        var created = software.AddVersion("1.0.0", new DateOnly(2026, 1, 1), isDeprecated: false);

        created.Should().NotBeNull();
        software.Versions.Should().HaveCount(1);
        software.Versions.Should().ContainSingle(v => v.Version == "1.0.0");
    }

    [Fact]
    public void AddVersion_ShouldThrow_WhenVersionAlreadyExists_ForSameSoftware()
    {
        var software = new Software("Sistema X", null);

        software.AddVersion("1.0.0", new DateOnly(2026, 1, 1), false);

        var act = () => software.AddVersion("1.0.0", new DateOnly(2026, 1, 2), false);

        act.Should().Throw<DomainException>()
            .WithMessage("*já existe*");
    }
}
