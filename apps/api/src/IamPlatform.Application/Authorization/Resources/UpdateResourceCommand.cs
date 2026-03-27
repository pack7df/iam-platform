using System;
using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;

namespace IamPlatform.Application.Authorization.Resources;

public sealed class UpdateResourceCommand
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}

public interface IUpdateResourceHandler
{
    Task HandleAsync(UpdateResourceCommand command, CancellationToken cancellationToken = default);
}

public sealed class UpdateResourceHandler : IUpdateResourceHandler
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResourceHandler(IResourceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UpdateResourceCommand command, CancellationToken cancellationToken = default)
    {
        var resource = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (resource == null)
        {
            throw new InvalidOperationException($"Resource with ID {command.Id} not found.");
        }

        resource.Rename(command.Name);
        await _repository.UpdateAsync(resource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
