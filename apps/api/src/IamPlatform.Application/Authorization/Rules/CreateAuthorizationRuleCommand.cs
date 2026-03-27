using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;
using IamPlatform.Domain.Tenants;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace IamPlatform.Application.Authorization.Rules;

public sealed record CreateAuthorizationRuleCommand(
    string Id,
    string? UserId,
    string? RoleId,
    string ResourceId,
    string OperationId,
    string Decision) : IRequest;

public sealed class CreateAuthorizationRuleHandler : IRequestHandler<CreateAuthorizationRuleCommand>
{
    private readonly IAuthorizationRuleRepository _ruleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IResourceRepository _resourceRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly IUnitOfWork _uow;

    public CreateAuthorizationRuleHandler(
        IAuthorizationRuleRepository ruleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IResourceRepository resourceRepository,
        IOperationRepository operationRepository,
        IUnitOfWork uow)
    {
        _ruleRepository = ruleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _resourceRepository = resourceRepository;
        _operationRepository = operationRepository;
        _uow = uow;
    }

    public async Task Handle(CreateAuthorizationRuleCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AuthorizationRuleDecision>(request.Decision, true, out var decision))
        {
            decision = AuthorizationRuleDecision.Deny;
        }

        var resource = await _resourceRepository.GetByIdAsync(request.ResourceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Resource {request.ResourceId} not found.");

        var operation = await _operationRepository.GetByIdAsync(request.OperationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Operation {request.OperationId} not found.");

        AuthorizationRule rule;

        if (!string.IsNullOrWhiteSpace(request.UserId) && !string.IsNullOrWhiteSpace(request.RoleId))
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new KeyNotFoundException($"User {request.UserId} not found.");
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
                ?? throw new KeyNotFoundException($"Role {request.RoleId} not found.");

            rule = AuthorizationRule.CreateForUserAndRole(request.Id, user, role, resource, operation, decision);
        }
        else if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new KeyNotFoundException($"User {request.UserId} not found.");

            rule = AuthorizationRule.CreateForUser(request.Id, user, resource, operation, decision);
        }
        else if (!string.IsNullOrWhiteSpace(request.RoleId))
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken)
                ?? throw new KeyNotFoundException($"Role {request.RoleId} not found.");

            rule = AuthorizationRule.CreateForRole(request.Id, role, resource, operation, decision);
        }
        else
        {
            throw new InvalidOperationException("Authorization rule must target at least one user or role.");
        }

        await _ruleRepository.AddAsync(rule, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
