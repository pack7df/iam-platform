using IamPlatform.Domain.Identity;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Identity;

public sealed record InviteSystemUserCommand(string InvitationId, string InvitedSystemUserId) : IRequest<SystemUserInvitationResult>;

public sealed class InviteSystemUserHandler : IRequestHandler<InviteSystemUserCommand, SystemUserInvitationResult>
{
    private readonly IInvitationRepository _invitationRepository;

    public InviteSystemUserHandler(IInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task<SystemUserInvitationResult> Handle(InviteSystemUserCommand request, CancellationToken cancellationToken)
    {
        var invitation = Invitation.InviteSystemUser(request.InvitationId, request.InvitedSystemUserId);
        await _invitationRepository.AddAsync(invitation, cancellationToken);

        return new SystemUserInvitationResult(invitation);
    }
}
