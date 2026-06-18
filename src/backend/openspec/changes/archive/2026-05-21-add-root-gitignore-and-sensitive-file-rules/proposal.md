## Why

El repositorio necesita una politica explicita de ignorado para evitar subir artefactos de compilacion, caches locales y posibles archivos sensibles. Definirlo ahora reduce riesgo de filtrado de secretos y mantiene el historial limpio y reproducible.

## What Changes

- Crear un `.gitignore` en la raiz del repo con reglas para ecosistema .NET y herramientas del proyecto.
- Ignorar resultados de compilacion (`bin/`, `obj/`), caches de IDE y archivos temporales locales.
- Ignorar archivos de configuracion con datos sensibles o dependientes del entorno cuando aplique.
- Documentar criterios minimos para no excluir archivos fuente necesarios del repositorio.

## Capabilities

### New Capabilities
- `repository-gitignore-hygiene`: Estandar de reglas de `.gitignore` para prevenir commits de artefactos y datos sensibles en el repositorio.

### Modified Capabilities
- No aplica.

## Impact

- Afecta la raiz del repositorio (`.gitignore`).
- Impacta el flujo de desarrollo local y CI al reducir ruido de archivos no versionables.
- Reduce riesgo de exposicion accidental de credenciales y secretos en commits.
