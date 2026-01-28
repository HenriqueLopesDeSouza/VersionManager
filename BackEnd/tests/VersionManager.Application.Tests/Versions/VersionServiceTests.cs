using FluentAssertions;
using Moq;
using VersionManager.Application.Abstractions;
using VersionManager.Application.Versions;
using VersionManager.Application.Versions.Dtos;
using VersionManager.Domain.Common;
using VersionManager.Domain.Softwares;

namespace VersionManager.Application.Tests.Versions;

public sealed class VersionServiceTests
{
    private readonly Mock<ISoftwareRepository> _repo = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);

    private VersionService CreateSut()
        => new VersionService(_repo.Object, _uow.Object);

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenSoftwareNotFound()
    {
        var ct = CancellationToken.None;
        var sut = CreateSut();

        var softwareId = Guid.NewGuid();
        var dto = new CreateVersionDto(
            Version: "1.0.0",
            ReleaseDate: new DateOnly(2026, 1, 1),
            IsDeprecated: false
        );


        _repo.Setup(r => r.GetWithVersionsAsync(softwareId, ct))
            .ReturnsAsync((Software?)null);

        var act = async () => await sut.CreateAsync(softwareId, dto, ct);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Software não encontrado*");

        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task PatchAsync_ShouldThrow_WhenNoFieldsProvided()
    {
        var ct = CancellationToken.None;
        var sut = CreateSut();

        var dto = new PatchVersionDto(
            Version: null,
            ReleaseDate: null,
            IsDeprecated: null
        );

        var act = async () => await sut.PatchAsync(Guid.NewGuid(), Guid.NewGuid(), dto, ct);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*ao menos um campo*");

        _repo.Verify(r => r.GetWithVersionsAsync(It.IsAny<Guid>(), ct), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }
}
