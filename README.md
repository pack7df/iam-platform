# iam-platform

Repositorio del MVP de IAM Platform con monorepo para `apps/web`, `apps/api` e infraestructura local.

## Configuracion local minima

- PostgreSQL local: `infra/docker-compose.yml`
- Variables de infraestructura: `infra/.env.example`
- Cadena de conexion base API: `apps/api/src/IamPlatform.Api/appsettings.Development.json`
- URL base sugerida para frontend: `apps/web/.env.example`
