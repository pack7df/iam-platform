using IamPlatform.Domain.Tenants;
using MediatR;

namespace IamPlatform.Application.Authorization.Roles;

public sealed class GetUserRolesHandler : IRequestHandler<GetUserRolesQuery, List<UserRoleResponse>>
{
    private readonly IUserRoleAssignmentRepository _assignmentRepository;
    private readonly IRoleRepository _roleRepository;

    public GetUserRolesHandler(
        IUserRoleAssignmentRepository assignmentRepository,
        IRoleRepository roleRepository)
    {
        _assignmentRepository = assignmentRepository;
        _roleRepository = roleRepository;
    }

    public async Task<List<UserRoleResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var roles = new List<UserRoleResponse>();

        foreach (var assignment in assignments)
        {
            var role = await _roleRepository.GetByIdAsync(assignment.RoleId, cancellationToken);
            if (role is not null)
            {
                roles.Add(new UserRoleResponse(role.Id, role.Name));
            }
        }

        return roles;
    }
}
