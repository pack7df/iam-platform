# Web

Frontend administrativo inicial del IAM Platform.

## Scripts

- `npm install`
- `npm run dev`
- `npm run build`
- `npm run lint`
- `npm run test`
- `npm run test:watch`

## Configuracion local

- Usa `apps/web/.env.example` como referencia para definir `VITE_API_BASE_URL`.
- El valor local sugerido apunta a la API en `http://localhost:5125`.

## Testing

- La base de pruebas usa `Vitest` con `jsdom` y `@testing-library/react`.
- Ejecuta `npm run test` para correr la suite una vez.
- Ejecuta `npm run test:watch` para dejar Vitest observando cambios.

## Estructura

- `src/`: codigo fuente de la SPA.
- `src/test/`: setup compartido de testing.
