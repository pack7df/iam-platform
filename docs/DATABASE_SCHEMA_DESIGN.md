# Diseño de Esquema de Base de Datos - IAM Platform

**Fecha:** 2026-03-24  
**Estado:** Propuesta inicial (revisada)  
**Tarea:** 3.1 - Diseñar esquema de base de datos  
**Motor:** PostgreSQL 15+  
**ORM:** Entity Framework Core 8+ con Npgsql

---

## Visión General

El esquema refleja el modelo de dominio completo del IAM, incluyendo:

- Multi-tenancy con aislamiento estricto
- Jerarquía de recursos (árbol)
- Sistema de autorización basado en reglas (RBAC extendido)
- Invitaciones pendientes y aceptadas
- Auditoría de cambios importantes

**Nota importante sobre usuarios:**
- **SystemUser** y **TenantUser** están unificados en una sola tabla `users`
- `tenant_id` es **NULLABLE**:
  - `NULL` → SystemUser (administrador global)
  - `NOT NULL` → TenantUser (pertenece a un tenant)
- El campo `type` distingue: `'SystemUser'`, `'TenantAdmin'`, `'EndUser'`, `'ServiceAdmin'`

---

## Diagrama de Entidades (Tablas)

### 1. tenants

Almacena los tenants (organizaciones) que usan la plataforma.

```sql
CREATE TABLE tenants (
    id VARCHAR(50) PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_tenants_active ON tenants(is_active);
```

**Relaciones:**
- 1:N → users (via `users.tenant_id` cuando no es NULL)
- 1:N → roles
- 1:N → applications

---

### 2. users

Usuarios del sistema **unificados**:

- **SystemUser**: `tenant_id = NULL`, `type = 'SystemUser'`
- **TenantUser**: `tenant_id = <tenant>`, `type = 'TenantAdmin' | 'EndUser' | 'ServiceAdmin'`

```sql
CREATE TABLE users (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50), -- NULL para SystemUser, NOT NULL para TenantUsers
    type VARCHAR(20) NOT NULL CHECK (type IN ('SystemUser', 'TenantAdmin', 'EndUser', 'ServiceAdmin')),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_users_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

-- Índice para búsqueda de usuarios por tenant (solo los que tienen tenant)
CREATE INDEX idx_users_tenant ON users(tenant_id) WHERE tenant_id IS NOT NULL;
-- Índice para búsqueda por tipo
CREATE INDEX idx_users_type ON users(type);
```

**Nota:** `tenant_id` puede ser NULL (SystemUser). Para TenantUsers debe ser NOT NULL. Esto se puede enforcing:
- Con **trigger** que verifique: si `type != 'SystemUser'` entonces `tenant_id IS NOT NULL`
- O desde la capa de aplicación (Domain)

---

### 3. roles

Roles definidos dentro de un tenant.

```sql
CREATE TABLE roles (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_roles_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

CREATE INDEX idx_roles_tenant ON roles(tenant_id);
CREATE INDEX idx_roles_active ON roles(is_active);
```

---

### 4. applications

Aplicaciones registradas dentro de un tenant.

```sql
CREATE TABLE applications (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_applications_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

CREATE INDEX idx_applications_tenant ON applications(tenant_id);
```

---

### 5. resources

Recursos de seguridad dentro de una aplicación. Soporta jerarquía a través de `parent_id`.

```sql
CREATE TABLE resources (
    id VARCHAR(50) PRIMARY KEY,
    application_id VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    key VARCHAR(100) NOT NULL,
    parent_id VARCHAR(50), -- NULL para recurso raíz
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_resources_application FOREIGN KEY (application_id) REFERENCES applications(id) ON DELETE CASCADE,
    CONSTRAINT fk_resources_parent FOREIGN KEY (parent_id) REFERENCES resources(id) ON DELETE RESTRICT
);

CREATE INDEX idx_resources_application ON resources(application_id);
CREATE INDEX idx_resources_parent ON resources(parent_id);
CREATE INDEX idx_resources_active ON resources(is_active);
CREATE UNIQUE INDEX uq_resources_application_key ON resources(application_id, key);
```

**Nota:** `parent_id` usa `ON DELETE RESTRICT` para evitar eliminar un recurso que tiene hijos.

---

### 6. operations

Operaciones que se pueden realizar sobre un recurso.

```sql
CREATE TABLE operations (
    id VARCHAR(50) PRIMARY KEY,
    application_id VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    key VARCHAR(100) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_operations_application FOREIGN KEY (application_id) REFERENCES applications(id) ON DELETE CASCADE
);

CREATE INDEX idx_operations_application ON operations(application_id);
CREATE INDEX idx_operations_active ON operations(is_active);
CREATE UNIQUE INDEX uq_operations_application_key ON operations(application_id, key);
```

---

### 7. authorization_rules

Reglas de autorización que determinan el acceso.

**Sin `tenant_id`** (se deriva de las relaciones).  
`user_id` en lugar de `tenant_user_id`.

```sql
CREATE TABLE authorization_rules (
    id VARCHAR(50) PRIMARY KEY,
    resource_id VARCHAR(50) NOT NULL,
    operation_id VARCHAR(50) NOT NULL,
    user_id VARCHAR(50), -- NULL si la regla aplica por rol
    role_id VARCHAR(50), -- NULL si la regla aplica por usuario
    decision VARCHAR(10) NOT NULL CHECK (decision IN ('Allow', 'Deny', 'Inherit')),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_authorization_rules_resource FOREIGN KEY (resource_id) REFERENCES resources(id) ON DELETE CASCADE,
    CONSTRAINT fk_authorization_rules_operation FOREIGN KEY (operation_id) REFERENCES operations(id) ON DELETE CASCADE,
    CONSTRAINT fk_authorization_rules_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_authorization_rules_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE,
    CONSTRAINT chk_authorization_rules_target CHECK (
        (user_id IS NOT NULL AND role_id IS NULL) OR
        (user_id IS NULL AND role_id IS NOT NULL) OR
        (user_id IS NOT NULL AND role_id IS NOT NULL)
    )
);

CREATE INDEX idx_authorization_rules_resource_operation ON authorization_rules(resource_id, operation_id);
CREATE INDEX idx_authorization_rules_user ON authorization_rules(user_id) WHERE user_id IS NOT NULL;
CREATE INDEX idx_authorization_rules_role ON authorization_rules(role_id) WHERE role_id IS NOT NULL;
CREATE INDEX idx_authorization_rules_active ON authorization_rules(is_active);
```

---

### 8. user_role_assignments

Asignación de roles a usuarios (relación N:M).

```sql
CREATE TABLE user_role_assignments (
    id VARCHAR(50) PRIMARY KEY,
    user_id VARCHAR(50) NOT NULL,
    role_id VARCHAR(50) NOT NULL,
    assigned_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_user_role_assignments_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_role_assignments_role FOREIGN KEY (role_id) REFERENCES roles(id) ON DELETE CASCADE,
    CONSTRAINT uq_user_role_assignment UNIQUE (user_id, role_id)
);

CREATE INDEX idx_user_role_assignments_user ON user_role_assignments(user_id);
CREATE INDEX idx_user_role_assignments_role ON user_role_assignments(role_id);
```

---

### 9. invitations

Invitaciones pendientes para usuarios de sistema y administradores de tenant.

```sql
CREATE TABLE invitations (
    id VARCHAR(50) PRIMARY KEY,
    target_type VARCHAR(20) NOT NULL CHECK (target_type IN ('SystemUser', 'TenantAdmin')),
    invited_identity_id VARCHAR(50) NOT NULL,
    tenant_id VARCHAR(50), -- NULL para SystemUser, NOT NULL para TenantAdmin
    status VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (status IN ('Pending', 'Accepted', 'Expired', 'Cancelled')),
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    accepted_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_invitations_status ON invitations(status);
CREATE INDEX idx_invitations_identity ON invitations(invited_identity_id);
CREATE INDEX idx_invitations_tenant ON invitations(tenant_id) WHERE tenant_id IS NOT NULL;
```

---

### 10. audit_logs

Registro de eventos de auditoría.

```sql
CREATE TABLE audit_logs (
    id BIGSERIAL PRIMARY KEY,
    actor_id VARCHAR(50) NOT NULL, -- User id (puede ser SystemUser o TenantUser)
    actor_type VARCHAR(20) NOT NULL CHECK (actor_type IN ('SystemUser', 'TenantUser', 'ServiceAdmin')),
    tenant_id VARCHAR(50), -- NULL para SystemUser
    
    action VARCHAR(50) NOT NULL,
    target_type VARCHAR(50),
    target_id VARCHAR(50),
    
    result VARCHAR(20) NOT NULL CHECK (result IN ('Success', 'Failure')),
    
    details JSONB,
    
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_audit_logs_actor ON audit_logs(actor_id, actor_type);
CREATE INDEX idx_audit_logs_tenant ON audit_logs(tenant_id);
CREATE INDEX idx_audit_logs_action ON audit_logs(action);
CREATE INDEX idx_audit_logs_created ON audit_logs(created_at DESC);
CREATE INDEX idx_audit_logs_target ON audit_logs(target_type, target_id);
```

---

## Consideraciones Clave

### Unificación de Usuarios
- Una sola tabla `users` para todos los tipos
- `tenant_id` NULL para SystemUser, NOT NULL para TenantUsers
- Validación necesaria: si `type != 'SystemUser'` → `tenant_id IS NOT NULL`

### TenantId en AuthorizationRule
- **NO** se incluye; se deriva de joins:
  - `user_id → users.tenant_id`
  - `role_id → roles.tenant_id`
  - `resource_id → resources → applications.tenant_id`
- Para queries por tenant, se requieren joins. Si performance es crítica, considerar columna redundante + trigger.

### Jerarquía de Recursos
- `resources.parent_id` self-referencing
- `ON DELETE RESTRICT` evita eliminar padre con hijos
- Heirarquis se resuelve en aplicación

---

## Orden de Migración

1. `tenants`
2. `users` (depende de tenants)
3. `roles` (depende de tenants)
4. `applications` (depende de tenants)
5. `resources` (depende de applications, self-reference)
6. `operations` (depende de applications)
7. `authorization_rules` (depende de resources, operations, users, roles)
8. `user_role_assignments` (depende de users, roles)
9. `invitations` (depende de tenants)
10. `audit_logs`

---

## Preguntas Abiertas

1. **Validación UserType vs tenant_id**: ¿Trigger o aplicación?
2. **Shadow properties**: ¿Incluir `created_at`/`updated_at` en entidades o usar shadow en EF Core?
3. **Unique constraint**: ¿Forzar `(tenant_id, id)` único solo para TenantUsers?
4. **Trigger updated_at**: ¿Automático en BD o manual desde código?

---

## Próxima Tarea

**Tarea 3.2**: Crear `IamPlatformDbContext` y configuraciones Fluent API.

---

**Autor:** Asistente AI (opencode)  
**Revisión:** Pendiente  
**Última actualización:** 2026-03-24 (unificación usuarios, eliminar tenant_id de authorization_rules)
