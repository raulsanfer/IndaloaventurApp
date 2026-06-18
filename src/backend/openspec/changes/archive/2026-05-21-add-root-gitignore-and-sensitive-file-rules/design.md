## Context

Actualmente el repositorio no tiene una definicion consolidada de archivos ignorados en su raiz. En un entorno .NET con herramientas de IDE y pruebas, esto aumenta la probabilidad de incluir artefactos temporales, binarios y configuraciones locales sensibles en commits.

## Goals / Non-Goals

**Goals:**
- Definir un `.gitignore` raiz alineado con stack .NET y herramientas usadas en el proyecto.
- Excluir artefactos de compilacion, caches locales, resultados de test y archivos temporales de IDE.
- Incluir reglas para reducir riesgo de commit de datos sensibles en archivos locales de configuracion/secrets.
- Mantener excepciones necesarias para no ignorar archivos fuente ni configuraciones versionables requeridas.

**Non-Goals:**
- Reescribir historial git ni eliminar secretos ya comprometidos historicamente.
- Reemplazar politicas de seguridad de CI/CD (secret scanning, branch protection).
- Estandarizar gitignore de subrepositorios externos no controlados por este repo.

## Decisions

1. Crear un unico `.gitignore` en la raiz del repo.
- Rationale: centraliza reglas y reduce divergencia entre carpetas.
- Alternativa: gitignore por subcarpeta; descartada por mantenimiento disperso.

2. Basar reglas en categorias explicitas: build, IDE, test, OS, local secrets.
- Rationale: facilita auditoria y evita reglas ambiguas.
- Alternativa: copiar plantilla generica extensa sin curado; descartada por ruido y falsos positivos.

3. No ignorar archivos de configuracion base versionables (ej. `appsettings.json`) y preferir ignorar variantes locales/sensibles.
- Rationale: preserva configuracion necesaria para ejecutar proyecto con valores no secretos.
- Alternativa: ignorar toda configuracion `appsettings*`; descartada por romper onboarding.

4. Añadir una tarea de validacion de `git status` para confirmar que artefactos locales quedan excluidos.
- Rationale: garantiza comportamiento esperado antes de cerrar cambio.

## Risks / Trade-offs

- [Reglas demasiado amplias oculten archivos utiles] → Mitigar con lista de excepciones y revision manual.
- [Reglas demasiado estrictas no cubran secretos locales] → Mitigar incluyendo patrones de secrets comunes y archivo de ejemplo documentado.
- [Diferencias entre IDEs no contempladas] → Mitigar incluyendo patrones de VS/VSCode/Rider mas comunes.

## Migration Plan

1. Crear `.gitignore` en raiz con secciones comentadas por categoria.
2. Revisar estado local con `git status --ignored` para validar cobertura.
3. Ajustar reglas con excepciones si se detectan falsos positivos.
4. Documentar convenciones basicas para nuevos archivos locales sensibles.

Rollback:
- Revertir `.gitignore` al commit anterior si alguna regla bloquea archivos requeridos.
- Mantener una lista breve de excepciones en el mismo archivo para correcciones rapidas.

## Open Questions

- Se desea ignorar explicitamente `.env` y variantes (`.env.*`) en todo el repo o solo en backend?
- Deben agregarse patrones de herramientas adicionales de equipo (por ejemplo, Docker Desktop, SQL local dumps)?
