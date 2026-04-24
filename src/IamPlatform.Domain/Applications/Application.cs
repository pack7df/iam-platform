using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Applications;

public class Application : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }

    private Application() { }

    [SetsRequiredMembers]
    public Application(Guid tenantId, string name, string slug)
    {
        TenantId = tenantId;
        Name = name;
        Slug = slug;
    }
}
