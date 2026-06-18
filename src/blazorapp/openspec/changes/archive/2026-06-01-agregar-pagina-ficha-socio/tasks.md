## 1. Ruta, acceso y modelo de datos

- [x] 1.1 Crear la nueva página de `Ficha de Socio` y decidir su integración de navegación desde `Mi Cuenta`
- [x] 1.2 Extender la capa `Member` para cargar y actualizar `GET/PUT /api/fichas-socio/me`
- [x] 1.3 Definir un modelo de edición que excluya `IsMember` y roles del flujo de autoservicio

## 2. UI y validaciones

- [x] 2.1 Implementar el componente compartido de ficha de socio con `fieldset` DaisyUI, breadcrumb, títulos y botón `Guardar`
- [x] 2.2 Construir un layout responsive y moderno para los campos personales, de contacto y consentimientos
- [x] 2.3 Añadir validaciones de valor y saneado de entrada/salida por cada campo editable

## 3. Estados, recursos y verificación

- [x] 3.1 Añadir estados de carga, éxito y error junto con recursos localizados ES y estilos SCSS globales necesarios
- [x] 3.2 Verificar la visibilidad solo para `IsMember = true` y el comportamiento no operativo para el resto
- [x] 3.3 Añadir o actualizar tests de componente/servicio y ejecutar la validación del frontend
