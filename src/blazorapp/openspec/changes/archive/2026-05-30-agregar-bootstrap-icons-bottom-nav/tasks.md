## 1. Integracion de Bootstrap Icons

- [x] 1.1 Agregar `Bootstrap Icons` al frontend y registrarlo para disponibilidad global en la aplicación web
- [x] 1.2 Verificar que la librería pueda consumirse desde componentes Razor mediante clases o identificadores del set

## 2. Refactor de BottomNav

- [x] 2.1 Actualizar el modelo de `BottomNav` para separar icono y label en cada item de navegación
- [x] 2.2 Sustituir en `BottomNav.razor` las abreviaturas actuales por renderizado de icono + texto localizado
- [x] 2.3 Ajustar estilos globales de la botonera para soportar composición icono/texto y estado activo coherente

## 3. Seleccion y validacion de iconos

- [x] 3.1 Intentar localizar en `Bootstrap Icons` equivalentes razonables para `Home`, `Mi Club` y `Mi Cuenta` según la referencia adjunta
- [x] 3.2 Aplicar los iconos localizados a los tres items de la `BottomNav`
- [x] 3.3 Si algún icono no tiene equivalencia suficientemente fiel, documentar el faltante y avisar explícitamente para aportación manual del recurso

## 4. Verificacion

- [ ] 4.1 Verificar visualmente que `BottomNav` mantiene legibilidad y estado activo tras introducir iconos
- [ ] 4.2 Comparar el resultado con la referencia adjunta y registrar cualquier desviación relevante
- [x] 4.3 Ejecutar build/pruebas relevantes del frontend y registrar la evidencia antes de marcar el cambio como completado
