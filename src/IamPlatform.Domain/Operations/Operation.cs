using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Operations;

public class Operation : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    
    public required string Name { get; set; }
    public required string Key { get; set; } // e.g. "read", "write", "admin"
    public string? Description { get; set; }

    private Operation() { }

    [SetsRequiredMembers]
    public Operation(Guid tenantId, string name, string key)
    {
        TenantId = tenantId;
        Name = name;
        Key = key;
    }
}

