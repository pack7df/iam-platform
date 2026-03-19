using FluentAssertions;
using IamPlatform.Application.Identity;
using IamPlatform.Domain.Identity;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserBootstrapperTests
{
    [Fact]
    public async Task BootstrapAsync_Should_Create_SystemUser_When_None_Exists()
    {
        var repository = new FakeSystemUserRepository(existing: false);
        var bootstrapper = new SystemUserBootstrapper(repository);

        var result = await bootstrapper.BootstrapAsync("system-user-001");

        result.IsCreated.Should().BeTrue();
        result.SystemUser.Should().NotBeNull();
        result.SystemUser!.Id.Should().Be("system-user-001");
        repository.AddedSystemUser.Should().NotBeNull();
        repository.AddedSystemUser!.Id.Should().Be("system-user-001");
    }

    [Fact]
    public async Task BootstrapAsync_Should_Reject_When_SystemUser_Already_Exists()
    {
        var repository = new FakeSystemUserRepository(existing: true);
        var bootstrapper = new SystemUserBootstrapper(repository);

        var result = await bootstrapper.BootstrapAsync("system-user-001");

        result.IsCreated.Should().BeFalse();
        result.SystemUser.Should().BeNull();
        repository.AddedSystemUser.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task BootstrapAsync_Should_Reject_Invalid_Id(string invalidId)
    {
        var repository = new FakeSystemUserRepository(existing: false);
        var bootstrapper = new SystemUserBootstrapper(repository);

        var act = () => bootstrapper.BootstrapAsync(invalidId);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("System user id is required.*");
        repository.AddedSystemUser.Should().BeNull();
    }

    private sealed class FakeSystemUserRepository : ISystemUserRepository
    {
        private readonly bool _existing;

        public FakeSystemUserRepository(bool existing)
        {
            _existing = existing;
        }

        public SystemUser? AddedSystemUser { get; private set; }

        public Task<bool> ExistsAnySystemUserAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_existing);
        }

        public Task AddAsync(SystemUser systemUser, CancellationToken cancellationToken = default)
        {
            AddedSystemUser = systemUser;
            return Task.CompletedTask;
        }
    }
}
