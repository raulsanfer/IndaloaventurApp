## ADDED Requirements

### Requirement: El sistema MUST ofrecer un flujo guiado de creación de señales
El sistema MUST ofrecer un proceso secuencial de creación de `Signal` accesible para cualquier usuario autenticado con acceso a `Signals`, organizado en varios pasos y orientado a uso móvil.

#### Scenario: Inicio del flujo de creación
- **WHEN** el usuario entra al flujo de creación desde `SignalHome`
- **THEN** el sistema MUST mostrar una pantalla específica de alta de señal
- **AND** el sistema MUST iniciar el proceso en el paso de selección de tipología

### Requirement: El flujo de creación MUST mostrar un indicador horizontal de pasos
El sistema MUST mostrar en la parte superior de cada pantalla o estado del wizard un componente horizontal de `steps` que indique visualmente el paso actual y el progreso completado.

#### Scenario: Progreso visible durante el wizard
- **WHEN** el usuario avanza por el flujo de creación de señales
- **THEN** el sistema MUST mostrar un indicador horizontal con los pasos del proceso
- **AND** el sistema MUST resaltar el paso actual
- **AND** el sistema MUST marcar como completados los pasos anteriores

### Requirement: El primer paso MUST exigir la selección de tipología
El sistema MUST mostrar en el primer paso el listado completo de tipologías disponibles, cada una con su icono a la izquierda y su nombre a continuación, y MUST exigir una selección para continuar.

#### Scenario: Selección de tipología
- **WHEN** el usuario visualiza el primer paso del flujo
- **THEN** el sistema MUST cargar todas las tipologías desde `signal-types`
- **AND** el sistema MUST mostrar cada tipología como opción seleccionable con icono y nombre
- **AND** el sistema MUST avanzar al siguiente paso cuando el usuario seleccione una tipología válida

### Requirement: El segundo paso MUST capturar los datos principales de la señal
El sistema MUST recoger en un segundo paso la ubicación opcional, la descripción obligatoria y los tags de la señal como texto libre separado por comas.

#### Scenario: Captura de datos con ubicación opcional
- **WHEN** el usuario visualiza el segundo paso del flujo
- **THEN** el sistema MUST permitir informar una ubicación opcional compatible con el contrato de coordenadas
- **AND** el sistema MUST permitir continuar aunque la ubicación no se informe

#### Scenario: Validación de descripción obligatoria
- **WHEN** el usuario intenta avanzar desde el segundo paso sin descripción
- **THEN** el sistema MUST bloquear el avance
- **AND** el sistema MUST mostrar una validación comprensible indicando que la descripción es obligatoria

#### Scenario: Captura de tags
- **WHEN** el usuario informa etiquetas en el segundo paso
- **THEN** el sistema MUST permitir introducirlas como texto libre separado por comas
- **AND** el sistema MUST conservar ese formato para su posterior guardado

### Requirement: El tercer paso MUST permitir capturar dos fotos
El sistema MUST ofrecer en el tercer paso dos acciones diferenciadas, `Foto 1` y `Foto 2`, para capturar o asociar las dos imágenes soportadas por la señal.

#### Scenario: Captura de Foto 1 o Foto 2
- **WHEN** el usuario pulsa `Foto 1` o `Foto 2`
- **THEN** el sistema MUST abrir el flujo de cámara o captura soportado por el dispositivo y navegador
- **AND** el sistema MUST almacenar la imagen resultante en la posición correspondiente del formulario

### Requirement: El último paso MUST mostrar un resumen antes de guardar
El sistema MUST mostrar un paso final de resumen con la información recopilada y un botón `Guardar` que complete la creación de la señal.

#### Scenario: Resumen previo al guardado
- **WHEN** el usuario llega al último paso del flujo
- **THEN** el sistema MUST mostrar un resumen de tipología, datos principales y fotos capturadas
- **AND** el sistema MUST mostrar un botón `Guardar` para confirmar el alta

### Requirement: El guardado MUST crear una señal activa con el contrato backend
El sistema MUST enviar el alta mediante `POST /api/signals` usando el contrato de creación vigente y MUST registrar la nueva señal como activa por defecto.

#### Scenario: Guardado correcto de la señal
- **WHEN** el usuario confirma el resumen final con datos válidos
- **THEN** el sistema MUST enviar `tipo`, `descripcion`, `tags`, `latitud`, `longitud`, `foto1` y `foto2` según corresponda
- **AND** el sistema MUST enviar la nueva señal con `activo=true`
- **AND** el sistema MUST informar al usuario de que la creación se ha completado correctamente

#### Scenario: Error al guardar la señal
- **WHEN** el backend no puede completar el alta de la señal
- **THEN** el sistema MUST mostrar un estado de error comprensible
- **AND** el sistema MUST evitar perder silenciosamente la información ya introducida durante la sesión actual
