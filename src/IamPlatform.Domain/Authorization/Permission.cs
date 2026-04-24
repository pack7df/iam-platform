using IamPlatform.Domain.Common;

namespace IamPlatform.Domain.Authorization;

public class Permission : BaseEntity
{
    public Guid ActionId { get; set; }
    
    // Exactly one of these should be populated
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    
    public PermissionOutcome Outcome { get; set; }

    private Permission() { }

    public Permission(Guid actionId, PermissionOutcome outcome, Guid? userId = null, Guid? roleId = null)
    {
        if (userId == null && roleId == null)
            throw new ArgumentException("A permission must be assigned to either a User or a Role.");
            
        if (userId != null && roleId != null)
            throw new ArgumentException("A permission cannot be assigned to both a User and a Role simultaneously.");

        ActionId = actionId;
        Outcome = outcome;
        UserId = userId;
        RoleId = roleId;
    }
}
