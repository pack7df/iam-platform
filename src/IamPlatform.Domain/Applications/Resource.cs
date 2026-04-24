using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Applications;

public class Resource : BaseEntity, IApplicationEntity
{
    public Guid ApplicationId { get; set; }
    public Guid? ParentId { get; set; }
    
    public required string Name { get; set; }
    public required string Key { get; set; } // e.g. "orders", "reports/monthly"
    public string? Description { get; set; }

    private Resource() { }

    [SetsRequiredMembers]
    public Resource(Guid applicationId, string name, string key, Guid? parentId = null)
    {
        ApplicationId = applicationId;
        Name = name;
        Key = key;
        ParentId = parentId;
    }

}
