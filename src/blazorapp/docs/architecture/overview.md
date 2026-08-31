# Vista general

IndaloAventurApp es una aplicacion Blazor Web App con render interactivo WebAssembly, soporte PWA y una libreria de componentes compartidos. El frontend consume los endpoints expuestos por IndaloaventurAPI mediante clientes HTTP tipados.

## Capas principales

- `IndaloaventurApp.Web`: host ASP.NET Core, configuracion del pipeline, render modes, endpoints locales auxiliares y recursos estaticos.
- `IndaloaventurApp.Web.Client`: aplicacion cliente Blazor WebAssembly, paginas ruteadas, registro de servicios frontend y clientes HTTP.
- `IndaloaventurApp.SharedUI`: componentes Razor reutilizables, modelos de UI/contrato, abstracciones de servicios y recursos localizados.
- `IndaloaventurApp.Frontend.Tests`: tests xUnit/bUnit organizados por feature.

## Flujo habitual de una pantalla

1. Una pagina ruteada en `IndaloaventurApp.Web.Client/Pages` define la URL y valida acceso basico.
2. La pagina renderiza un componente de `IndaloaventurApp.SharedUI/Components`.
3. El componente inyecta una abstraccion de `IndaloaventurApp.SharedUI/Abstractions`.
4. La implementacion concreta vive en `IndaloaventurApp.Web.Client/Infrastructure` y llama al API o al host.
5. Los modelos compartidos viven en `IndaloaventurApp.SharedUI/Models`.
6. Los resultados se devuelven como `ServiceResult<T>` para separar exito y error.

## Principio de diseno

El proyecto evita acoplar los componentes compartidos al host concreto. `SharedUI` debe poder reutilizarse en otro frontend Blazor, incluida una posible app hibrida o movil, siempre que se registren las mismas abstracciones de servicio.
