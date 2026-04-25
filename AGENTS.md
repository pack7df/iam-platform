# Agent Guide - IAM Platform

This repository contains the **IAM Platform**, a multi-tenant Identity and Access Management system built with .NET 9 and React.

## 🏗️ Architecture & Structure

The project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles.

- **src/IamPlatform.Domain**: Core business logic, entities, and repository interfaces.
  - Entities inherit from `BaseEntity`.
  - Scoping interfaces: `ITenantEntity`, `IApplicationEntity`, `IResourceEntity`.
- **src/IamPlatform.Infrastructure**: Data persistence (EF Core + PostgreSQL), repository implementations, and external services.
  - **Repositories**: Follow a strict scoping hierarchy (Tenant -> Application -> Resource).
  - **BaseRepository**: Generic CRUD operations.
  - **ScopedRepositories**: (e.g., `TenantScopedRepository<T>`) Automatically enforce scope on `Add` and `Get` operations.
- **src/IamPlatform.Application**: Use cases, CQRS commands/queries (MediatR), and business services.
- **src/IamPlatform.Api**: ASP.NET Core Web API, controllers, and middleware.
- **tests/**:
  - **IntegrationTests**: Uses `Testcontainers` (PostgreSQL) and `Respawn` for database isolation.
  - **UnitTests**: Focused on domain logic and mapping.

## 🛠️ Essential Commands

- **Build**: `dotnet build`
- **Test**: `dotnet test`
- **Migrations**: `dotnet ef migrations add <Name> --project src/IamPlatform.Infrastructure --startup-project src/IamPlatform.Api`
- **Run API**: `dotnet run --project src/IamPlatform.Api`

## 📝 Conventions & Patterns

### 1. Repository Access & Scoping (Critical)
The codebase uses a **Hierarchical Repository Builder** pattern. Instead of injecting all repositories via DI, you typically start with a root repository and "drill down" to more specific ones. This enforces structural scoping (Multi-tenancy).

**The Hierarchy Flow:**
1.  **`ITenantRepository` (Root)**: Injected via DI. Used to access `Tenants`.
    -   `GetUserRepository(tenantId)` -> `IUserRepository`
    -   `GetRoleRepository(tenantId)` -> `IRoleRepository`
    -   `GetApplicationRepository(tenantId)` -> `IApplicationRepository`
2.  **`IApplicationRepository`**:
    -   `GetResourceRepository(applicationId)` -> `IResourceRepository`
3.  **`IResourceRepository`**:
    -   `GetActionRepository(resourceId)` -> `IActionRepository`
4.  **`IActionRepository`**:
    -   `GetPermissionRepository(actionId)` -> `IPermissionRepository`

**Why this matters:**
-   **Enforced Scope**: Scoped repositories (inheriting from `TenantScopedRepository`, `ApplicationScopedRepository`, etc.) automatically set the parent ID during `AddAsync` and filter by it during `Get` operations.
-   **No "Naked" access**: You cannot (and should not) instantiate a `UserRepository` without a `tenantId`.

**Gotcha**: Scoped repositories enforce the `ScopeId` on `AddAsync`. You must provide the `ScopeId` via the constructor (usually handled by the parent repository's factory method).

### 2. Entity Lifecycle
All entities inherit from `BaseEntity` which includes:
- `Id` (Guid, auto-generated)
- `CreatedAt` / `UpdatedAt`
- `IsDeleted` / `DeletedAt` (Soft delete support)

### 3. Naming & Style
- Follow **Technical English** for all code and documentation (per `STYLEGUIDE.md`).
- Use **C# Coding Conventions** (PascalCase for methods/classes, _camelCase for private fields).
- Entities in `IamPlatformDbContext` are explicitly scoped to avoid naming collisions (e.g., `IamPlatform.Domain.Applications.Action`).

### 4. Integration Testing
- Inherit from `BaseIntegrationTest`.
- Each test run starts a fresh PostgreSQL container.
- Use `await ResetDatabaseAsync()` if you need a clean state within the same test class.

## ⚠️ Important Gotchas

- **MediatR**: Commands and Queries are the primary way to interact with the application layer. Check `IamPlatform.Application` for existing handlers.
- **EF Core Filters**: While scoped repositories exist, always double-check if a global query filter is also needed in `IamPlatformDbContext` for soft deletes or multi-tenancy.
- **Shadow Properties**: Be aware of shadow properties if they are used for auditing in `OnModelCreating`.

## 📂 Key Files
- `docs/technical/DATABASE_MODEL.md`: ERD and schema details.
- `docs/technical/TECH_STACK.md`: Official tech stack.
- `STYLEGUIDE.md`: Language and formatting rules.
- `src/IamPlatform.Infrastructure/Persistence/IamPlatformDbContext.cs`: EF Core configuration.
