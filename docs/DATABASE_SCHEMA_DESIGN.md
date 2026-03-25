# Diseño de Esquema de Base de Datos - IAM Platform

**Fecha:** 2026-03-24  
**Estado:** Propuesta inicial  
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
- 1:N → users
- 1:N → roles
- 1:N → applications

---

### 2. system_users

Usuarios globales de la plataforma (administradores del sistema).

```sql
CREATE TABLE system_users (
    id VARCHAR(50) PRIMARY KEY,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_system_users_active ON system_users(is_active);
```

**Nota:** No pertenecen a ningún tenant.

---

### 3. users

Usuarios de tenant (incluye TenantAdmin, EndUser, ServiceAdmin).

```sql
CREATE TABLE users (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50) NOT NULL,
    type VARCHAR(20) NOT NULL CHECK (type IN ('TenantAdmin', 'EndUser', 'ServiceAdmin')),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_users_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

CREATE INDEX idx_users_tenant ON users(tenant_id);
CREATE INDEX idx_users_type ON users(type);
CREATE UNIQUE INDEX uq_users_tenant_id ON users(tenant_id, id); -- opcional: id único por tenant
```

---

### 4. roles

Roles definidos dentro de un tenant.

```sql
CREATE TABLE roles (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_roles_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

CREATE INDEX idx_roles_tenant ON roles(tenant_id);
CREATE INDEX idx_roles_active ON roles(is_active);
```

---

### 5. applications

Aplicaciones registradas dentro de un tenant.

```sql
CREATE TABLE applications (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_applications_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

CREATE INDEX idx_applications_tenant ON applications(tenant_id);
```

---

### 6. resources

Recursos de seguridad dentro de una aplicación. Soporta jerarquía通过 `parent_id`.

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
CREATE UNIQUE INDEX uq_resources_application_key ON resources(application_id, key); -- key único por app
```

**Nota:** `parent_id` usa `ON DELETE RESTRICT` para evitar eliminar un recurso que tiene hijos.

---

### 7. operations

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

### 8. authorization_rules

Reglas de autorización que determinan accesso.

```sql
CREATE TABLE authorization_rules (
    id VARCHAR(50) PRIMARY KEY,
    tenant_id VARCHAR(50) NOT NULL,
    resource_id VARCHAR(50) NOT NULL,
    operation_id VARCHAR(50) NOT NULL,
    user_id VARCHAR(50), -- NULL si la regla aplica por rol
    role_id VARCHAR(50), -- NULL si la regla aplica por usuario
    decision VARCHAR(10) NOT NULL CHECK (decision IN ('Allow', 'Deny', 'Inherit')),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_authorization_rules_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE,
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

CREATE INDEX idx_authorization_rules_tenant ON authorization_rules(tenant_id);
CREATE INDEX idx_authorization_rules_resource_operation ON authorization_rules(resource_id, operation_id);
CREATE INDEX idx_authorization_rules_user ON authorization_rules(user_id) WHERE user_id IS NOT NULL;
CREATE INDEX idx_authorization_rules_role ON authorization_rules(role_id) WHERE role_id IS NOT NULL;
CREATE INDEX idx_authorization_rules_active ON authorization_rules(is_active);
```

**Constraints importantes:**
- `chk_authorization_rules_target`: al menos uno de `user_id` o `role_id` debe estar presente (ambos pueden estar)
- Foreign keys aseguran integridad referencial
- Índices parciales (partial indexes) para queries eficientes

---

### 9. user_role_assignments

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

### 10. invitations

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
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    
    CONSTRAINT fk_invitations_tenant FOREIGN KEY (tenant_id) REFERENCES tenants(id) ON DELETE CASCADE
);

CREATE INDEX idx_invitations_status ON invitations(status);
CREATE INDEX idx_invitations_identity ON invitations(invited_identity_id);
CREATE INDEX idx_invitations_tenant ON invitations(tenant_id);
```

**Nota:** `tenant_id` es NULL para `SystemUser` (global), obligatorio para `TenantAdmin`.

---

### 11. audit_logs

Registro de eventos de auditoría (cambios administrativos, logins, consultas de autorización).

```sql
CREATE TABLE audit_logs (
    id BIGSERIAL PRIMARY KEY,
    actor_id VARCHAR(50) NOT NULL, -- SystemUser id o User id
    actor_type VARCHAR(20) NOT NULL CHECK (actor_type IN ('SystemUser', 'TenantUser', 'ServiceAdmin')),
    tenant_id VARCHAR(50), -- NULL para SystemUser
    
    action VARCHAR(50) NOT NULL, -- ej: 'UserCreated', 'ResourceUpdated', 'AuthorizationEvaluated'
    target_type VARCHAR(50), -- ej: 'User', 'Resource', 'AuthorizationRule'
    target_id VARCHAR(50), -- id del objeto afectado
    
    result VARCHAR(20) NOT NULL CHECK (result IN ('Success', 'Failure')),
    
    details JSONB, -- detalles estructurados (valores antiguos/nuevos, contexto)
    
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_audit_logs_actor ON audit_logs(actor_id, actor_type);
CREATE INDEX idx_audit_logs_tenant ON audit_logs(tenant_id);
CREATE INDEX idx_audit_logs_action ON audit_logs(action);
CREATE INDEX idx_audit_logs_created ON audit_logs(created_at DESC);
CREATE INDEX idx_audit_logs_target ON audit_logs(target_type, target_id);
```

**Nota:** Usamos `JSONB` para detalles flexibles (antes/después, cambios de campos).

---

## Secuencias de IDs

Todos los `id` VARCHAR se generan en la aplicación (UUIDs o IDs legibles). No usamos serial/auto-increment.

---

## Consideraciones de Diseño

### Multi-tenancy
- Todas las tablas excepto `system_users` tienen `tenant_id` (directo o indirecto via FK)
- Foreign keys con `ON DELETE CASCADE` para limpieza automática al eliminar un tenant
- Índices en `tenant_id` para filtrado rápido por tenant

### Jerarquía de Recursos
- `resources.parent_id` se apunta a sí mismo (relación recursiva)
- `ON DELETE RESTRICT` evita eliminar padre con hijos
- La lógica de herencia se resuelve en aplicación (recorrer ParentId)

### Autorización
- Tabla `authorization_rules` permite:
  - Reglas para usuario específico (`user_id` not null, `role_id` null)
  - Reglas para rol (`role_id` not null, `user_id` null)
  - Reglas para ambos (`user_id` y `role_id` not null)
- Check constraint asegura que al menos uno esté presente
- Índices parciales optimizan queries por usuario o por rol

### Auditoría
- Tabla `audit_logs` con `JSONB` para flexibilidad
- Índices en actor, tenant, acción, fecha
- `actor_type` distingue entre SystemUser, TenantUser y ServiceAdmin

### Timestamps
- Todas las tablas tienen `created_at` y `updated_at`
- Default `NOW()` (PostgreSQL) para `created_at`
- `updated_at` debe actualizarse via trigger o desde aplicación

---

## Migración Inicial

Esta es la propuesta para la **migración InitialCreate** en EF Core.

### Orden de creación (por dependencias):

1. `tenants`
2. `system_users`
3. `users` (depende de tenants)
4. `roles` (depende de tenants)
5. `applications` (depende de tenants)
6. `resources` (depende de applications, y self-referencing)
7. `operations` (depende de applications)
8. `authorization_rules` (depende de tenants, resources, operations, users, roles)
9. `user_role_assignments` (depende de users, roles)
10. `invitations` (depende de tenants, users indirectamente)
11. `audit_logs` (sin dependencias fuertes)

---

## Preguntas Abiertas

1. **UUIDs vs IDs legibles**: ¿Usamos UUIDs v4 o IDs legibles como `tenant-001`? (Decisión pendiente)
2. **Soft delete**: ¿Debemos agregar `deleted_at` en lugar de `ON DELETE CASCADE`? (Por ahora, hard delete en cascade)
3. **Updated_at automático**: ¿Trigger en BD o actualización manual desde código? (Sugerencia: trigger)
4. **Auditoría de evaluaciones**: ¿Registramos cada evaluación de autorización? (Costoso, puede ser optativo)
5. **Índices adicionales**: ¿Necesitamos más índices compuestos para queries frecuentes? (Revisar después de pruebas de carga)

---

## Próxima Tarea

**Tarea 3.2**: Crear `IamPlatformDbContext` y configuraciones Fluent API para este esquema.

---

**Autor:** Asistente AI (opencode)  
**Revisión:** Pendiente
