using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Tenants;

public class Role : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    
    public required string Name { get; set; }
    public string? Description { get; set; }

    private Role() { }

    [SetsRequiredMembers]
    public Role(Guid tenantId, string name)
    {
        TenantId = tenantId;
        Name = name;
    }
}
