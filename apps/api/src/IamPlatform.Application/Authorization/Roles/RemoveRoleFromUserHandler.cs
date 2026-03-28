using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using MediatR;

namespace IamPlatform.Application.Authorization.Roles;

public sealed class RemoveRoleFromUserHandler : IRequestHandler<RemoveRoleFromUserCommand>
{
    private readonly IUserRoleAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromUserHandler(
        IUserRoleAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var assignment = assignments.FirstOrDefault(a => a.RoleId == request.RoleId);
        
        if (assignment is not null)
        {
            await _assignmentRepository.RemoveAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
