using IamPlatform.Domain.Common;
using IamPlatform.Domain.Identity;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Tenants;

public sealed record InviteTenantAdminCommand(string InvitationId, string TenantId, string InvitedTenantAdminId) : IRequest<TenantAdminInvitationResult>;

public sealed class InviteTenantAdminHandler : IRequestHandler<InviteTenantAdminCommand, TenantAdminInvitationResult>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IUnitOfWork _uow;

    public InviteTenantAdminHandler(IInvitationRepository invitationRepository, IUnitOfWork uow)
    {
        _invitationRepository = invitationRepository;
        _uow = uow;
    }

    public async Task<TenantAdminInvitationResult> Handle(InviteTenantAdminCommand request, CancellationToken cancellationToken)
    {
        var invitation = Invitation.InviteTenantAdmin(request.InvitationId, request.TenantId, request.InvitedTenantAdminId);
        await _invitationRepository.AddAsync(invitation, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new TenantAdminInvitationResult(invitation);
    }
}
