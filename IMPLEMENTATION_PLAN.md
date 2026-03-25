# IAM Platform - Plan de Implementacion Inicial

## Objetivo del plan
- Entregar una primera version funcional que valide el core del dominio.
- Priorizar el modelo de tenants, usuarios, roles, aplicaciones, recursos, operaciones y reglas.
- Implementar los flujos secundarios de la forma mas simple posible para comprobar que el core funciona.
- Mantener TDD, DDD, SOLID y PRs pequenos durante toda la implementacion.

## Principios de ejecucion
- El foco principal es el backend y el modelo de dominio.
- La UI inicial debe ser funcional, no sofisticada.
- Cada tarea debe producir un PR pequeno y revisable.
- Objetivo normal por PR: hasta 600 lineas cambiadas.
- Limite maximo por PR: 1000 lineas cambiadas.
- Todo cambio debe seguir `red -> green -> refactor`.

## Simplificaciones intencionales para la version inicial
- Invitaciones simples: generar y consumir invitaciones sin depender de envio real de email.
- UI simple: tablas, formularios basicos y navegacion minima.
- Arbol de recursos simple: alta y edicion por `parentId`, sin editor visual complejo.
- Auditoria simple: registrar y listar eventos, sin analitica avanzada.
- Integracion externa simple: exponer endpoints suficientes para probar autenticacion y consulta de decision efectiva, sin cerrar aun una federacion completa.

## Bloqueos funcionales a cerrar antes o durante la Etapa 0
- Resuelto en tarea `0.1`: cuando no existe ninguna regla aplicable, el default es `denegado`.
- Resuelto en tarea `0.1`: cuando la herencia llega a la raiz sin valor explicito, ese resultado se ignora y solo se devuelve `denegado` si no quedan otras reglas resueltas.
- Resuelto en tarea `0.2`: las invitaciones crean una invitacion pendiente y la identidad nace al aceptar.
- Resuelto en tarea `0.2`: el `TenantAdmin` queda limitado al alcance administrativo y no actua como usuario final.

## Etapa 0 - Base tecnica y cierre de bloqueos
### Objetivo
Dejar listo el esqueleto tecnico y cerrar las decisiones minimas que bloquean la implementacion del motor de reglas.

### Alcance
- Crear la estructura real del monorepo.
- Inicializar `apps/web` con React + Vite.
- Inicializar `apps/api` con solucion .NET y capas DDD.
- Configurar PostgreSQL local y configuracion por ambiente.
- Configurar testing base: Vitest, xUnit, FluentAssertions, WebApplicationFactory y Testcontainers.
- Dejar scripts base para correr app, tests y lint.
- Cerrar por escrito los bloqueos funcionales minimos del motor de autorizacion.

### Entregables
- Monorepo funcional con proyectos creados.
- Health check basico en la API.
- Shell minimo del frontend.
- Pipeline local de tests funcionando.
- Decision documentada para defaults de reglas e invitaciones.

### Criterio de salida
- Se puede levantar `web`, `api` y PostgreSQL en local.
- Se pueden correr tests vacios o basicos en frontend y backend.
- No quedan decisiones bloqueantes para modelar reglas efectivas.

### Desglose en tareas revisables
- Cada tarea de esta etapa deberia vivir idealmente en un PR separado.
- Si una tarea supera el objetivo normal de tamano, debe dividirse sin mezclar cambios de docs, frontend, backend e infraestructura.

#### Tarea 0.1 - Cerrar defaults del motor de autorizacion
- Objetivo: resolver los dos bloqueos que afectan directamente la semantica del motor de reglas.
- Alcance:
  - definir que ocurre cuando no existe ninguna regla aplicable;
  - definir que ocurre cuando la herencia llega a la raiz sin un valor explicito;
  - dejar la decision escrita en los documentos de requisitos que correspondan.
- Entregables:
  - decision documentada sin ambiguedad;
  - criterios de aceptacion y preguntas abiertas actualizados.
- Criterio de revision:
  - una persona revisora puede responder ambos escenarios sin asumir comportamiento implicito.
- Decision cerrada:
  - el default sin reglas aplicables es `denegado`;
  - una herencia sin valor explicito hasta la raiz no cuenta como decision resuelta;
  - si, tras ignorar herencias no resueltas, no quedan otras reglas resueltas aplicables, el resultado final es `denegado`.

#### Tarea 0.2 - Cerrar invitaciones y alcance del tenant admin
- Objetivo: resolver las definiciones funcionales minimas del modelo de identidad antes de implementar casos de uso.
- Alcance:
  - definir si la invitacion crea identidad inmediatamente o requiere aceptacion posterior;
  - definir si un `TenantAdmin` tambien puede actuar como usuario final sujeto a reglas;
  - dejar la decision escrita en los documentos de requisitos que correspondan.
- Entregables:
  - flujo de invitaciones definido;
  - alcance del `TenantAdmin` documentado.
- Criterio de revision:
  - el modelo de actores e invitaciones queda implementable sin supuestos extra.
- Decision cerrada:
  - las invitaciones generan una invitacion pendiente y la identidad se crea al aceptar;
  - el `TenantAdmin` solo actua con alcance administrativo;
  - si una persona necesita alcance administrativo y tambien ser evaluada por reglas, debe usar identidades separadas.

#### Tarea 0.3 - Crear el esqueleto del monorepo
- Objetivo: reflejar en el repositorio la estructura base acordada en `TECH_SPEC.md`.
- Alcance:
  - crear `apps/`, `infra/` y las rutas base esperadas para `web` y `api`;
  - preparar los archivos raiz minimos para trabajar el monorepo.
- Entregables:
  - estructura real del repositorio creada;
  - punto de entrada claro para frontend, backend e infraestructura.
- Criterio de revision:
  - la estructura del repo coincide con la arquitectura definida y puede recorrerse sin huecos importantes.

#### Tarea 0.4 - Inicializar `apps/web`
- Objetivo: dejar arrancado el frontend administrativo con un shell minimo.
- Alcance:
  - crear `apps/web` con React + Vite;
  - dejar una pantalla inicial basica;
  - validar que la app levanta en local.
- Entregables:
  - proyecto frontend inicializado;
  - shell minimo funcional.
- Criterio de revision:
  - `web` arranca localmente y muestra una pagina base sin depender aun del dominio.

#### Tarea 0.5 - Configurar testing base del frontend
- Objetivo: dejar listo el arnes minimo de pruebas del frontend antes de agregar funcionalidad real.
- Alcance:
  - integrar `Vitest`;
  - dejar al menos un test basico del shell o de un componente minimo;
  - documentar como correr las pruebas del frontend.
- Entregables:
  - setup de testing de `web`;
  - prueba automatizada base pasando.
- Criterio de revision:
  - una persona revisora puede ejecutar los tests del frontend sin configuracion manual oculta.

#### Tarea 0.6 - Inicializar la solucion .NET del backend
- Objetivo: dejar creada la base tecnica de `apps/api` con sus capas DDD.
- Alcance:
  - crear la solucion y los proyectos `Api`, `Application`, `Domain` e `Infrastructure`;
  - dejar referencias entre proyectos coherentes con la arquitectura;
  - asegurar que la solucion compila.
- Entregables:
  - solucion .NET creada;
  - capas base del backend conectadas.
- Criterio de revision:
  - la solucion compila en limpio y la separacion de capas es visible en el repo.

#### Tarea 0.7 - Agregar `health check` y arnes base de testing backend
- Objetivo: validar que la API puede arrancar y que el backend tiene base para TDD e integracion.
- Alcance:
  - exponer un `health check` minimo en la API;
  - preparar `xUnit`, `FluentAssertions`, `WebApplicationFactory` y `Testcontainers`;
  - dejar al menos un test base de arranque o smoke test.
- Entregables:
  - endpoint de salud funcional;
  - proyectos de tests backend creados y ejecutables.
- Criterio de revision:
  - la API responde al `health check` y existe al menos una prueba backend pasando.

#### Tarea 0.8 - Preparar PostgreSQL local y configuracion por ambiente
- Objetivo: habilitar una base local reproducible y convenciones explicitas de configuracion.
- Alcance:
  - definir el mecanismo local para levantar PostgreSQL desde `infra/`;
  - separar configuracion por ambiente para backend y, si aplica, frontend;
  - documentar variables y valores minimos requeridos para desarrollo.
- Entregables:
  - PostgreSQL local levantable;
  - configuracion base por ambiente disponible.
- Criterio de revision:
  - otra persona puede levantar la base y conectar la API sin descubrir pasos implicitos.

#### Tarea 0.9 - Consolidar scripts base y flujo local
- Objetivo: dejar un flujo de trabajo minimo y repetible para ejecutar apps y pruebas.
- Alcance:
  - agregar scripts o comandos base para levantar `web`, `api` y PostgreSQL;
  - agregar comandos base para correr tests y lint;
  - documentar la secuencia minima de arranque local.
- Entregables:
  - comandos base de desarrollo definidos;
  - flujo local minimo documentado.
- Criterio de revision:
  - una persona revisora puede seguir pocos comandos y verificar que la etapa 0 quedo cerrada.

### Orden sugerido dentro de la Etapa 0
1. Tareas `0.1` y `0.2`.
2. Tarea `0.3`.
3. Tareas `0.4` y `0.6`.
4. Tareas `0.5` y `0.7`.
5. Tarea `0.8`.
6. Tarea `0.9`.

**Estado:** ✅ **Completada**

## Etapa 1 - Core de identidad, tenancy y administracion basica
### Objetivo
Construir el nucleo de identidades y limites del sistema, antes de entrar al motor de autorizacion.

### Resumen de Ejecución
Durante esta etapa se tomó la decisión técnica de **unificar el modelo de usuario**. En lugar de tener entidades separadas para `SystemUser` y `TenantUser`, se utiliza una única entidad `User` con un `UserType`. Esto simplifica la persistencia y la lógica de autenticación centralizada.

**Estado:** ✅ **Completada**

### Entregables
- Entidades, value objects y reglas de invariantes del core.
- Tests unitarios del dominio para tenancy, tipos de usuario e invitaciones.
- Casos de uso de aplicacion para bootstrap, registro e invitaciones.
- Endpoints minimos para bootstrap y registro de tenant admin.

### Criterio de salida
- Existe un modelo coherente para usuarios globales y usuarios de tenant.
- Se puede registrar un tenant admin y crear su tenant.
- Se puede invitar otro tenant admin y otro system user.
- Los privilegios administrativos implicitos quedan separados del motor de reglas.

### Desglose propuesto en tareas revisables
- La etapa debe priorizar dominio y tests antes de casos de uso y API.
- Cada tarea deberia producir un PR pequeno, idealmente por agregado o caso de uso.
- Cuando una tarea combine dominio + application, el alcance debe seguir siendo una sola capacidad verificable.

#### Tarea 1.1 - Modelar `Tenant`
- Objetivo: introducir el agregado base del aislamiento funcional del sistema.
- Alcance:
  - crear la entidad o agregado `Tenant`;
  - definir identidad, nombre, estado y reglas de activacion/desactivacion;
  - cubrir invariantes minimas con tests unitarios.
- Entregables:
  - modelo de dominio de `Tenant`;
  - tests unitarios del agregado.
- Branch sugerida: `feat/task-1.1-tenant-aggregate`
- PR title sugerido: `[task-1.1] feat: add tenant aggregate`
- Criterio de revision:
  - el agregado expresa claramente el limite de tenant y sus invariantes minimas.

#### Tarea 1.2 - Modelar `TenantUser` y tipos de usuario
- Objetivo: representar las identidades que viven dentro de un tenant.
- Alcance:
  - crear `TenantUser`;
  - modelar tipos `TenantAdmin`, `EndUser` y `ServiceAdmin`;
  - asegurar pertenencia exacta a un tenant;
  - cubrir invariantes con tests unitarios.
- Entregables:
  - modelo de dominio de `TenantUser` y su tipo;
  - tests unitarios de pertenencia y tipos.
- Branch sugerida: `feat/task-1.2-tenant-user-types`
- PR title sugerido: `[task-1.2] feat: add tenant user types`
- Criterio de revision:
  - el modelo distingue sin ambiguedad usuarios administrativos, finales y de servicio dentro del tenant.

#### Tarea 1.3 - Modelar `SystemUser`
- Objetivo: introducir la identidad administrativa global fuera del alcance tenant.
- Alcance:
  - crear `SystemUser`;
  - reflejar que no pertenece a ningun tenant;
  - cubrir invariantes y estado minimo con tests unitarios.
- Entregables:
  - modelo de dominio de `SystemUser`;
  - tests unitarios del actor global.
- Branch sugerida: `feat/task-1.3-system-user`
- PR title sugerido: `[task-1.3] feat: add system user model`
- Criterio de revision:
  - el actor global queda separado del dominio tenant sin atajos ambiguos.

#### Tarea 1.4 - Modelar `Role` y `Application`
- Objetivo: completar las entidades base administrativas dentro del tenant.
- Alcance:
  - crear `Role` y `Application`;
  - asegurar pertenencia estricta al tenant;
  - cubrir invariantes minimas con tests unitarios.
- Entregables:
  - modelos de dominio de `Role` y `Application`;
  - tests unitarios asociados.
- Branch sugerida: `feat/task-1.4-role-and-application`
- PR title sugerido: `[task-1.4] feat: add role and application models`
- Criterio de revision:
  - ambos conceptos quedan listos para ser usados por etapas posteriores sin mezclar autorizacion aun.

#### Tarea 1.5 - Modelar invitaciones
- Objetivo: capturar el flujo pendiente de incorporacion de identidades administrativas.
- Alcance:
  - crear el modelo de invitacion para `SystemUser` y `TenantAdmin`;
  - reflejar estado pendiente y aceptacion posterior;
  - impedir que la invitacion cree identidad activa inmediatamente;
  - cubrir reglas de negocio con tests unitarios.
- Entregables:
  - modelo de dominio de invitaciones;
  - tests unitarios de emision y aceptacion.
- Branch sugerida: `feat/task-1.5-invitations-model`
- PR title sugerido: `[task-1.5] feat: add invitation model`
- Criterio de revision:
  - una persona revisora puede ver claramente que invitar no equivale a crear identidad.

#### Tarea 1.6 - Bootstrap del primer `SystemUser`
- Objetivo: modelar la inicializacion unica del primer usuario global.
- Alcance:
  - crear la capacidad de bootstrap del primer `SystemUser`;
  - asegurar que solo exista uno en el arranque inicial;
  - cubrir el comportamiento con tests unitarios o de application liviana.
- Entregables:
  - servicio o caso de uso de bootstrap;
  - tests de comportamiento del bootstrap unico.
- Branch sugerida: `feat/task-1.6-system-user-bootstrap`
- PR title sugerido: `[task-1.6] feat: add system user bootstrap`
- Criterio de revision:
  - el flujo deja claro cuando el bootstrap es valido y cuando debe rechazarse.

#### Tarea 1.7 - Caso de uso de registro inicial de `TenantAdmin`
- Objetivo: habilitar el alta inicial de un tenant junto con su administrador.
- Alcance:
  - crear el caso de uso que registra un `TenantAdmin` y crea su `Tenant`;
  - respetar el modelo de identidad administrativa separada del usuario final;
  - cubrir escenarios principales con tests de application.
- Entregables:
  - caso de uso de registro inicial;
  - tests de application del flujo de registro.
- Branch sugerida: `feat/task-1.7-tenant-admin-registration`
- PR title sugerido: `[task-1.7] feat: add tenant admin registration`
- Criterio de revision:
  - el flujo crea tenant + admin de forma coherente y sin supuestos ocultos.

#### Tarea 1.8 - Caso de uso de invitacion de `SystemUser`
- Objetivo: habilitar la emision de invitaciones para administradores globales.
- Alcance:
  - crear el caso de uso para invitar otro `SystemUser`;
  - emitir solo invitacion pendiente;
  - cubrir validaciones principales con tests de application.
- Entregables:
  - caso de uso de invitacion de usuarios globales;
  - tests de application asociados.
- Branch sugerida: `feat/task-1.8-system-user-invite`
- PR title sugerido: `[task-1.8] feat: add system user invitations`
- Criterio de revision:
  - el caso de uso deja explicito que la identidad nace solo al aceptar.

#### Tarea 1.9 - Caso de uso de invitacion de `TenantAdmin`
- Objetivo: habilitar la emision de invitaciones administrativas dentro del tenant.
- Alcance:
  - crear el caso de uso para invitar otro `TenantAdmin`;
  - limitar el alcance al mismo tenant;
  - emitir solo invitacion pendiente;
  - cubrir reglas principales con tests de application.
- Entregables:
  - caso de uso de invitacion de tenant admins;
  - tests de application asociados.
- Branch sugerida: `feat/task-1.9-tenant-admin-invite`
- PR title sugerido: `[task-1.9] feat: add tenant admin invitations`
- Criterio de revision:
  - el flujo conserva correctamente el tenant de origen y no crea identidad activa inmediata.

#### Tarea 1.10 - Privilegios implicitos de `SystemUser` y `TenantAdmin`
- Objetivo: dejar explicita la separacion entre privilegios administrativos y futuro motor de reglas.
- Alcance:
  - modelar o encapsular los privilegios implicitos de `SystemUser`;
  - modelar o encapsular los privilegios implicitos de `TenantAdmin`;
  - dejar fuera del motor de autorizacion de recursos cualquier chequeo de este tipo;
  - cubrir comportamiento con tests.
- Entregables:
  - politica o servicio para privilegios implicitos;
  - tests que prueban la separacion con respecto al motor de reglas futuro.
- Branch sugerida: `feat/task-1.10-implicit-admin-privileges`
- PR title sugerido: `[task-1.10] feat: add implicit admin privileges`
- Criterio de revision:
  - el camino administrativo implicito queda aislado del modelo de reglas tenant.

#### Tarea 1.11 - Endpoints minimos de Etapa 1
- Objetivo: exponer una API basica para validar la etapa de identidad y tenancy.
- Alcance:
  - exponer endpoints minimos para bootstrap, registro inicial e invitaciones principales;
  - mantener el alcance reducido a los casos de uso ya modelados;
  - cubrir el comportamiento con integration tests de API.
- Entregables:
  - endpoints HTTP minimos de la etapa;
  - integration tests de arranque y flujos principales.
- Branch sugerida: `feat/task-1.11-identity-api-minimum`
- PR title sugerido: `[task-1.11] feat: add identity api endpoints`
- Criterio de revision:
  - una persona revisora puede ejecutar la API y comprobar bootstrap, registro e invitaciones basicas.

### Orden recomendado dentro de la Etapa 1
1. Tareas `1.1`, `1.2` y `1.3`.
2. Tareas `1.4` y `1.5`.
3. Tarea `1.6`.
4. Tareas `1.7`, `1.8` y `1.9`.
5. Tarea `1.10`.
6. Tarea `1.11`.

### Recomendaciones de corte para IA en la Etapa 1
- Priorizar un agregado o caso de uso por PR.
- Empezar cada tarea por tests de dominio o application segun corresponda.
- No mezclar persistencia fuerte de PostgreSQL ni EF Core en esta etapa salvo necesidad justificada.
- No introducir aun reglas de autorizacion de recursos; esa responsabilidad pertenece a la Etapa 2.
- Vigilar especialmente tres limites: `SystemUser` fuera de tenant, `TenantAdmin` fuera del modelo de usuario final y las invitaciones como identidades pendientes.

## Etapa 2 - Core de autorizacion y arbol de recursos
### Objetivo
Construir y validar el corazon del producto: recursos, operaciones, reglas y decision efectiva.

### Alcance
- Modelar `Resource`, `Operation` y jerarquia de recursos.
- Modelar `AuthorizationRule` con `usuarioTenant?`, `rol?`, `recurso`, `operacion`, `decision`.
- Modelar asignacion de roles a usuarios.
- Implementar matching por usuario, por rol y por usuario + rol.
- Implementar herencia por misma operacion hacia el padre.
- Implementar precedencia de `denegado`.
- Implementar servicio de dominio para resolver decision efectiva.
- Cubrir contradicciones, redundancias y herencia con TDD.

### Entregables
- Modelo de dominio completo del motor de autorizacion.
- Suite fuerte de tests unitarios para decision efectiva.
- API interna o endpoint tecnico para evaluar decisiones efectivas.

### Criterio de salida
- El motor resuelve correctamente reglas directas, por rol, mixtas y heredadas.
- `denegado` gana siempre sobre `permitido`.
- La decision efectiva puede probarse automaticamente sobre escenarios reales del dominio.

**Estado:** ✅ **Completada y probada** (ver `docs/ETAPA_2_COMPLETADA.md`)

## Etapa 3 - Persistencia y APIs administrativas del core
### Objetivo
Conectar el modelo a PostgreSQL y exponer APIs suficientes para administrar el core desde fuera.

### Alcance
- Diseñar el esquema inicial de base de datos.
- Implementar persistencia con EF Core + Npgsql.
- Crear migraciones iniciales.
- Implementar repositorios e integracion de infraestructura.
- Exponer APIs CRUD minimas para:
  - tenants;
  - usuarios de tenant;
  - roles;
  - aplicaciones;
  - recursos;
  - operaciones;
  - reglas;
  - invitaciones.
- Exponer endpoint de evaluacion efectiva para pruebas de integracion.
- Registrar auditoria basica sobre cambios administrativos.

### Entregables
- Base de datos versionada con migraciones.
- Endpoints del core funcionales.
- Tests de integracion contra PostgreSQL y API.
- Auditoria minima persistida.

### Criterio de salida
- Los modelos core pueden crearse, editarse y consultarse via API.
- La evaluacion efectiva funciona con datos persistidos.
- La auditoria registra cambios administrativos principales.

### Desglose en tareas revisables (recomendado)

#### Tarea 3.1 - Diseñar esquema de base de datos inicial
- Objetivo: Definir tablas, relaciones y constraints para el core completo.
- Alcance:
  - Diseñar tablas: `tenants`, `users`, `roles`, `applications`, `resources`, `operations`, `authorization_rules`, `user_role_assignments`, `invitations`
  - Definir claves foráneas, índices y constraints
  - Documentar el esquema (diagrama o archivo de migración base)
- Entregables:
  - Archivo con esquema SQL o documento de diseño
  - Justificación de decisiones (ej: por qué Resource tiene ParentId, cómo se modela herencia)
- Criterio de revisión:
  - Todas las entidades del dominio tienen una tabla correspondiente
  - Relaciones 1:N y N:N están claras
  - Integridad referencial asegurada
- Branch sugerida: `docs/task-3.1-database-schema-design`
- PR title sugerido: `[task-3.1] docs: design initial database schema`

#### Tarea 3.2 - Crear DbContext y configuraciones EF Core iniciales
- Objetivo: Establecer el contexto de base de datos con DbSets y configuraciones básicas.
- Alcance:
  - Crear `IamPlatformDbContext` que herede de `DbContext`
  - Definir DbSets para todas las entidades
  - Configurar property mappings (columnas, tipos, constraints)
  - Configurar relaciones entre entidades (Fluent API)
- Entregables:
  - `IamPlatformDbContext.cs`
  - Configuraciones de entidades (en carpetas `Configurations/` o en `OnModelCreating`)
  - Asegurar que el contexto compile sin errores
- Criterio de revisión:
  - Todas las entidades están mapeadas
  - Relaciones configuradas correctamente
  - No hay configuraciones contradictorias
- Branch sugerida: `feat/task-3.2-dbcontext-initial`
- PR title sugerido: `[task-3.2] feat: add initial DbContext and entity configurations`
- **Precondición**: Tarea 3.1 completada

#### Tarea 3.3 - Crear primera migración de EF Core
- Objetivo: Generar migración inicial que refleje el esquema completo.
- Alcance:
  - Asegurar `DbContext` funcional
  - Ejecutar `dotnet ef migrations add InitialCreate`
  - Revisar y ajustar migración si es necesario
  - Agregar migración al repositorio
- Entregables:
  - Migración generada en `Migrations/` folder
  - Archivo `IamPlatformDbContextModelSnapshot` actualizado
- Criterio de revisión:
  - Migración refleja todas las tablas y constraints
  - Migración puede aplicarse a una base vacía
- Branch sugerida: `feat/task-3.3-initial-migration`
- PR title sugerido: `[task-3.3] feat: add initial EF Core migration`
- **Precondición**: Tarea 3.2 completada

#### Tarea 3.4 - Implementar `ResourceRepository` con EF Core
- Objetivo: Convertir el repositorio in-memory a persistencia real.
- Alcance:
  - Crear `ResourceRepository` que implemente `IResourceRepository`
  - Usar `IamPlatformDbContext` para operaciones CRUD
  - Implementar método `GetByIdAsync` y cualquier otro específico
  - Asegurar queries eficientes (incluir navegaciones si es necesario)
- Entregables:
  - `ResourceRepository.cs` en Infrastructure
  - Tests de integración que usen PostgreSQL real (con Testcontainers)
- Criterio de revisión:
  - CRUD básico funciona
  - Relaciones se cargan correctamente
  - Tests de integración pasan
- Branch sugerida: `feat/task-3.4-resource-repository`
- PR title sugerido: `[task-3.4] feat: implement ResourceRepository with EF Core`
- **Precondición**: Tarea 3.3 completada

#### Tarea 3.5 - Implementar `AuthorizationRuleRepository` con EF Core
- Objetivo: Repositorio para reglas de autorización.
- Alcance:
  - Crear `AuthorizationRuleRepository` que implemente `IAuthorizationRuleRepository`
  - Implementar método clave `GetApplicableRulesAsync(userId, resourceId, operationId)` con query eficiente
  - Considerar filtrado por `IsActive` y `TenantId`
- Entregables:
  - `AuthorizationRuleRepository.cs`
  - Tests de integración específicos para este repositorio
- Criterio de revisión:
  - Query devuelve solo reglas aplicables (activas, mismo tenant, recurso/operación)
  - Performance aceptable
- Branch sugerida: `feat/task-3.5-authorizationrule-repository`
- PR title sugerido: `[task-3.5] feat: implement AuthorizationRuleRepository`
- **Precondición**: Tarea 3.4 completada

#### Tarea 3.6 - Implementar `UserRoleAssignmentRepository` con EF Core
- Objetivo: Repositorio para asignaciones usuario-rol.
- Alcance:
  - Crear `UserRoleAssignmentRepository` que implemente `IUserRoleAssignmentRepository`
  - Implementar métodos: `GetUserRolesAsync(userId)`, `AssignAsync`, `RemoveAsync`
- Entregables:
  - `UserRoleAssignmentRepository.cs`
  - Tests de integración
- Criterio de revisión:
  - Puede obtener todos los roles de un usuario
  - Asignación y remoción funcionan
- Branch sugerida: `feat/task-3.6-userroleassignment-repository`
- PR title sugerido: `[task-3.6] feat: implement UserRoleAssignmentRepository`
- **Precondición**: Tarea 3.5 completada

#### Tarea 3.7 - Implementar repositorios restantes
- Objetivo: Asegurar que todos los repositorios del dominio tengan implementación EF Core.
- Alcance:
  - `IUserRepository` → `UserRepository` (unificado para todos los tipos de usuario)
  - `IRoleRepository`
  - `IApplicationRepository`
  - `ITenantRepository`
- Entregables:
  - Repositorios actualizados.
  - Tests de integración.

**Estado:** 🚧 **En progreso** (Ya existen implementaciones `InMemory` unificadas preparadas para el cambio a EF Core).
- Branch sugerida: `feat/task-3.7-remaining-repositories`
- PR title sugerido: `[task-3.7] feat: complete remaining EF Core repositories`
- **Precondición**: Tarea 3.6 completada

#### Tarea 3.8 - Actualizar Infrastructure DI para PostgreSQL
- Objetivo: Configurar la inyección de dependencias para usar PostgreSQL en producción.
- Alcance:
  - Modificar `Infrastructure/DependencyInjection.cs` para registrar `IamPlatformDbContext` con Npgsql
  - Configurar connection string desde `IConfiguration`
  - registrar repositorios EF Core (Scoped)
  - Desregistrar o condicionar repositorios in-memory (solo para tests)
- Entregables:
  - Método `AddInfrastructure` actualizado
  - Archivo de configuración de connection strings (ej: `appsettings.json` o variables de entorno)
- Criterio de revisión:
  - `DbContext` se resuelve correctamente
  - Repositorios EF Core están registrados
  - Tests unitarios pueden mockear repositorios (las interfaces no cambian)
- Branch sugerida: `feat/task-3.8-infrastructure-di`
- PR title sugerido: `[task-3.8] feat: update Infrastructure DI for PostgreSQL`
- **Precondición**: Tarea 3.7 completada

#### Tarea 3.9 - Endpoints CRUD para Recursos
- Objetivo: Exponer API HTTP para gestionar recursos (Resource).
- Alcance:
  - `POST /resources` - crear recurso (root o child con parentId)
  - `GET /resources/{id}` - obtener recurso
  - `PUT /resources/{id}` - actualizar (rename, changeKey, activate/deactivate)
  - `GET /resources` - listar (filtrado por applicationId, padre/hijos?)
  - `DELETE /resources/{id}` (o soft-delete via `Deactivate`)
- Entregables:
  - Endpoints en `Program.cs`
  - DTOs de request/response si son necesarios
  - Tests de integración API
- Criterio de revisión:
  - CRUD completo funciona
  - Validaciones aplican (ej: no crear ciclos)
  - Respuestas HTTP apropiadas (201, 200, 404, etc.)
- Branch sugerida: `feat/task-3.9-resources-crud-endpoints`
- PR title sugerido: `[task-3.9] feat: add CRUD endpoints for resources`
- **Precondición**: Tarea 3.8 completada

#### Tarea 3.10 - Endpoints CRUD para Operaciones
- Objetivo: API para gestionar operaciones (Operation).
- Alcance:
  - `POST /operations` - crear operación
  - `GET /operations/{id}`
  - `PUT /operations/{id}` - rename, changeKey, activate/deactivate
  - `GET /operations` - listar por applicationId
- Entregables:
  - Endpoints en `Program.cs`
  - Tests de integración
- Criterio de revisión:
  - Operaciones pueden crearse y gestionarse
  - Relación con Application validada
- Branch sugerida: `feat/task-3.10-operations-crud-endpoints`
- PR title sugerido: `[task-3.10] feat: add CRUD endpoints for operations`
- **Precondición**: Tarea 3.9 completada

#### Tarea 3.11 - Endpoints CRUD para Reglas de Autorización
- Objetivo: API para gestionar AuthorizationRule.
- Alcance:
  - `POST /authorization/rules` - crear regla (for user, for role, for both)
  - `GET /authorization/rules/{id}`
  - `PUT /authorization/rules/{id}` - changeDecision, activate/deactivate
  - `GET /authorization/rules` - listar (filtrado por resourceId, userId, roleId, tenantId)
  - `DELETE /authorization/rules/{id}` (o deactivate)
- Entregables:
  - Endpoints en `Program.cs`
  - DTOs que cubran los tres casos de creación
  - Tests de integración
- Criterio de revisión:
  - Reglas pueden crearse con cualquiera de los tres objetivos
  - Validaciones funcionan (misma app recurso/operación, mismo tenant usuario/rol)
- Branch sugerida: `feat/task-3.11-authorization-rules-crud`
- PR title sugerido: `[task-3.11] feat: add CRUD endpoints for authorization rules`
- **Precondición**: Tarea 3.10 completada

#### Tarea 3.12 - Endpoints CRUD para Asignación de Roles (UserRoleAssignment)
- Objetivo: API para asignar/remover roles a usuarios.
- Alcance:
  - `POST /users/{userId}/roles` - asignar rol
  - `DELETE /users/{userId}/roles/{roleId}` - remover rol
  - `GET /users/{userId}/roles` - listar roles del usuario
  - Validar que usuario y rol pertenecen al mismo tenant
- Entregables:
  - Endpoints en `Program.cs`
  - Tests de integración
- Criterio de revisión:
  - Asignación y remoción funcionan
  - Error sitenant no coincide
- Branch sugerida: `feat/task-3.12-userroleassignment-crud`
- PR title sugerido: `[task-3.12] feat: add endpoints for user role assignments`
- **Precondición**: Tarea 3.11 completada

#### Tarea 3.13 - Endpoint de Evaluación de Autorización (público)
- Objetivo: Exponer endpoint para que aplicaciones consulten decisión efectiva.
- Alcance:
  - `POST /authorization/evaluate` o `GET /authorization/evaluate?userId=...&resourceId=...&operationId=...`
  - Recibir: `userId`, `resourceId`, `operationId`
  - Usar `IAuthorizationService` para evaluar
  - Devolver: `{ "decision": "Allow"|"Deny", "appliedRules": [...] }`
  - Considerar autenticación (aún simple, quizás solo userId o token dummy)
- Entregables:
  - Endpoint en `Program.cs`
  - Tests de integración
- Criterio de revisión:
  - Evaluación funciona con datos persistidos
  - Resultado coincide con motor de dominio
- Branch sugerida: `feat/task-3.13-authorization-evaluate-endpoint`
- PR title sugerido: `[task-3.13] feat: add authorization evaluation endpoint`
- **Precondición**: Tarea 3.12 completada

#### Tarea 3.14 - Tests de Integración generales del Core persistido
- Objetivo: Verificar que todos los endpoints CRUD + evaluación funcionan contra PostgreSQL real.
- Alcance:
  - Crear Testcontainers con PostgreSQL
  - Escribir tests que:
    - Creen Tenant, User, Role, Application
    - Creen Resource hierarchy (padre → hijo)
    - Creen Operations
    - Creen AuthorizationRules (user, role, both)
    - Asignen roles a usuarios
    - Evalúen decisiones efectivas
    - Verifiquen herencia, deny precedence, default deny
- Entregables:
  - Archivos de integración en `IamPlatform.IntegrationTests`
  - CI/CD configurado para correr estos tests (si aplica)
- Criterio de revisión:
  - 100% de los escenarios de Etapa 2 también funcionan con datos persistidos
  - Base de datos se crea y migra automáticamente en tests
- Branch sugerida: `feat/task-3.14-core-integration-tests`
- PR title sugerido: `[task-3.14] feat: add integration tests for persisted core`
- **Precondición**: Tarea 3.13 completada

#### Tarea 3.15 - Implementar auditoría básica de cambios administrativos
- Objetivo: Registrar cambios importantes en tablas de auditoría.
- Alcance:
  - Diseñar tabla `audit_logs` (actor, tipo_actor, tenant_id, accion, objetivo, resultado, timestamp, detalles_jsonb)
  - Implementar EF Core entity `AuditLog`
  - Usar EF Core interceptors o middleware para capturar:
    - Creación/edición/eliminación de entidades principales
    - Evaluaciones de autorización (opcional por ahora)
  - Exponer endpoint para consultar logs (ReadOnly)
- Entregables:
  - Entidad AuditLog + migración
  - Interceptor o middleware de auditoría
  - Endpoint `GET /audit-logs`
- Criterio de revisión:
  - Cambios en entidades core quedan registrados
  - Logs incluyen quién hizo qué y cuándo
- Branch sugerida: `feat/task-3.15-audit-logging`
- PR title sugerido: `[task-3.15] feat: implement basic audit logging`
- **Precondición**: Tarea 3.14 completada

---

## Etapa 4 - Frontend administrativo minimo
### Objetivo
Proveer una UI suficiente para operar el sistema y verificar manualmente que el core funciona.

### Alcance
- Implementar login simple del panel administrativo.
- Implementar registro de tenant admin.
- Implementar vistas basicas para:
  - usuarios de tenant;
  - roles;
  - aplicaciones;
  - recursos;
  - operaciones;
  - reglas;
  - invitaciones;
  - logs.
- Implementar asignacion de roles a usuarios.
- Implementar carga de arbol de recursos con `parentId`.
- Implementar una pantalla simple para consultar decision efectiva.

### Entregables
- SPA funcional conectada a la API.
- Flujos minimos de alta/edicion del core.
- Tests de componentes y flujos criticos.

### Criterio de salida
- Un tenant admin puede operar el core desde la web sin usar herramientas externas.
- Se puede crear una aplicacion, su arbol de recursos, operaciones, reglas y consultar decisiones efectivas desde la UI.

## Etapa 5 - Integracion minima y cierre de MVP
### Objetivo
Demostrar el uso real del sistema por aplicaciones consumidoras y estabilizar la primera version.

### Alcance
- Exponer endpoint simple para autenticacion central inicial.
- Exponer endpoint para consulta de decision efectiva desde aplicaciones externas.
- Exponer acceso administrativo por API para `service admin` dentro del tenant.
- Completar auditoria de login, consultas de autorizacion e invitaciones.
- Agregar pruebas de integracion cruzadas entre dominio, API y persistencia.
- Documentar el flujo minimo de uso del sistema.

### Entregables
- API consumible por una aplicacion externa simple.
- Flujo basico de usuario de servicio administrativo.
- Documentacion de arranque y prueba del MVP.
- Smoke tests de backend y frontend.

### Criterio de salida
- Una aplicacion externa puede autenticar y consultar una decision efectiva.
- Un service admin puede automatizar administracion basica por API.
- El sistema demuestra de punta a punta el valor del core del IAM.

## Orden recomendado de prioridad dentro de cada etapa
1. Dominio y tests.
2. Casos de uso de aplicacion.
3. Persistencia e integracion.
4. Endpoints HTTP.
5. UI minima.

## Criterios de exito de la version inicial
- El core del modelo esta implementado y probado con TDD.
- El motor de reglas funciona con herencia y precedencia de `denegado`.
- El sistema soporta multiples tenants y sus actores principales.
- Existe una UI simple pero funcional para operar el core.
- Existe una API suficiente para validar el uso real por otras aplicaciones.

## Recomendacion de corte de tareas para IA
- Dividir por agregado o modulo de dominio cuando sea posible.
- Separar PRs de dominio, persistencia, API y UI si el tamano crece demasiado.
- Mantener cada PR enfocado en una sola idea verificable.
- Si un sprint resulta demasiado grande, dividirlo en mini-sprints internos sin romper el orden de prioridades.
