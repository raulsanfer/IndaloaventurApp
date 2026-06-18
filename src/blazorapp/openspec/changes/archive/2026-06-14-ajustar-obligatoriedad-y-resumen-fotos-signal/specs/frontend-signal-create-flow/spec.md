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

### Requirement: El tercer paso MUST requerir Foto 1 y permitir Foto 2 como opcional
El sistema MUST ofrecer en el tercer paso dos posiciones diferenciadas, `Foto 1` y `Foto 2`. `Foto 1` MUST ser obligatoria para continuar y `Foto 2` MUST poder permanecer vacía sin bloquear el flujo.

#### Scenario: Validación de Foto 1 obligatoria
- **WHEN** el usuario intenta avanzar desde el tercer paso sin haber informado `Foto 1`
- **THEN** el sistema MUST bloquear el avance al resumen
- **AND** el sistema MUST mostrar una validación comprensible indicando que `Foto 1` es obligatoria

#### Scenario: Avance con Foto 2 vacía
- **WHEN** el usuario ha informado `Foto 1` y deja `Foto 2` vacía
- **THEN** el sistema MUST permitir avanzar al resumen
- **AND** el sistema MUST conservar `Foto 2` como vacía sin introducir errores de validación

#### Scenario: Captura o selección de Foto 1 o Foto 2
- **WHEN** el usuario elige la acción de cámara o galería/archivo para `Foto 1` o `Foto 2`
- **THEN** el sistema MUST abrir el flujo soportado por el dispositivo y navegador
- **AND** el sistema MUST almacenar la imagen resultante en la posición correspondiente del formulario

### Requirement: El paso de fotos MUST normalizar imágenes antes de aplicar el límite máximo
El sistema MUST mantener un límite efectivo de 2 MB por foto para el contenido final del formulario, pero MUST intentar redimensionar y/o recomprimir automáticamente las imágenes seleccionadas antes de rechazarlas por tamaño.

#### Scenario: Imagen original superior al límite pero adaptable
- **WHEN** el usuario selecciona o captura una imagen cuyo tamaño original supera 2 MB
- **THEN** el sistema MUST intentar generar automáticamente una versión optimizada válida para el formulario
- **AND** el sistema MUST aceptar la imagen si la versión optimizada queda en 2 MB o menos
- **AND** el sistema MUST usar la versión optimizada para preview y guardado

#### Scenario: Imagen no adaptable dentro del límite
- **WHEN** el sistema no puede reducir una imagen seleccionada o capturada a 2 MB o menos manteniendo un resultado procesable
- **THEN** el sistema MUST rechazar esa imagen concreta
- **AND** el sistema MUST mostrar un mensaje de error comprensible indicando que no ha sido posible adaptarla automáticamente
- **AND** el sistema MUST conservar sin cambios el resto de datos ya introducidos en el wizard

### Requirement: El último paso MUST mostrar un resumen editable de la segunda foto antes de guardar
El sistema MUST mostrar un paso final de resumen con la información recopilada y un botón `Guardar`. En ese resumen, las previews de fotos MUST mostrarse en formato compacto a dos columnas y `Foto 2` MUST poder eliminarse mediante una acción visual con icono de cruz.

#### Scenario: Resumen previo al guardado con previews compactas
- **WHEN** el usuario llega al último paso del flujo
- **THEN** el sistema MUST mostrar un resumen de tipología, datos principales y fotos disponibles
- **AND** el sistema MUST renderizar las previews de `Foto 1` y `Foto 2` con un tamaño reducido para ocupar aproximadamente el 50 % del ancho disponible cada una
- **AND** el sistema MUST mostrar un botón `Guardar` para confirmar el alta

#### Scenario: Eliminación de Foto 2 desde el resumen
- **WHEN** el usuario pulsa la acción con icono de cruz sobre `Foto 2` en el resumen
- **THEN** el sistema MUST eliminar `Foto 2` del borrador actual
- **AND** el sistema MUST actualizar el resumen sin obligar a reiniciar el wizard
- **AND** el sistema MUST mantener `Foto 1` y el resto de datos ya informados

### Requirement: El guardado MUST crear una señal activa con Foto 1 y Foto 2 opcional
El sistema MUST enviar el alta mediante `POST /api/signals` usando el contrato de creación vigente y MUST registrar la nueva señal como activa por defecto. `foto1` MUST enviarse siempre y `foto2` MUST enviarse solo si existe o como valor vacío/nulo compatible con el backend.

#### Scenario: Guardado correcto con una sola foto
- **WHEN** el usuario confirma el resumen final con datos válidos y solo `Foto 1` informada
- **THEN** el sistema MUST enviar `tipo`, `descripcion`, `tags`, `latitud`, `longitud` y `foto1`
- **AND** el sistema MUST omitir `foto2` o enviarla vacía/nula según el contrato backend compatible
- **AND** el sistema MUST enviar la nueva señal con `activo=true`

#### Scenario: Guardado correcto con dos fotos
- **WHEN** el usuario confirma el resumen final con datos válidos y `Foto 1` y `Foto 2` informadas
- **THEN** el sistema MUST enviar ambas imágenes en `foto1` y `foto2`
- **AND** el sistema MUST enviar la nueva señal con `activo=true`

#### Scenario: Error al guardar la señal
- **WHEN** el backend no puede completar el alta de la señal
- **THEN** el sistema MUST mostrar un estado de error comprensible
- **AND** el sistema MUST evitar perder silenciosamente la información ya introducida durante la sesión actual
