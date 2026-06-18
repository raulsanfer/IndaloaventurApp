## ADDED Requirements

### Requirement: El sistema MUST permitir iniciar la edicion limitada de una signal propia desde el detalle
El sistema MUST permitir que el propietario autenticado de una signal inicie un flujo de edicion desde la accion `Editar` mostrada en el detalle y MUST precargar en ese flujo los valores actuales editables.

#### Scenario: Apertura del flujo con datos precargados
- **WHEN** el propietario de una signal activa la accion `Editar` desde el detalle
- **THEN** el sistema MUST abrir un flujo de edicion asociado a esa signal
- **AND** el sistema MUST precargar `Titulo`, `Descripcion` y `Estado` con los valores actuales

#### Scenario: Cancelacion sin guardar
- **WHEN** el usuario cancela la edicion antes de guardar
- **THEN** el sistema MUST abandonar el flujo de edicion sin persistir cambios
- **AND** el sistema MUST conservar el detalle original en modo de lectura

### Requirement: El flujo de edicion MUST limitar los campos editables
El sistema MUST permitir modificar exclusivamente `Titulo`, `Descripcion` y `Estado` dentro del flujo de edicion propia y MUST mantener fuera de alcance el resto de datos de la signal.

#### Scenario: Formulario con alcance restringido
- **WHEN** el flujo de edicion propia se muestra
- **THEN** el sistema MUST ofrecer controles editables solo para `Titulo`, `Descripcion` y `Estado`
- **AND** el sistema MUST no exponer como editables categoria, tags, coordenadas, fotos ni comentarios

#### Scenario: Campos no editables preservados
- **WHEN** el usuario guarda cambios validos en una signal propia
- **THEN** el sistema MUST preservar sin alteraciones los campos no editables por este flujo
- **AND** el sistema MUST reflejar en el detalle unicamente los cambios permitidos

### Requirement: El guardado MUST actualizar la signal propia y manejar estados de resultado
El sistema MUST persistir los cambios validos de una signal propia, MUST refrescar la informacion mostrada tras un guardado correcto y MUST informar de forma comprensible cualquier error de validacion, autorizacion o servicio.

#### Scenario: Guardado correcto de una signal propia
- **WHEN** el propietario guarda cambios validos en `Titulo`, `Descripcion` o `Estado`
- **THEN** el sistema MUST actualizar la signal correspondiente
- **AND** el sistema MUST volver al detalle mostrando los valores actualizados

#### Scenario: Error al guardar cambios
- **WHEN** el guardado no puede completarse por validacion o por error del servicio
- **THEN** el sistema MUST mostrar un mensaje de error comprensible
- **AND** el sistema MUST conservar los datos introducidos para que el usuario pueda corregir o reintentar

### Requirement: El flujo de edicion propia MUST impedir operaciones sobre signals ajenas
El sistema MUST impedir que un usuario complete la edicion de una signal cuya autoria no coincide con su sesion, incluso si la accion se intenta forzar fuera del flujo normal de la interfaz.

#### Scenario: Intento de edicion no autorizada
- **WHEN** el frontend intenta iniciar o guardar una edicion sobre una signal ajena
- **THEN** el sistema MUST denegar la operacion
- **AND** el sistema MUST mostrar un estado de autorizacion o error comprensible sin aplicar cambios
