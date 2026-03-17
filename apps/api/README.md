# API

Solucion backend del IAM en ASP.NET Core Web API con separacion DDD.

- `src/`: proyectos de produccion.
- `tests/`: proyectos de prueba.
- `../..\IamPlatform.sln`: solucion raiz que conecta las capas base.

Capas iniciales:

- `IamPlatform.Api`: entrada HTTP y composition root.
- `IamPlatform.Application`: coordinacion de casos de uso y dependencias de aplicacion.
- `IamPlatform.Domain`: nucleo del dominio y sus invariantes.
- `IamPlatform.Infrastructure`: detalles tecnicos e integraciones.

Comando base de compilacion:

- `dotnet build IamPlatform.sln`
