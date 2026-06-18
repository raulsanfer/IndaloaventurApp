## Why

El módulo relacionado con `AdventureMembers` ya no forma parte del flujo funcional actual y mantiene ruido técnico que dificulta el mantenimiento. Además, faltan pruebas base para proteger los casos de uso y endpoints clave de gestión de usuarios ante regresiones.

## What Changes

- Eliminar carpetas y artefactos de `AdventureMembers` que ya no se utilizan en la solución backend.
- Asegurar que la eliminación no deje referencias rotas en DI, rutas, proyectos o compilación.
- Añadir tests básicos para cada caso de uso activo del dominio de gestión de usuarios.
- Añadir tests básicos para endpoints de gestión de usuarios (escenarios de éxito y validaciones/autorización mínimas).
- Definir una base mínima de cobertura verificable en CI para estos componentes.

## Capabilities

### New Capabilities
- `unused-module-cleanup`: Define la eliminación segura de módulos/carpetas no usadas, incluyendo validación de referencias y compilación limpia.
- `user-management-test-baseline`: Define una batería mínima de pruebas para casos de uso y endpoints de gestión de usuarios.

### Modified Capabilities
- `jwt-identity-authentication`: Se amplía con requisitos verificables de pruebas base para endpoints de gestión de usuarios protegidos por autenticación/autorización.

## Impact

- Código afectado: capas `Api`, `Application`, `Domain`, `Infrastructure` y proyectos de test relacionados con usuarios.
- APIs afectadas: endpoints de gestión de usuarios ya existentes (sin añadir breaking changes contractuales).
- Dependencias/sistemas: pipeline de pruebas y configuración de proyectos de testing.
