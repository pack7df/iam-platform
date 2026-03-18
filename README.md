# iam-platform

Repositorio del MVP de IAM Platform con monorepo para `apps/web`, `apps/api` e infraestructura local.

## Configuracion local minima

- PostgreSQL local: `infra/docker-compose.yml`
- Variables de infraestructura: `infra/.env.example`
- Cadena de conexion base API: `apps/api/src/IamPlatform.Api/appsettings.Development.json`
- URL base sugerida para frontend: `apps/web/.env.example`

## Scripts base

- `npm run infra:up`: levanta PostgreSQL local en `5433`.
- `npm run infra:down`: apaga PostgreSQL local.
- `npm run dev:api`: levanta la API en `http://localhost:5125`.
- `npm run dev:web`: levanta la SPA administrativa.
- `npm run build:api`: compila la solucion .NET.
- `npm run build:web`: compila el frontend.
- `npm run test:api`: ejecuta unit + integration tests del backend.
- `npm run test:web`: ejecuta Vitest en `apps/web`.
- `npm run lint:web`: ejecuta ESLint del frontend.

## Flujo local minimo

1. Ejecuta `npm install --prefix apps/web`.
2. Ejecuta `npm run infra:up`.
3. En una terminal, ejecuta `npm run dev:api`.
4. En otra terminal, ejecuta `npm run dev:web`.
5. Para verificar la etapa 0, ejecuta `npm run test:api`, `npm run test:web` y `npm run build:api`.
6. Al terminar, ejecuta `npm run infra:down`.
