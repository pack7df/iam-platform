# IAM Platform - Especificacion Tecnica Inicial

## Objetivo del documento
- Capturar decisiones tecnicas iniciales que acompanen `SPEC.md`.
- Mantener una arquitectura pequena, coherente y refinable.
- Separar requisitos funcionales de decisiones de implementacion.

## Estado actual
- Version tecnica: v0.8
- Etapa: decisiones iniciales cerradas

## Principios arquitectonicos
- Monorepo como estructura principal del proyecto.
- Dos aplicaciones principales: `web` y `api`.
- Un solo despliegue logico para frontend y backend.
- El backend expone solo APIs.
- El backend es la unica fuente de verdad del dominio.
- La arquitectura del backend sigue DDD.
- El diseno del codigo debe respetar principios SOLID.
- TDD debe ser la metodologia por defecto para implementar cambios.
- Cada tarea de implementacion debe asociarse a un pull request propio.
- Cada pull request debe mantenerse por debajo de 1000 lineas cambiadas para revision humana de calidad.
- El objetivo normal de tamano por pull request debe ser sensiblemente menor al limite maximo.
- Se prioriza simplicidad antes que distribucion temprana.

## Arquitectura general
- Aplicacion 1: frontend web administrativo.
- Aplicacion 2: backend central de IAM.
- El frontend consume al backend por API HTTP sobre JSON.
- El backend concentra autenticacion, autorizacion, evaluacion de reglas, invitaciones, auditoria y gestion multi-tenant.
- Las aplicaciones consumidoras se integran solo con el backend.

## Aplicacion frontend
- Tipo: SPA administrativa.
- Stack elegido: React + Vite.
- Recomendacion: TypeScript para mejorar mantenibilidad, contratos y asistencia por IA.
- Stack de testing recomendado: Vitest + React Testing Library.
- Objetivo principal: ofrecer la UI administrativa del sistema.
- Responsabilidades iniciales:
  - login;
  - registro de tenant admin;
  - gestion de usuarios, roles, aplicaciones y tenants;
  - gestion de recursos, operaciones y reglas;
  - visualizacion de logs e invitaciones.
- Restricciones:
  - no debe contener logica fuente de autorizacion;
  - no debe acceder directamente a la base de datos;
  - no debe contener reglas de negocio criticas duplicadas.

## Aplicacion backend
- Tipo: API HTTP.
- Stack elegido: ASP.NET Core Web API con C#.
- Stack de testing recomendado: xUnit + FluentAssertions.
- El backend debe exponer solo APIs, sin renderizado de UI.
- Objetivo principal: implementar toda la logica de negocio del IAM.
- Responsabilidades iniciales:
  - autenticacion central;
  - gestion de usuarios de sistema y usuarios de tenant;
  - gestion de tenants, roles, aplicaciones, recursos, operaciones y reglas;
  - evaluacion de decisiones efectivas de acceso;
  - invitaciones;
  - auditoria;
  - endpoints para integracion con aplicaciones consumidoras.

## Base de datos
- Base de datos elegida: PostgreSQL.
- Motivos principales:
  - el dominio es fuertemente relacional;
  - requiere consistencia transaccional para usuarios, roles, reglas e invitaciones;
  - soporta bien jerarquias y consultas recursivas para arboles de recursos;
  - tiene muy buena integracion con .NET mediante `Npgsql` y `EF Core`;
  - permite usar `JSONB` si luego se necesita flexibilidad adicional en auditoria o metadata.
- Regla de acceso: solo el backend accede a PostgreSQL.
- Persistencia preferida: `EF Core` con proveedor `Npgsql`.

## Backend DDD
- La solucion backend debe separarse al menos en estas capas:
  - `Api`;
  - `Application`;
  - `Domain`;
  - `Infrastructure`.
- Responsabilidad de capas:
  - `Domain`: entidades, value objects, agregados, invariantes, servicios de dominio y contratos de repositorio;
  - `Application`: casos de uso, comandos, queries, DTOs y coordinacion de reglas de negocio;
  - `Infrastructure`: persistencia, integraciones externas, implementaciones concretas de repositorios y servicios tecnicos;
  - `Api`: endpoints HTTP, autenticacion, autorizacion de entrada, serializacion y composition root.
- Modulos de dominio iniciales sugeridos:
  - `PlatformAdministration`;
  - `TenantAdministration`;
  - `Authorization`;
  - `Identity`;
  - `Audit`.

## Principios SOLID aplicados
- `S`: cada servicio y clase debe tener una sola responsabilidad clara.
- `O`: nuevas reglas y comportamientos deben poder agregarse sin reescribir el nucleo innecesariamente.
- `L`: las abstracciones deben poder reemplazarse sin romper comportamiento esperado.
- `I`: las interfaces deben ser pequenas y enfocadas por caso de uso.
- `D`: la logica de dominio y aplicacion no debe depender de detalles de infraestructura.

## Reglas de implementación
- Evitar métodos con código muy largo (más de ~30 líneas). Dividir en métodos más pequeños y con nombres descriptivos.
- Minimizar la anidación de código. Preferir retornos anticipados (early returns) y guard clauses en lugar de `else` anidados.
- Mantener la lógica de dominio en las entidades y servicios de dominio, no en la capa de aplicación.
- Usar factories estáticas en entidades cuando la creación sea simple; solo usar factories inyectables cuando haya lógica compleja o múltiples variantes.

## Estrategia de testing
- Se adopta TDD como flujo por defecto: `red -> green -> refactor`.
- La logica de dominio debe empezar por tests unitarios antes de implementar la solucion.
- Los casos de uso de `Application` deben cubrirse con tests unitarios o de integracion liviana segun el caso.
- La infraestructura critica debe validarse con tests de integracion.
- La API debe validarse con tests de integracion sobre endpoints principales.
- El frontend debe cubrir componentes y flujos criticos con tests automatizados.
- La integracion con PostgreSQL debe probarse preferentemente con entornos efimeros de prueba.
- Los cambios no deben considerarse terminados si no existe cobertura automatizada correspondiente.

## Herramientas de testing sugeridas
- Frontend: `Vitest`, `React Testing Library`, `@testing-library/user-event`.
- Backend unit tests: `xUnit`, `FluentAssertions`.
- Backend integration tests: `xUnit` + `WebApplicationFactory`.
- Integracion con base de datos: `Testcontainers` con PostgreSQL.

## Flujo de trabajo
- Cada tarea de desarrollo debe producir un pull request identificable y acotado.
- Un pull request debe representar una unidad de cambio revisable de principio a fin.
- El tamano objetivo normal de cada pull request debe ser de hasta 600 lineas cambiadas.
- El limite maximo de cada pull request debe ser de 1000 lineas cambiadas.
- Si una tarea supera el objetivo normal, debe justificarse que sigue siendo facilmente revisable.
- Si una tarea supera el limite maximo, debe dividirse en subtareas y multiples pull requests.
- Se debe priorizar una secuencia de cambios pequenos, coherentes y revisables.
- TDD debe aplicarse dentro de ese flujo, manteniendo tests y codigo dentro del mismo pull request.
- Excepciones puntuales al objetivo normal pueden incluir migraciones, lockfiles, codigo generado o renombres masivos, pero no deben usarse para mezclar multiples cambios conceptuales.
- Al trabajar con IA, una tarea grande debe dividirse preferentemente por vertical funcional, por capa tecnica o por tipo de cambio.

## Politica de ramas y PRs
- No se debe trabajar directamente sobre `main`.
- `main` queda reservado para integracion estable y releases.
- `dev` es la rama base de integracion para el trabajo diario.
- Cada tarea debe vivir en una rama corta dedicada y producir exactamente un PR principal.
- Cada rama de tarea debe nacer desde `dev` y volver a `dev` mediante pull request.
- La plantilla operativa para IA debe tomarse de `AI_TASK_PR_TEMPLATE.md`.
- La plantilla de cuerpo de PR para GitHub debe tomarse de `.github/pull_request_template.md`.
- Formato de rama recomendado: `<tipo>/<task-id>-<short-slug>`.
- Tipos permitidos de rama: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`.
- Si una tarea aun no tiene identificador externo, se puede usar un identificador interno corto como `task-001`.
- Ejemplos de ramas: `feat/task-014-tenant-admin-invite`, `fix/task-022-rule-inheritance`.
- El titulo del PR debe seguir el formato: `[<task-id>] <tipo>: <resumen corto>`.
- Ejemplos de PR: `[task-014] feat: add tenant admin invitations`, `[task-022] fix: correct rule inheritance evaluation`.
- El PR debe describir claramente alcance, razon del cambio, estrategia de testing y limites conocidos.
- Se recomienda usar `draft PR` cuando la tarea todavia no este lista para revision final.
- Una vez aprobado, el merge preferido debe ser `squash merge` para mantener una historia limpia.
- La rama debe eliminarse despues del merge.

## Politica de commits
- Los commits deben ser pequenos, frecuentes y coherentes con TDD.
- El mensaje debe seguir una convencion tipo Conventional Commits.
- Formato recomendado: `<tipo>(<scope>): <resumen>`.
- Tipos recomendados: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`.
- Cuando exista task id, se recomienda incluirlo al final del mensaje o en el cuerpo del commit.
- Ejemplos: `test(auth): add tenant admin registration tests`, `feat(rules): implement deny precedence`.
- Dentro de una misma rama, los commits deben contar una secuencia entendible: tests, implementacion, refactor.

## Estructura inicial del monorepo
```text
/
  SPEC.md
  TECH_SPEC.md
  apps/
    web/
      src/
      tests/
    api/
      src/
        IamPlatform.Api/
        IamPlatform.Application/
        IamPlatform.Domain/
        IamPlatform.Infrastructure/
      tests/
        IamPlatform.UnitTests/
        IamPlatform.IntegrationTests/
  infra/
```

## Comunicacion entre componentes
- `web -> api`: API HTTP sobre JSON.
- `consumer apps -> api`: API HTTP para autenticacion y consulta de decisiones efectivas.
- No debe existir acceso directo del frontend ni de aplicaciones consumidoras a PostgreSQL.

## Estrategia de despliegue
- Frontend y backend se versionan y despliegan juntos como una sola unidad logica.
- El backend sigue siendo API-only.
- El frontend se construye como archivos estaticos.
- El despliegue recomendado es una sola stack compuesta por:
  - un servidor estatico para `web`;
  - la API ASP.NET Core;
  - PostgreSQL como persistencia.
- Esto mantiene un solo despliegue del sistema sin mezclar la responsabilidad del backend con servir UI.

## Decisiones tecnicas cerradas
- Frontend: React + Vite.
- Backend: ASP.NET Core Web API + C#.
- Persistencia: PostgreSQL.
- Acceso a datos: EF Core + Npgsql.
- Organizacion del repositorio: monorepo.
- Estilo arquitectonico del backend: DDD.
- Principios de diseno: SOLID.
- Estrategia de desarrollo: TDD.
- Flujo de entrega: una tarea por pull request.
- Politica de tamano de PR: objetivo de hasta 600 lineas y maximo de 1000 lineas cambiadas.
- Politica de ramas: `<tipo>/<task-id>-<short-slug>` desde `dev`.
- Politica de integracion: todo PR debe apuntar a `dev`; `main` solo recibe cambios promovidos desde `dev`.
- Politica de PR: titulo `[<task-id>] <tipo>: <resumen corto>` y merge preferido `squash`.
- Politica de commits: Conventional Commits.
- Plantilla operativa para IA: `AI_TASK_PR_TEMPLATE.md`.
- Plantilla de PR de GitHub: `.github/pull_request_template.md`.
- Testing frontend: Vitest + React Testing Library.
- Testing backend: xUnit + FluentAssertions.
- Testing de integracion: WebApplicationFactory + Testcontainers.
- Estrategia de release: un solo despliegue logico para `web` y `api`.

## No objetivos tecnicos por ahora
- Microservicios.
- Server-side rendering en el frontend.
- Aplicacion mobile nativa.
- Arquitectura distribuida compleja.
- Multiples backends especializados desde el inicio.
- Event sourcing o CQRS complejos desde la primera version.

## Preguntas tecnicas abiertas
- Se incorporaran tests end-to-end desde la primera version o en una etapa posterior?
- El panel administrativo web usara autenticacion basada en cookies, tokens o una combinacion?
- Como se expondra el SSO para aplicaciones externas: OIDC, OAuth2 u otro protocolo?
- El servidor estatico del frontend sera `nginx`, `caddy` u otra opcion?
- La API seguira un estilo REST clasico con controllers o una variante mas vertical por feature?
  - Si es vertical por feature, como se mapearan los endpoints a los servicios de aplicacion (ej: en Program.cs, clases estáticas, controladores ligeros)?
  - Se debe establecer una convención clara de que los endpoints solo delegan a servicios de aplicación y no contienen lógica de negocio.
- Como se resolvera la gestion de secretos y configuracion por ambiente?
