## ADDED Requirements

### Requirement: Acceso exclusivo a la ficha propia del socio
El sistema MUST ofrecer una página de `Ficha de Socio` accesible únicamente a usuarios autenticados cuyo `IsMember` sea `true`.

#### Scenario: Socio accede a su ficha
- **WHEN** un usuario autenticado con `IsMember = true` navega a la página de ficha de socio
- **THEN** el sistema MUST permitir el acceso a la pantalla y cargar su propia ficha

#### Scenario: Usuario no socio intenta acceder a la ficha
- **WHEN** un usuario autenticado con `IsMember = false` usa la aplicación o intenta abrir manualmente la URL
- **THEN** el sistema MUST no exponer desde la UI una vía operativa para editar la ficha de socio
- **THEN** el sistema MUST resolver un estado no operativo para esa pantalla

### Requirement: Carga y guardado de la ficha propia
La página de ficha de socio MUST cargar los datos desde `GET /api/fichas-socio/me` y MUST guardar cambios mediante `PUT /api/fichas-socio/me`.

#### Scenario: Carga correcta de ficha
- **WHEN** un socio abre su ficha
- **THEN** el sistema MUST invocar `GET /api/fichas-socio/me`
- **THEN** el sistema MUST mostrar los datos recibidos en el formulario editable

#### Scenario: Guardado correcto de ficha
- **WHEN** un socio envía cambios válidos en su ficha
- **THEN** el sistema MUST invocar `PUT /api/fichas-socio/me`
- **THEN** el sistema MUST reflejar en la UI la ficha actualizada devuelta por el API o un estado equivalente consistente

### Requirement: Campos excluidos de la edición del propio usuario
La página de ficha de socio MUST ocultar y MUST impedir la edición de `IsMember` y de cualquier dato de roles por parte del mismo usuario que visualiza su ficha.

#### Scenario: Renderizado de campos sensibles
- **WHEN** la ficha del socio se muestra en pantalla
- **THEN** el sistema MUST no renderizar controles editables para `IsMember`
- **THEN** el sistema MUST no renderizar controles editables para roles

#### Scenario: Envío de actualización
- **WHEN** el socio guarda su ficha
- **THEN** el sistema MUST no incluir `IsMember` ni roles en el payload de actualización

### Requirement: Formulario responsive y coherente con la app
La ficha de socio MUST mostrarse en un `fieldset` DaisyUI con breadcrumb, títulos y layout responsive coherentes con el resto de la aplicación.

#### Scenario: Pantalla en móvil
- **WHEN** la ficha se visualiza en una pantalla estrecha
- **THEN** el sistema MUST reorganizar los campos sin desbordes horizontales
- **THEN** el sistema MUST mantener visible y usable el botón `Guardar`

#### Scenario: Pantalla en escritorio
- **WHEN** la ficha se visualiza en una pantalla amplia
- **THEN** el sistema MUST aprovechar el espacio para mejorar legibilidad y agrupación de campos

### Requirement: Validación y seguridad por campo
Cada campo editable de la ficha MUST aplicar validaciones de valor y controles de seguridad antes de enviar cambios al API.

#### Scenario: Valor inválido en formulario
- **WHEN** el usuario introduce un valor que no cumple las reglas del campo
- **THEN** el sistema MUST mostrar feedback de validación asociado a ese campo
- **THEN** el sistema MUST impedir el guardado mientras existan errores

#### Scenario: Normalización del payload
- **WHEN** el usuario guarda una ficha válida
- **THEN** el sistema MUST normalizar y sanear los valores editables antes de construir `UpdateFichaSocioRequest`

### Requirement: Feedback operativo de ficha de socio
La pantalla de ficha de socio MUST mostrar estados de carga, éxito y error sin romper la composición principal.

#### Scenario: Error al cargar la ficha
- **WHEN** falla `GET /api/fichas-socio/me`
- **THEN** el sistema MUST mostrar un estado de error controlado con recuperación o reintento equivalente

#### Scenario: Error al guardar la ficha
- **WHEN** falla `PUT /api/fichas-socio/me`
- **THEN** el sistema MUST informar del error al usuario y MUST mantener el formulario en un estado consistente
