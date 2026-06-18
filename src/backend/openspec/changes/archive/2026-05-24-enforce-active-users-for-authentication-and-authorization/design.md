## Context

El sistema ya utiliza ASP.NET Identity y JWT para autenticar y autorizar usuarios, y ademas gestiona un estado de activacion/desactivacion de cuentas administradas. Sin una regla transversal explicita, pueden quedar huecos donde un usuario inactivo obtenga token o acceda a endpoints protegidos con credenciales/tokens previos.

## Goals / Non-Goals

**Goals:**
- Garantizar que solo usuarios activos puedan autenticarse.
- Garantizar que usuarios inactivos no puedan ser autorizados en endpoints protegidos.
- Homogeneizar el comportamiento de seguridad para login tradicional y social login.
- Definir pruebas que prevengan regresiones en este control.

**Non-Goals:**
- Cambiar el modelo de roles o politicas RBAC existentes.
- Rediseñar el formato de JWT o su estructura de claims.
- Introducir nuevos estados de cuenta distintos de activo/inactivo.

## Decisions

1. Validar estado activo antes de emitir JWT en casos de login.
- Rationale: evita generar sesiones nuevas para usuarios inactivos.
- Alternative considered: emitir token y filtrar solo en autorizacion; se descarta por abrir una ventana de uso no deseada.

2. Revalidar estado activo en el flujo de autorizacion de endpoints protegidos.
- Rationale: cubre tokens emitidos antes de la desactivacion y asegura revocacion efectiva por estado.
- Alternative considered: esperar expiracion natural del token; se descarta por no cumplir requisito de bloqueo inmediato.

3. Mantener contratos HTTP existentes (401/403) segun tipo de fallo.
- Rationale: minimiza impacto en clientes y conserva semantica REST ya usada.

## Risks / Trade-offs

- [Riesgo] Aumento de consultas a Identity en autorizacion de cada request protegida -> Mitigacion: revisar uso de cache y tiempos de expiracion de token.
- [Riesgo] Diferencias de comportamiento entre login normal y social login -> Mitigacion: pruebas de integracion para ambos caminos.
- [Trade-off] Bloqueo inmediato por estado activo puede invalidar sesiones en curso de usuarios desactivados -> Mitigacion: este es el comportamiento buscado por seguridad y debe documentarse.