# Coding Conventions

## Principle: One Class/Interface per File

Each file in the codebase should contain exactly **one** primary type declaration (class, interface, enum, struct, record, etc.). This rule applies to:

- Classes
- Interfaces  
- Enums
- Records
- Structs

### Rationale

- **Clarity**: It's immediately clear what a file contains by its name.
- **Maintainability**: Smaller, focused files are easier to navigate, review, and refactor.
- **Git diffs**: Changes affect only one logical component, making version history more meaningful.
- **Searchability**: Finding a type by name is straightforward (filename matches type name).

### Exceptions

- Nested types that are tightly coupled and only used by the containing type may be defined in the same file.
- Small utility methods (extension methods, guard clauses) that logically belong to the primary type.

### Naming Convention

- The file name must **exactly match** the name of the primary type declared within it.
- Example: `UserRepository.cs` contains `public class UserRepository`.
- Example: `IUserRepository.cs` contains `public interface IUserRepository`.

### Enforcement

- This convention is expected to be followed during development.
- Code reviews should verify that each file focuses on a single responsibility.
- If a file grows multiple unrelated types, refactor into separate files.

### Examples

✅ **Good**:
```
User.cs          -> public class User
IUserRepo.cs     -> public interface IUserRepository
UserType.cs      -> public enum UserType
```

❌ **Avoid**:
```
UserAndRole.cs   -> public class User + public class Role (separate files)
Utils.cs         -> multiple unrelated static helper classes
```

---

## Dependency Injection: Always Inject Interfaces, Never Concrete Types

When injecting dependencies via constructors, method parameters, or endpoints, **always use interfaces, never concrete implementations**. This rule applies to:

- Constructor parameters in classes (Application Services, Domain Services, etc.)
- Endpoint parameters in Minimal APIs (Program.cs)
- Controller constructors (if used)
- Service registrations in DependencyInjection classes

### Examples

✅ **Correct:**
```csharp
// Minimal API endpoint
app.MapPost("/system-user-invitations", 
    async (InviteSystemUserRequest request, ISystemUserInvitation invitationService, CancellationToken cancellationToken) => { ... });

// Constructor
public sealed class SystemUserBootstrapper : ISystemUserBootstrapper
{
    public SystemUserBootstrapper(ISystemUserRepository systemUserRepository) { ... }
}

// Registration
services.AddScoped<ISystemUserBootstrapper, SystemUserBootstrapper>();
```

❌ **Avoid:**
```csharp
// BAD: concrete type in endpoint
app.MapPost("/system-user-invitations", 
    async (InviteSystemUserRequest request, SystemUserInvitation invitationService, ...) => { ... });

// BAD: concrete type in constructor
public SystemUserBootstrapper(SystemUserRepository systemUserRepository) { ... }
```

### Rationale

- **Testability**: Interfaces enable easy mocking and stubbing in tests
- **SOLID - Dependency Inversion Principle**: Depend on abstractions, not implementations
- **Flexibility**: Swap implementations without changing consumers
- **Clarity**: Interfaces define explicit contracts
- **Consistency**: Keeps architecture layers clean and decoupled

### Exception

Only concrete types may be injected when:
- The type is a simple DTO/record with no behavior (e.g., `BootstrapSystemUserRequest`, `RegisterTenantAdminRequest`)
- The type is a primitive or built-in .NET type (`string`, `int`, `CancellationToken`, etc.)

---

## Additional Notes

This convention aligns with:
- **SOLID** Single Responsibility & Dependency Inversion Principles
- **Clean Architecture** dependency rule (dependencies point inward)
- **TDD** practices (easy mocking)
- Team development best practices

All code reviews should verify this rule is followed.
