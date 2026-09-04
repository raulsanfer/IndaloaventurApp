# IndaloAventurApp

IndaloAventurApp es una aplicacion para el club Indalo Aventura. El repositorio contiene un frontend Blazor y un backend ASP.NET Core separados en soluciones independientes, con documentacion tecnica generada con DocFX.

## Resumen del proyecto

### Frontend Blazor

El frontend vive en `src/blazorapp` y esta construido sobre .NET 9:

- `IndaloaventurApp.Web`: host ASP.NET Core de la Blazor Web App, render interactivo, recursos estaticos, configuracion PWA y endpoints auxiliares del host.
- `IndaloaventurApp.Web.Client`: cliente Blazor WebAssembly, paginas ruteadas, clientes HTTP y registro de servicios frontend.
- `IndaloaventurApp.SharedUI`: componentes Razor reutilizables, modelos compartidos, abstracciones de servicios y recursos de localizacion.
- `IndaloaventurApp.Frontend.Tests`: tests xUnit/bUnit por feature.

Funcionalidades principales documentadas: autenticacion, sesion, recuperacion de password, Mi Cuenta, Mi Club, telefonos de interes, licencias federativas, senales, alertas alimentarias, navegacion PWA y pantallas de configuracion.

### Backend

El backend vive en `src/backend` y esta construido sobre ASP.NET Core Web API en .NET 9:

- `IndaloAventurApi.Api`: entrada HTTP, controllers, Swagger, CORS, autenticacion JWT, autorizacion y ProblemDetails.
- `IndaloAventurApi.Application`: casos de uso, comandos, queries y contratos de aplicacion.
- `IndaloAventurApi.Domain`: modelo de dominio y reglas centrales.
- `IndaloAventurApi.Infrastructure`: persistencia, Identity, JWT, integraciones externas, correo, WordPress y almacenamiento de imagenes.
- `tests`: tests de aplicacion, dominio, arquitectura e integracion.

El backend expone la API consumida por el frontend y usa configuracion para base de datos, JWT, CORS, seed de administrador, autenticacion social, email, WordPress y almacenamiento de imagenes.

## Requisitos

- .NET SDK 9.
- DocFX como herramienta global para generar la documentacion tecnica.
- SQL Server o LocalDB para ejecutar el backend con persistencia real.
- Certificado local de desarrollo HTTPS confiado si se usan los perfiles `https`.

Instalar o actualizar DocFX:

```powershell
dotnet tool update -g docfx
```

## Estructura

```text
src/
  backend/
    IndaloAventurApi.sln
    src/
    tests/
    openspec/
  blazorapp/
    IndaloaventurApp.sln
    IndaloaventurApp.Web/
    IndaloaventurApp.SharedUI/
    IndaloaventurApp.Frontend.Tests/
    docs/
    openspec/
```

## Ejecutar en local

Restaurar dependencias:

```powershell
dotnet restore src/backend/IndaloAventurApi.sln
dotnet restore src/blazorapp/IndaloaventurApp.sln
```

Ejecutar el backend:

```powershell
dotnet run --project src/backend/src/IndaloAventurApi.Api/IndaloAventurApi.Api.csproj --launch-profile https
```

URL por defecto:

- API: `https://localhost:7160`
- Swagger en desarrollo: `https://localhost:7160/swagger`

Ejecutar el frontend:

```powershell
dotnet run --project src/blazorapp/IndaloaventurApp.Web/IndaloaventurApp.Web/IndaloaventurApp.Web.csproj --launch-profile https
```

URL por defecto:

- App: `https://localhost:7095`

Para desarrollo local, el frontend debe apuntar a la URL local del backend mediante `ApiSettings:BaseUrl`. Los valores sensibles deben configurarse con user secrets, variables de entorno o archivos locales ignorados por Git.

## Configuracion local

No subas secretos al repositorio. La `.gitignore` ya excluye archivos como `.env`, `secrets.json`, `appsettings.Development.json`, `appsettings.Local.json`, certificados y `appsetting_api.json`.

Configuracion habitual del backend:

- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:api_ContextConnection`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `Jwt:AccessTokenMinutes`
- `AdminSeed:Email`
- `AdminSeed:Password`
- `Cors:AllowedOrigins`
- `SocialAuth:GoogleAudience`
- `Email:*`
- `WordPress:*`
- `SignalImageStorage:RootPath`

Configuracion habitual del frontend:

- `ApiSettings:BaseUrl`
- `GoogleAuth:ClientId`
- `FoodAlerts:BaseUrl`

El `ClientId` publico de Google no es una clave secreta, pero cualquier password, connection string, application password de WordPress, clave JWT o credencial de email debe tratarse como privado.

## Tests

Ejecutar todos los tests del backend:

```powershell
dotnet test src/backend/IndaloAventurApi.sln
```

Ejecutar los tests del frontend:

```powershell
dotnet test src/blazorapp/IndaloaventurApp.sln
```

## Documentacion tecnica

La documentacion vive en `src/blazorapp/docs` y se genera con DocFX.

Desde la raiz del repositorio:

```powershell
cd src/blazorapp
docfx docs/docfx.json
docfx docs/docfx.json --serve
```

`docfx docs/docfx.json` genera el sitio estatico en `src/blazorapp/docs/_site/`. `--serve` levanta una vista local para revisar navegacion, busqueda y enlaces antes de publicar o compartir cambios.

## Documentacion relacionada

- `src/blazorapp/docs/index.md`
- `src/blazorapp/docs/development.md`
- `src/blazorapp/docs/architecture/overview.md`
- `src/blazorapp/security.md`
