using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using MediatR;

namespace IamPlatform.Application.Authorization.Roles;

public sealed class AssignRoleToUserHandler : IRequestHandler<AssignRoleToUserCommand>
{
    private readonly IUserRoleAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleToUserHandler(
        IUserRoleAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
        {
            throw new KeyNotFoundException($"Role with ID {request.RoleId} not found.");
        }

        var assignment = UserRoleAssignment.Assign(Guid.NewGuid().ToString(), user, role);
        await _assignmentRepository.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
