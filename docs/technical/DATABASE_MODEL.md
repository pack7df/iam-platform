# Database Schema Design - IAM Platform

This document outlines the relational database schema for the IAM Platform, ensuring multi-tenant isolation, hierarchical resource management, and auditability.

## 1. General Principles
- **Multi-Tenancy**: Every tenant-specific table contains a `TenantId` column.
- **Soft Deletes**: Entities include `IsDeleted` (Boolean) and `DeletedAt` (Timestamp) to support audit requirements without permanent data loss.
- **Auditing**: Every record includes `CreatedAt`, `CreatedBy`, `UpdatedAt`, and `UpdatedBy`.

## 2. Core Entities

### 2.1 Tenants
Table `Tenants`:
- `Id`: UUID (Primary Key)
- `Name`: String
- `Slug`: String (Unique, used for URL context)
- `Status`: Enum (Active, Suspended, Deleted)
- `IsDeleted`, `DeletedAt`

### 2.2 Users
Table `Users`:
- `Id`: UUID (Primary Key)
- `TenantId`: UUID (Foreign Key to Tenants)
- `Email`: String
- `PasswordHash`: String
- `IsActive`: Boolean
- `IsDeleted`, `DeletedAt`
*Constraint: Unique constraint on `(TenantId, Email)` to allow same email in different tenants.*

### 2.3 Applications
Table `Applications`:
- `Id`: UUID (Primary Key)
- `TenantId`: UUID (Foreign Key to Tenants)
- `Name`: String
- `ClientId`: String (Unique)
- `ClientSecret`: String
- `IsDeleted`, `DeletedAt`

## 3. Authorization Engine

### 3.1 Resources
Table `Resources`:
- `Id`: UUID (Primary Key)
- `ApplicationId`: UUID (Foreign Key to Applications)
- `ParentResourceId`: UUID (Self-referencing Foreign Key, NULL for root)
- `Name`: String
- `Type`: String (e.g., "Folder", "Object", "System")
- `IsDeleted`

### 3.2 Operations
Table `Operations`:
- `Id`: UUID (Primary Key)
- `Name`: String (e.g., "Read", "Write", "Delete", "Admin")

### 3.3 Actions
Table `Actions`:
- `Id`: UUID (Primary Key)
- `ResourceId`: UUID (Foreign Key to Resources)
- `OperationId`: UUID (Foreign Key to Operations)
*Constraint: Unique on `(ResourceId, OperationId)`.*

### 3.4 Roles
Table `Roles`:
- `Id`: UUID (Primary Key)
- `TenantId`: UUID (Foreign Key to Tenants)
- `Name`: String
- `Description`: Text
- `IsDeleted`

### 3.5 Permissions
Table `Permissions`:
- `Id`: UUID (Primary Key)
- `TargetUserId`: UUID (Nullable, Foreign Key to Users)
- `TargetRoleId`: UUID (Nullable, Foreign Key to Roles)
- `ActionId`: UUID (Foreign Key to Actions)
- `Decision`: Enum (Allowed, Denied, Inherited)
*Note: A permission can be assigned to a User, a Role, or both. If both are present, the permission applies **only if the User belongs to that specific Role** (Conditional Permission).*

## 4. Auditing

### 4.1 AuditLogs
Table `AuditLogs`:
- `Id`: UUID (Primary Key)
- `Timestamp`: DateTime
- `UserId`: UUID (Foreign Key to Users - Always mandatory)
- `TenantId`: UUID (Foreign Key to Tenants)
- `ActionName`: String
- `EntityName`: String
- `EntityId`: UUID
- `Changes`: JSONB (Store before/after delta)
- `IpAddress`: String
