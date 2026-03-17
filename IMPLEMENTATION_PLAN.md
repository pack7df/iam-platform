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
- Definir el default cuando no existe ninguna regla aplicable.
- Definir el default cuando la herencia llega a la raiz sin valor explicito.
- Definir si las invitaciones crean identidad inmediatamente o requieren aceptacion posterior.
- Definir si un administrador de tenant tambien puede actuar como usuario final sujeto a reglas.

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

## Etapa 1 - Core de identidad, tenancy y administracion basica
### Objetivo
Construir el nucleo de identidades y limites del sistema, antes de entrar al motor de autorizacion.

### Alcance
- Modelar `SystemUser` y bootstrap del primer usuario de sistema.
- Modelar `Tenant`, `TenantUser` y tipos de usuario de tenant.
- Modelar `Role` y `Application` dentro del tenant.
- Modelar invitaciones de usuario de sistema y tenant admin.
- Implementar casos de uso de registro de tenant admin creando tenant.
- Implementar casos de uso de invitacion de system users y tenant admins.
- Implementar privilegios implicitos de `SystemUser` y `TenantAdmin` fuera del motor de reglas.

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
