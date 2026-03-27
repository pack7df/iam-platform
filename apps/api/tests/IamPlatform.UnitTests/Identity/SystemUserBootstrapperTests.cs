using FluentAssertions;
using IamPlatform.Application.Identity;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Identity;

public sealed class SystemUserBootstrapperTests
{
    [Fact]
    public async Task Handle_Should_Create_SystemUser_When_None_Exists()
    {
        var repository = new FakeUserRepository(existing: false);
        var uow = new FakeUnitOfWork();
        var handler = new BootstrapSystemUserHandler(repository, uow);
        var command = new BootstrapSystemUserCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsCreated.Should().BeTrue();
        result.User.Should().NotBeNull();
        result.User!.Id.Should().NotBeNullOrWhiteSpace();
        result.User.Type.Should().Be(UserType.SystemUser);
        repository.AddedUser.Should().NotBeNull();
        repository.AddedUser!.Id.Should().Be(result.User.Id);
        repository.AddedUser.Type.Should().Be(UserType.SystemUser);
        uow.SaveChangesCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Reject_When_SystemUser_Already_Exists()
    {
        var repository = new FakeUserRepository(existing: true);
        var uow = new FakeUnitOfWork();
        var handler = new BootstrapSystemUserHandler(repository, uow);
        var command = new BootstrapSystemUserCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsCreated.Should().BeFalse();
        result.User.Should().BeNull();
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
