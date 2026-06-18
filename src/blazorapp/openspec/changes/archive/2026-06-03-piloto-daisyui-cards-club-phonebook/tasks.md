## 1. Preparación del piloto

- [x] 1.1 Verificar o habilitar soporte real de Tailwind + DaisyUI en el frontend sin alterar todavía el resto de vistas
- [x] 1.2 Documentar el alcance del piloto para que DaisyUI quede limitado a las cards de `ClubPhonebookView`

## 2. Refactor de las fichas de contacto

- [x] 2.1 Ajustar el markup de los items de `ClubPhonebookView` para que cada registro se renderice como una card independiente basada en DaisyUI
- [x] 2.2 Mantener el nombre del contacto como cabecera principal de la ficha sin alterar su jerarquía actual
- [x] 2.3 Diseñar debajo del nombre una presentación más elaborada para teléfonos y email, evitando el aspecto de texto corrido
- [x] 2.4 Reubicar dirección y observaciones como contenido secundario sin perder datos ni enlaces operativos

## 3. Convivencia con estilos actuales

- [x] 3.1 Ajustar el SCSS residual del módulo `Mi Club` para evitar conflictos con el piloto DaisyUI y reforzar el borde sencillo y la separación entre fichas
- [x] 3.2 Verificar que breadcrumb, estados de carga/error y maquetación externa de la página no cambian de comportamiento

## 4. Verificación del piloto

- [ ] 4.1 Verificar visualmente que cada contacto se percibe como una card independiente con borde claro y separación suficiente
- [ ] 4.2 Verificar que nombre, teléfonos y email se leen con una composición más cuidada que la actual y sin perder dirección/observaciones
- [x] 4.3 Ejecutar build/pruebas relevantes y registrar evidencia antes de decidir si DaisyUI merece ampliación o descarte
