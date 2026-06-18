## 1. Entrada de creación en SignalHome

- [x] 1.1 Añadir el FAB de creación en `SignalHome` con color primario, icono `+` blanco y posición fija en esquina inferior derecha
- [x] 1.2 Conectar el FAB con la ruta del flujo de alta de señales manteniendo la coherencia del shell autenticado

## 2. Base del wizard de creación

- [x] 2.1 Crear la nueva pantalla o contenedor del flujo de alta con estado compartido entre steps
- [x] 2.2 Implementar el indicador horizontal de pasos con DaisyUI y el marcado del paso actual/completado
- [x] 2.3 Añadir la navegación entre steps con validación progresiva y posibilidad de retroceso

## 3. Captura de datos del formulario

- [x] 3.1 Implementar el paso de selección de tipología cargando todas las opciones desde `signal-types`
- [x] 3.2 Implementar el paso de datos principales con ubicación opcional, descripción obligatoria y tags separados por comas
- [x] 3.3 Implementar el paso de fotos con los botones `Foto 1` y `Foto 2` conectados a captura o selección de imagen compatible con cámara móvil
- [x] 3.4 Implementar el paso final de resumen con revisión de datos y botón `Guardar`

## 4. Integración con API y estados

- [x] 4.1 Extender el servicio de `signals` para soportar `POST /api/signals`
- [x] 4.2 Mapear el formulario del wizard al contrato `CreateSignalRequest`, incluyendo `activo=true` por defecto
- [x] 4.3 Implementar estados de carga, error y confirmación para el guardado

## 5. Recursos, estilos y validación

- [x] 5.1 Añadir los literales localizados del FAB, steps, campos, validaciones, fotos, resumen y guardado
- [x] 5.2 Crear los estilos DaisyUI + SCSS complementarios del flujo de alta y ajustar su comportamiento en móvil
- [ ] 5.3 Verificar manualmente el recorrido completo: FAB, selección de tipología, datos, cámara/fotos, resumen y guardado final
