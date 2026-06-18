## 1. Refactor de estructura visual

- [x] 1.1 Ajustar `ClubPhonebookView` para que cada ficha priorice nombre, teléfonos y email en una composición de card más cercana a un patrón de grid list
- [x] 1.2 Reubicar `Dirección` y `Observaciones` como información secundaria dentro de la ficha sin perder compatibilidad con campos opcionales
- [x] 1.3 Verificar que la nueva ficha no renderiza avatar ni icono de contacto en ninguna variante

## 2. Layout grid y estilos

- [x] 2.1 Refactorizar el SCSS global de `Teléfonos de interés` para presentar el listado como una rejilla de cards homogéneas inspirada en `Grid Lists`
- [x] 2.2 Ajustar espaciados, jerarquía tipográfica y responsive del grid para móvil y escritorio sin usar estilos inline

## 3. Verificación

- [ ] 3.1 Verificar manualmente que la lista de contactos se percibe como un grid de fichas y no como bloques apilados inconsistentes
- [ ] 3.2 Verificar manualmente que la información básica del contacto destaca y que email, dirección u observaciones siguen siendo legibles cuando existan
- [x] 3.3 Ejecutar `dotnet build` y las pruebas automatizadas aplicables antes de marcar tareas como completadas
