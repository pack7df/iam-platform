namespace IamPlatform.Domain.Common;

public class AuditLog
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EntityName { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public Guid? UserId { get; private set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    public string Changes { get; private set; } = string.Empty;

    private AuditLog() { }

    public AuditLog(string entityName, Guid entityId, string action, Guid? userId, string changes)
    {
        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        UserId = userId;
        Changes = changes;
    }
}
