# Infra

Espacio reservado para recursos de infraestructura local y despliegue.

## PostgreSQL local

- `docker-compose.yml` levanta una instancia local de PostgreSQL para desarrollo.
- Copia `infra/.env.example` a `infra/.env` si necesitas personalizar credenciales o puertos.
- La base expuesta por defecto coincide con la configuracion de desarrollo de la API.
- El puerto por defecto para desarrollo local es `5433` para evitar conflictos con instalaciones locales de PostgreSQL.

Comandos base:

- `docker compose --env-file infra/.env -f infra/docker-compose.yml up -d`
- `docker compose --env-file infra/.env -f infra/docker-compose.yml down`
