using System.Diagnostics.CodeAnalysis;
using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Tenants;

public enum TenantStatus
{
    Active,
    Suspended,
    Deleted
}

public class Tenant : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;

    private Tenant() { } // For EF Core

    [SetsRequiredMembers]
    public Tenant(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }
}

