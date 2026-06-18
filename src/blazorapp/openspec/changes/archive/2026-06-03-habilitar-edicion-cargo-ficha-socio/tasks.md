## 1. Contratos y datos de cargo

- [x] 1.1 Revisar o ampliar el contrato compartido necesario para cargar el catálogo de cargos desde `SharedUI`
- [x] 1.2 Añadir el modelo de opción de cargo o mapeo necesario para resolver `Id` y descripción legible
- [x] 1.3 Incorporar en la lógica de ficha el estado necesario para distinguir edición `Admin` y vista no editable `Member`

## 2. Ficha de socio de usuario autenticado

- [x] 2.1 Sustituir `member-cargo-id` por un selector de cargos cuando el usuario autenticado tenga rol `Admin`
- [x] 2.2 Mostrar el cargo en modo no editable y con nombre legible cuando el usuario autenticado tenga rol `Member`
- [x] 2.3 Mantener el guardado de la ficha enviando el `CargoId` correcto tras los cambios

## 3. Ficha administrativa de socio

- [x] 3.1 Aplicar el mismo patrón de selector legible en `AdminMemberProfileView`
- [x] 3.2 Reutilizar la carga del catálogo de cargos y la resolución del nombre sin duplicar lógica

## 4. Verificación

- [x] 4.1 Verificar manual o automáticamente el comportamiento `Admin` con cambio de cargo
- [x] 4.2 Verificar manual o automáticamente el comportamiento `Member` con campo visible no editable
