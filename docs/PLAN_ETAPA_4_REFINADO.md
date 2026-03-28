# Plan Refinado: Etapa 4 - Ciclo de Vida de Identidad y Panel Administrativo (Cognito-like)

Este documento detalla el refinamiento de la Etapa 4 para construir un sistema de IAM con flujo de onboarding completo y una interfaz visual premium, similar a la experiencia de AWS Cognito.

## Objetivos de la Etapa
1.  **Registro de Autoservicio**: Flujo anónimo para `TenantAdmin` (Registro -> Email -> Verificación -> Password).
2.  **Autenticación**: Login con JWT.
3.  **Administración**: Panel Web para gestionar Usuarios, Roles, Aplicaciones, Recursos y Reglas.
4.  **Seguridad Proactiva**: Prevención de IDOR y contexto de identidad implícita.

## Desglose de Tareas (Límite < 600 líneas)

### Bloque 1: Cimientos de Identidad
- **4.1 - Dominio de Credenciales y Verificación**: Ampliación de `User` (Status, PasswordHash) y entidad `VerificationCode`.
- **4.2 - Infraestructura de Notificaciones**: Interfaz `IEmailSender` y `FakeEmailSender`.

### Bloque 2: Onboarding (Web + API)
- **4.3 - Registro Anónimo (Vertical Slice)**: Pantalla de Registro e Inicio de proceso (Tenant/User en estado `Pending`).
- **4.4 - Verificación y Activación**: Pantallas de Código y Password + Comandos de activación final.

### Bloque 3: Sesión y Contexto
- **4.5 - Autenticación y JWT**: Pantalla de Login y emisión de Tokens.
- **4.6 - Contexto de Identidad Implícita**: Middleware para proteger endpoints y filtrar datos por `TenantId`.

### Bloque 4: Administración del Tenant
- **4.7 - Gestión de Usuarios y Roles**: CRUD completo (UI + API).
- **4.8 - Gestión de Aplicaciones y Recursos**: CRUD y explorador de árbol (UI + API).

## Criterios de Calidad
- **Estética**: Diseño moderno (Glassmorphism, gradientes, tipografía cuidada).
- **Seguridad**: Contraseñas hasheadas y validación estricta de códigos.
- **DDD**: Lógica de negocio encapsulada en el dominio.
