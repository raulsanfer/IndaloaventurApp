## Why

La aplicacion necesita una documentacion tecnica navegable que explique su infraestructura Blazor, la separacion entre proyectos, componentes y servicios, y los flujos funcionales principales sin depender de conocimiento tribal del repositorio.

DocFX permite consolidar documentacion conceptual en Markdown y referencia tecnica generada desde comentarios XML de C#, creando una web estatica local o publicable para consulta por el equipo.

## What Changes

- Crear una estructura de documentacion tecnica basada en DocFX.
- Incorporar paginas conceptuales de arquitectura, estructura del repositorio, infraestructura transversal, separacion de componentes, servicios, clientes API, autenticacion/sesion y estrategia de testing.
- Incorporar paginas por feature para explicar los flujos principales de la aplicacion: autenticacion, club, licencias, socios/perfil, signals, alertas alimentarias y settings/admin.
- Configurar la generacion de referencia API desde los proyectos C# relevantes mediante comentarios XML, priorizando `SharedUI`, `Web.Client` y los contratos/modelos que sean utiles para entender el frontend.
- Definir un flujo de build/preview local para generar y visualizar la web de documentacion.
- Mantener la documentacion como artefacto tecnico del repositorio, sin cambiar comportamiento funcional de la aplicacion Blazor.

## Capabilities

### New Capabilities

- `technical-documentation-site`: Cubre la existencia, estructura, contenido minimo y proceso de generacion de una web de documentacion tecnica con DocFX.

### Modified Capabilities

- None.

## Impact

- Nuevos archivos de documentacion bajo una carpeta dedicada, previsiblemente `docs/`.
- Nueva configuracion DocFX, previsiblemente `docs/docfx.json`, `docs/toc.yml` y TOCs por seccion.
- Posibles ajustes no funcionales en proyectos `.csproj` para habilitar XML documentation output cuando sea necesario.
- Posible incorporacion de instrucciones de ejecucion en README o documentacion de desarrollo.
- No se esperan cambios en endpoints, componentes de runtime, rutas de usuario ni contratos backend.
