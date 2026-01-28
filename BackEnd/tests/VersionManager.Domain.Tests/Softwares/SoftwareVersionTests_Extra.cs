using FluentAssertions;
using VersionManager.Domain.Common;
using VersionManager.Domain.Softwares;

namespace VersionManager.Domain.Tests.Softwares;

public sealed class SoftwareVersionTests_Extra
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_ShouldThrow_WhenVersionIsInvalid(string invalidVersion)
    {
        var version = new SoftwareVersion(
            softwareId: Guid.NewGuid(),
            version: "1.0.0",
            releaseDate: new DateOnly(2026, 1, 1),
            isDeprecated: false
        );

        var act = () => version.Update(invalidVersion, new DateOnly(2026, 1, 1), false);

        act.Should().Throw<DomainException>()
            .WithMessage("*Versão inválida*");
    }
}
