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

## Configuracion local

- La API toma la cadena principal desde `ConnectionStrings:IamPlatform`.
- `appsettings.json` define un valor base local.
- `appsettings.Development.json` separa la base de desarrollo local.
- Se puede sobreescribir por variable de entorno con `ConnectionStrings__IamPlatform`.

Ejemplo de cadena local:

- `Host=localhost;Port=5433;Database=iam_platform_dev;Username=iam_platform;Password=iam_platform`
