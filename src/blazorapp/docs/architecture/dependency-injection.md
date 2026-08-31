# Inyeccion de dependencias

La composicion de servicios esta centralizada para que los componentes dependan de interfaces y no de implementaciones concretas.

## Registro cliente

`IndaloaventurApp.Web.Client/DependencyInjection/ServiceCollectionExtensions.cs` expone `AddIndaloFrontendServices`. Este metodo registra:

- `ISessionService` con `SessionService`;
- `GoogleAuthOptions`;
- clientes HTTP tipados para autenticacion, perfiles, usuarios admin, WordPress, agenda telefonica, signals, licencias federativas y cargos.

Cada cliente HTTP recibe `BaseAddress` desde configuracion y un timeout de 20 segundos.

## Arranque WebAssembly

`IndaloaventurApp.Web.Client/Program.cs` configura:

- localizacion;
- servicios frontend compartidos;
- `IFoodAlertService` con `FoodAlertAppApiClient`, apuntando al host de la propia app.

## Arranque del host

`IndaloaventurApp.Web/IndaloaventurApp.Web/Program.cs` configura:

- Razor Components con render interactivo server y WebAssembly;
- localizacion `es-ES`;
- servicios frontend;
- `IFoodAlertService` con `ExternalFoodAlertService`;
- endpoints locales de alertas alimentarias;
- assets estaticos y render modes.

## Criterio para nuevos servicios

1. Declara la interfaz en `SharedUI/Abstractions/<Feature>`.
2. Haz que el componente dependa de la interfaz.
3. Implementa la infraestructura en `Web.Client/Infrastructure/<Feature>`.
4. Registra la implementacion en `AddIndaloFrontendServices` o en el `Program.cs` correspondiente si depende del host.
5. Cubre el contrato con tests de cliente API y de componente.
