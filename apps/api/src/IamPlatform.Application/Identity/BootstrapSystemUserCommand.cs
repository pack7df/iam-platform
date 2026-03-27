using IamPlatform.Domain.Common;
using IamPlatform.Domain.Identity;
using IamPlatform.Domain.Tenants;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Identity;

public sealed record BootstrapSystemUserCommand(string? SystemUserId = null) : IRequest<SystemUserBootstrapResult>;

public sealed class BootstrapSystemUserHandler : IRequestHandler<BootstrapSystemUserCommand, SystemUserBootstrapResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;

    public BootstrapSystemUserHandler(IUserRepository userRepository, IUnitOfWork uow)
    {
        _userRepository = userRepository;
        _uow = uow;
    }

    public async Task<SystemUserBootstrapResult> Handle(BootstrapSystemUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByTypeAsync(UserType.SystemUser, cancellationToken))
        {
            return SystemUserBootstrapResult.AlreadyBootstrapped();
        }

        var id = request.SystemUserId ?? Guid.NewGuid().ToString();
        var systemUser = User.CreateSystemUser(id);
        await _userRepository.AddAsync(systemUser, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return SystemUserBootstrapResult.Created(systemUser);
    }
}
