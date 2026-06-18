## 1. Gitignore Definition

- [x] 1.1 Crear `.gitignore` en la raiz del repositorio con secciones para build/test, IDE, sistema operativo y temporales.
- [x] 1.2 Añadir reglas para excluir artefactos de compilacion .NET (`bin/`, `obj/`) y resultados de pruebas/cobertura.
- [x] 1.3 Añadir reglas para archivos locales potencialmente sensibles (por ejemplo `.env`, secretos locales y dumps temporales).

## 2. Validation And Safety Checks

- [x] 2.1 Verificar con `git status --ignored` que los artefactos locales quedan excluidos correctamente.
- [x] 2.2 Confirmar que archivos fuente y configuracion base versionable no quedan ignorados por error.
- [x] 2.3 Ajustar reglas o excepciones si se detectan falsos positivos/falsos negativos durante la validacion.
