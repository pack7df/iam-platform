using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class UserRoleAssignmentTests
{
    [Fact]
    public void Assign_Should_Set_User_And_Role_Ids_When_Both_Belong_To_Same_Tenant()
    {
        var tenantUser = User.Create("tenant-user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");

        var assignment = UserRoleAssignment.Assign("assignment-001", tenantUser, role);

        assignment.Id.Should().Be("assignment-001");
        assignment.UserId.Should().Be("tenant-user-001");
        assignment.RoleId.Should().Be("role-001");
        assignment.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Assign_Should_Reject_When_User_And_Role_Belong_To_Different_Tenants()
    {
        var tenantUser = User.Create("tenant-user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-002", "Operators");

        var act = () => UserRoleAssignment.Assign("assignment-001", tenantUser, role);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tenant user and role must belong to the same tenant.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Assign_Should_Reject_Invalid_Id(string invalidId)
    {
        var tenantUser = User.Create("tenant-user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");

        var act = () => UserRoleAssignment.Assign(invalidId, tenantUser, role);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant user role assignment id is required.*");
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var tenantUser = User.Create("tenant-user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var assignment = UserRoleAssignment.Assign("assignment-001", tenantUser, role);

        assignment.Deactivate();

        assignment.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var tenantUser = User.Create("tenant-user-001", "tenant-001", UserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var assignment = UserRoleAssignment.Assign("assignment-001", tenantUser, role);
        assignment.Deactivate();

        assignment.Activate();

        assignment.IsActive.Should().BeTrue();
    }
}
