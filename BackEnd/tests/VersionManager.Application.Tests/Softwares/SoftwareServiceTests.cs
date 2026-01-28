using FluentAssertions;
using Moq;
using System.Xml.Linq;
using VersionManager.Application.Abstractions;
using VersionManager.Application.Softwares;
using VersionManager.Application.Softwares.Dtos;
using VersionManager.Domain.Common;
using VersionManager.Domain.Softwares;

namespace VersionManager.Application.Tests.Softwares;

public sealed class SoftwareServiceTests
{
    private readonly Mock<ISoftwareRepository> _repo = new(MockBehavior.Strict);
    private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);

    private SoftwareService CreateSut()
        => new SoftwareService(_repo.Object, _uow.Object);

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenNameAlreadyExists()
    {
        var ct = CancellationToken.None;
        var sut = CreateSut();

        var dto = new CreateSoftwareDto ( Name: "Sistema X", Description: "desc" );

        _repo.Setup(r => r.ExistsByNameAsync("Sistema X", null, ct))
            .ReturnsAsync(true);

        var act = async () => await sut.CreateAsync(dto, ct);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Já existe um software*");

        _repo.Verify(r => r.AddAsync(It.IsAny<Software>(), ct), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task PatchAsync_ShouldThrow_WhenNoFieldsProvided()
    {
        var ct = CancellationToken.None;
        var sut = CreateSut();

        var dto = new PatchSoftwareDto ( Name: null, Description: null );

        var act = async () => await sut.PatchAsync(Guid.NewGuid(), dto, ct);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*ao menos um campo*");

        _repo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), ct), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }
}
