# IndaloAventurApp - documentacion tecnica

Esta documentacion explica como esta organizada la aplicacion Blazor del club y como evolucionarla sin romper sus limites principales.

El contenido esta pensado para desarrolladores. La referencia API generada por DocFX complementa a las paginas conceptuales, pero la fuente principal para entender el sistema son las secciones de arquitectura y features.

## Lectura recomendada

1. [Vista general de arquitectura](architecture/overview.md)
2. [Estructura del repositorio](architecture/project-structure.md)
3. [SharedUI y componentes](architecture/shared-ui.md)
4. [Clientes HTTP](architecture/http-api-clients.md)
5. [Testing](architecture/testing-strategy.md)
6. [Features](features/toc.yml)

## Comandos locales

Instalar o actualizar DocFX:

```powershell
dotnet tool update -g docfx
```

Restaurar dependencias:

```powershell
dotnet restore
```

Generar la web estatica:

```powershell
docfx docs/docfx.json
```

Previsualizar localmente:

```powershell
docfx docs/docfx.json --serve
```

La salida generada queda en `docs/_site/`.
