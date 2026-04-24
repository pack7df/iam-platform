using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Applications;

public class Action : BaseEntity, IResourceEntity
{
    public Guid ResourceId { get; set; }

    public Guid OperationId { get; set; }

    private Action() { }

    [SetsRequiredMembers]
    public Action(Guid resourceId, Guid operationId)
    {
        ResourceId = resourceId;
        OperationId = operationId;
    }
}
