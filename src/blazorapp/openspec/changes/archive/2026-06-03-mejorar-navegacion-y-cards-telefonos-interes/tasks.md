## 1. Cabecera y navegación contextual

- [x] 1.1 Sustituir en `ClubPhonebookView` el enlace superior y el eyebrow actual por un breadcrumb accionable con `Volver a Mi Club` y `Área del socio` como contexto actual
- [x] 1.2 Eliminar de la vista de `Teléfonos de interés` la descripción bajo el título manteniendo el encabezado principal accesible y compacto

## 2. Presentación de fichas de contacto

- [x] 2.1 Ajustar la estructura Razor de cada contacto para reforzar la separación visual por card sin perder nombre, teléfonos, email y datos opcionales
- [x] 2.2 Reorganizar los bloques internos de teléfono y metadatos para favorecer el escaneo rápido en móvil y mantener enlaces `tel:` y `mailto:` operativos

## 3. Estilos y recursos

- [x] 3.1 Actualizar el SCSS global de `Mi Club` y `Teléfonos de interés` para el nuevo breadcrumb y el nuevo acabado visual de cards, sin estilos inline
- [x] 3.2 Revisar y ajustar los recursos localizados reutilizados por la pantalla para asegurar consistencia textual del breadcrumb y la cabecera

## 4. Verificación

- [ ] 4.1 Verificar manualmente que el breadcrumb permite volver a `Mi Club` y muestra `Área del socio` como página actual en móvil
- [ ] 4.2 Verificar manualmente que la pantalla ya no muestra la descripción bajo `Teléfonos de interés` y que el contenido útil aparece antes en viewport
- [ ] 4.3 Verificar manualmente que cada contacto se distingue como card independiente y que teléfonos, email y observaciones siguen siendo legibles
- [x] 4.4 Ejecutar `dotnet build` y las pruebas automatizadas aplicables antes de marcar tareas como completadas
