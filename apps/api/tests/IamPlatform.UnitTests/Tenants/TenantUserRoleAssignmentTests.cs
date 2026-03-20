using FluentAssertions;
using IamPlatform.Domain.Tenants;
using Xunit;

namespace IamPlatform.UnitTests.Tenants;

public sealed class TenantUserRoleAssignmentTests
{
    [Fact]
    public void Assign_Should_Set_User_And_Role_Ids_When_Both_Belong_To_Same_Tenant()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");

        var assignment = TenantUserRoleAssignment.Assign("assignment-001", tenantUser, role);

        assignment.Id.Should().Be("assignment-001");
        assignment.TenantUserId.Should().Be("tenant-user-001");
        assignment.RoleId.Should().Be("role-001");
        assignment.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Assign_Should_Reject_When_TenantUser_And_Role_Belong_To_Different_Tenants()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-002", "Operators");

        var act = () => TenantUserRoleAssignment.Assign("assignment-001", tenantUser, role);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tenant user and role must belong to the same tenant.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Assign_Should_Reject_Invalid_Id(string invalidId)
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");

        var act = () => TenantUserRoleAssignment.Assign(invalidId, tenantUser, role);

        act.Should().Throw<ArgumentException>()
            .WithMessage("Tenant user role assignment id is required.*");
    }

    [Fact]
    public void Deactivate_Should_Set_Inactive_Status()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var assignment = TenantUserRoleAssignment.Assign("assignment-001", tenantUser, role);

        assignment.Deactivate();

        assignment.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_Set_Active_Status()
    {
        var tenantUser = TenantUser.Create("tenant-user-001", "tenant-001", TenantUserType.EndUser);
        var role = Role.Create("role-001", "tenant-001", "Operators");
        var assignment = TenantUserRoleAssignment.Assign("assignment-001", tenantUser, role);
        assignment.Deactivate();

        assignment.Activate();

        assignment.IsActive.Should().BeTrue();
    }
}
