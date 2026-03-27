using FluentAssertions;
using IamPlatform.Application.Identity;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserBootstrapperTests
{
    [Fact]
    public async Task BootstrapAsync_Should_Create_SystemUser_When_None_Exists()
    {
        var repository = new FakeUserRepository(existing: false);
        var uow = new FakeUnitOfWork();
        var bootstrapper = new SystemUserBootstrapper(repository, uow);

        var result = await bootstrapper.BootstrapAsync();

        result.IsCreated.Should().BeTrue();
        result.SystemUser.Should().NotBeNull();
        result.SystemUser!.Id.Should().NotBeNullOrWhiteSpace(); // Generado automáticamente
        result.SystemUser.Type.Should().Be(UserType.SystemUser);
        repository.AddedUser.Should().NotBeNull();
        repository.AddedUser!.Id.Should().Be(result.SystemUser.Id);
        repository.AddedUser.Type.Should().Be(UserType.SystemUser);
        uow.SaveChangesCalled.Should().BeTrue();
    }

    [Fact]
    public async Task BootstrapAsync_Should_Reject_When_SystemUser_Already_Exists()
    {
        var repository = new FakeUserRepository(existing: true);
        var uow = new FakeUnitOfWork();
        var bootstrapper = new SystemUserBootstrapper(repository, uow);

        var result = await bootstrapper.BootstrapAsync();

        result.IsCreated.Should().BeFalse();
        result.SystemUser.Should().BeNull();
        repository.AddedUser.Should().BeNull();
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly bool _existing;

        public FakeUserRepository(bool existing)
        {
            _existing = existing;
        }

        public User? AddedUser { get; private set; }

        public Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<User?>(_existing ? User.CreateSystemUser(userId) : null);
        }

        public Task<bool> ExistsByTypeAsync(UserType type, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_existing && type == UserType.SystemUser);
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            AddedUser = user;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public Task<IReadOnlyCollection<User>> GetByTenantIdAsync(string tenantId, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
