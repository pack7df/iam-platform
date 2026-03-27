using System.Threading;
using System.Threading.Tasks;
using IamPlatform.Domain.Authorization;
using IamPlatform.Domain.Common;

namespace IamPlatform.Application.Authorization.Resources;

public interface IDeleteResourceHandler
{
    Task HandleAsync(string id, CancellationToken cancellationToken = default);
}

public sealed class DeleteResourceHandler : IDeleteResourceHandler
{
    private readonly IResourceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResourceHandler(IResourceRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(string id, CancellationToken cancellationToken = default)
    {
        var resource = await _repository.GetByIdAsync(id, cancellationToken);
        if (resource == null)
        {
            return;
        }

        await _repository.RemoveAsync(resource, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
