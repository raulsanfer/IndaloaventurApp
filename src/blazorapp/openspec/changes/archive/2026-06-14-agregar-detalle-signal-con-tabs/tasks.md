## 1. Preparacion del rediseño

- [x] 1.1 Revisar la implementacion actual del detalle de signal y localizar el breadcrumb existente que debe mantenerse sin cambios funcionales.
- [x] 1.2 Mapear el boceto de `openspec/design/signal/detail` a componentes DaisyUI reutilizables antes de proponer clases SCSS nuevas.

## 2. Composicion de la pagina

- [x] 2.1 Rediseñar la cabecera del detalle para mostrar titulo, categoria, estado y metadato temporal principal con DaisyUI.
- [x] 2.2 Reorganizar el bloque resumen del detalle con cards o stats DaisyUI para los metadatos principales.
- [x] 2.3 Mantener las tabs `Datos de la signal` y `Mapa/Ubicacion`, adaptando su apariencia al nuevo layout sin cambiar su comportamiento funcional.

## 3. Estados y restricciones visuales

- [x] 3.1 Adaptar los estados de carga, error, no encontrado y ausencia de coordenadas al nuevo diseño usando componentes DaisyUI coherentes.
- [x] 3.2 Confirmar que el breadcrumb actual permanece visible y sin rediseño en todos los estados de la pagina.
- [x] 3.3 Evitar CSS inline o estilos de componente; si hace falta SCSS, limitarlo a ajustes estructurales minimos y compartidos.

## 4. Validacion

- [ ] 4.1 Verificar manualmente en movil y escritorio la correspondencia con el boceto de `openspec/design/signal/detail`.
- [x] 4.2 Verificar manual o automaticamente la navegacion desde el listado, el acceso directo por URL y el cambio entre tabs tras el rediseño.
