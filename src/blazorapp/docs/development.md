# Desarrollo de la documentacion

## Objetivo

La documentacion debe responder rapido a tres preguntas:

- donde vive cada responsabilidad;
- como se conecta una pantalla con servicios, clientes API y modelos;
- que pruebas protegen cada flujo.

## Build y preview

```powershell
dotnet tool update -g docfx
dotnet restore
docfx docs/docfx.json
docfx docs/docfx.json --serve
```

`docfx docs/docfx.json` genera el sitio en `docs/_site/`. `--serve` levanta una vista local para revisar navegacion, busqueda y enlaces.

## Mantenimiento

Actualiza la documentacion cuando un cambio modifique:

- una ruta o pagina de `IndaloaventurApp.Web.Client/Pages`;
- una responsabilidad de `IndaloaventurApp.SharedUI/Components`;
- una abstraccion de `IndaloaventurApp.SharedUI/Abstractions`;
- un cliente de `IndaloaventurApp.Web.Client/Infrastructure`;
- un flujo de autenticacion, sesion, carga, error o permisos;
- la organizacion de tests en `IndaloaventurApp.Frontend.Tests/Features`.

No documentes cada metodo privado. Documenta decisiones, contratos publicos y recorridos que otro desarrollador necesite entender para hacer cambios seguros.
