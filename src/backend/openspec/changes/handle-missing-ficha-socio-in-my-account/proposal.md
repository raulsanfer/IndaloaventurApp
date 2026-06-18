## Why

En la pagina "Mi Cuenta" se produce una excepcion cuando el usuario aun no tiene ficha de socio creada, interrumpiendo la carga del resto de componentes. Se necesita que esta ausencia sea un estado valido y no un error para mejorar robustez y experiencia de uso.

## What Changes

- Cambiar el flujo de consulta de ficha de socio para que la ausencia de ficha no genere excepcion.
- Definir respuesta de API tolerante cuando la ficha del usuario no exista todavia.
- Mantener validaciones de autorizacion actuales (solo cambia el caso "ficha no encontrada").
- Ajustar pruebas de aplicacion/integracion para cubrir el escenario sin ficha.

## Capabilities

### New Capabilities
- `ficha-socio-optional-read`: lectura de ficha de socio que permite resultado vacio cuando el usuario aun no tiene ficha.

### Modified Capabilities
Ninguna.

## Impact

- Aplicacion: `GetFichaSocioQueryHandler` y contratos de salida asociados.
- API: endpoint(s) de ficha de socio consumidos por "Mi Cuenta".
- Tests: cobertura de escenario "sin ficha" sin excepcion.
