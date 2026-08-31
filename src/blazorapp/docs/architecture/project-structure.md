# Estructura del repositorio

## Solucion

- `IndaloaventurApp.sln`: solucion principal del frontend.
- `IndaloaventurApp.Web/IndaloaventurApp.Web.sln`: solucion del host web.

## Proyectos

### `IndaloaventurApp.SharedUI`

Contiene UI y contratos compartidos:

- `Components`: componentes Razor por area funcional.
- `Components/*/*.razor.cs`: code-behind partial para la logica C# de cada componente.
- `Abstractions`: interfaces que los componentes consumen.
- `Models`: modelos de formularios, peticiones, respuestas y objetos de vista.
- `Resources`: literales localizados con `SharedTexts`.

### `IndaloaventurApp.Web.Client`

Contiene la aplicacion cliente:

- `Pages`: paginas con `@page`, normalmente envoltorios delgados que alojan componentes de `SharedUI`.
- `Infrastructure`: implementaciones concretas de servicios, sobre todo clientes HTTP.
- `DependencyInjection/ServiceCollectionExtensions.cs`: registro central de servicios frontend.
- `Components/SessionInitializationGate`: compuerta de inicializacion de sesion persistida.
- `wwwroot/appsettings*.json`: configuracion cliente.

### `IndaloaventurApp.Web`

Contiene el host:

- `Program.cs`: pipeline ASP.NET Core, localizacion, render modes y endpoints locales.
- `Infrastructure/FoodAlerts`: proxy/servicio externo para alertas alimentarias.
- `wwwroot/scss`: estilos globales organizados.

### `IndaloaventurApp.Frontend.Tests`

Contiene tests de frontend:

- `Features/*`: tests agrupados por area funcional.
- `TestDoubles.cs`: dobles compartidos de servicios, localizer y HTTP.

## Regla practica

Si el codigo describe comportamiento visual reutilizable, empieza en `SharedUI`. Si define una URL o integra el host cliente, empieza en `Web.Client`. Si habla con una API o sistema externo, empieza en `Infrastructure`.
