## MODIFIED Requirements

### Requirement: El tercer paso MUST permitir capturar dos fotos
El sistema MUST ofrecer en el tercer paso dos acciones diferenciadas, `Foto 1` y `Foto 2`, para asociar las dos imágenes soportadas por la señal y MUST permitir, en dispositivos y navegadores compatibles, obtener cada imagen desde cámara o desde galería/selector de archivos.

#### Scenario: Captura de Foto 1 o Foto 2 con cámara
- **WHEN** el usuario elige la acción de cámara para `Foto 1` o `Foto 2`
- **THEN** el sistema MUST abrir el flujo de captura soportado por el dispositivo y navegador
- **AND** el sistema MUST almacenar la imagen resultante en la posición correspondiente del formulario

#### Scenario: Selección de Foto 1 o Foto 2 desde galería o archivos
- **WHEN** el usuario elige la acción de galería o archivo para `Foto 1` o `Foto 2`
- **THEN** el sistema MUST abrir el selector de imágenes disponible en el dispositivo
- **AND** el sistema MUST permitir seleccionar una imagen existente sin obligar al uso de la cámara
- **AND** el sistema MUST almacenar la imagen seleccionada en la posición correspondiente del formulario

## ADDED Requirements

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
