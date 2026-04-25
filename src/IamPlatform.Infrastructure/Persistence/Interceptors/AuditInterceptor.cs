using IamPlatform.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace IamPlatform.Infrastructure.Persistence.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;

    public AuditInterceptor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null) return base.SavingChanges(eventData, result);

        OnBeforeSaveChanges(eventData.Context);
        
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        OnBeforeSaveChanges(eventData.Context);
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void OnBeforeSaveChanges(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        var userId = _userContext.UserId;
        var entries = context.ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            ProcessEntry(context, entry, userId);
        }
    }

    private void ProcessEntry(DbContext context, EntityEntry entry, Guid? userId)
    {
        if (entry.Entity is AuditLog || entry.State is EntityState.Detached or EntityState.Unchanged)
        {
            return;
        }

        UpdateAuditFields(entry, userId);

        var auditLog = GenerateAuditLog(entry, userId);
        if (auditLog != null)
        {
            context.Set<AuditLog>().Add(auditLog);
        }
    }

    private void UpdateAuditFields(EntityEntry entry, Guid? userId)
    {
        if (entry.Entity is not BaseEntity baseEntity) return;

        var now = DateTime.UtcNow;

        if (entry.State == EntityState.Added)
        {
            baseEntity.CreatedAt = now;
            baseEntity.CreatedBy = userId;
        }

        if (entry.State == EntityState.Modified)
        {
            baseEntity.UpdatedAt = now;
            baseEntity.UpdatedBy = userId;
        }
    }

    private AuditLog? GenerateAuditLog(EntityEntry entry, Guid? userId)
    {
        var oldValues = new Dictionary<string, object?>();
        var newValues = new Dictionary<string, object?>();
        var keyValues = new Dictionary<string, object?>();
        var auditType = string.Empty;

        foreach (var property in entry.Properties)
        {
            ExtractPropertyChanges(entry, property, oldValues, newValues, keyValues, ref auditType);
        }

        if (string.IsNullOrEmpty(auditType)) return null;

        var entityId = keyValues.Values.FirstOrDefault() is Guid guid ? guid : Guid.Empty;
        var changes = JsonSerializer.Serialize(new { Old = oldValues, New = newValues });

        return new AuditLog(entry.Entity.GetType().Name, entityId, auditType, userId, changes);
    }

    private void ExtractPropertyChanges(
        EntityEntry entry,
        PropertyEntry property,
        Dictionary<string, object?> oldValues,
        Dictionary<string, object?> newValues,
        Dictionary<string, object?> keyValues,
        ref string auditType)
    {
        var propertyName = property.Metadata.Name;

        if (property.Metadata.IsPrimaryKey())
        {
            keyValues[propertyName] = property.CurrentValue;
            return;
        }

        switch (entry.State)
        {
            case EntityState.Added:
                auditType = "Insert";
                newValues[propertyName] = property.CurrentValue;
                break;

            case EntityState.Deleted:
                auditType = "Delete";
                oldValues[propertyName] = property.OriginalValue;
                break;

            case EntityState.Modified:
                if (property.IsModified)
                {
                    auditType = "Update";
                    oldValues[propertyName] = property.OriginalValue;
                    newValues[propertyName] = property.CurrentValue;
                }
                break;
        }
    }
}


