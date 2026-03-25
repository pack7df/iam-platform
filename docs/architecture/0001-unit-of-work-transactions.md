# Decision Record: Unit of Work Pattern for Transaction Management

**Estado:** Aceptado  
**Fecha:** 25 de Marzo, 2026

## Contexto
En la transición a PostgreSQL (Etapa 3), se identificó un riesgo de integridad de datos. Los repositorios realizaban `SaveChangesAsync()` de forma individual, lo que impedía que operaciones complejas (ej. registrar un Tenant junto a su Admin) fueran atómicas. Un fallo en la segunda operación dejaba la base de datos en un estado inconsistente.

## Decisión
Se implementa el patrón **Unit of Work** para centralizar la persistencia de cambios.

1.  **Abstracción**: Se crea la interfaz `IUnitOfWork` en la capa de Dominio (`Domain.Common`).
2.  **Implementación**: `IamPlatformDbContext` implementa `IUnitOfWork`.
3.  **Desacoplamiento**: Los repositorios (`UserRepository`, `TenantRepository`, etc.) ya no llaman a `SaveChangesAsync()`. Solo se encargan de registrar/rastrear cambios en el contexto.
4.  **Orquestación**: Los servicios de Aplicación (Use Cases) inyectan `IUnitOfWork` y llaman a `SaveChangesAsync()` al finalizar exitosamente todas las operaciones de dominio.

## Consecuencias
- **Positivas**: Garantía de atomicidad (ACID) en PostgreSQL, cumplimiento de SOLID (Sustitución de Liskov e Inversión de Dependencias), y mayor facilidad para testing transaccional.
- **Negativas**: Mayor complejidad en los servicios de aplicación al tener que coordinar la persistencia.
