## 1. Dominio y persistencia

- [x] 1.1 Añadir `Email` y `Direccion` a `FichaContacto` con reglas de actualización y creación coherentes.
- [x] 1.2 Ajustar el mapeo de persistencia y crear migración para almacenar ambos campos.

## 2. Aplicación y API

- [x] 2.1 Actualizar comandos, queries, DTOs y controladores de agenda telefónica para aceptar y devolver `Email` y `Direccion`.
- [x] 2.2 Añadir o ajustar validaciones de entrada para los nuevos campos.

## 3. Verificación

- [x] 3.1 Añadir o ajustar pruebas de dominio y aplicación para creación, actualización y lectura con `Email` y `Direccion`.
- [x] 3.2 Añadir o ajustar pruebas de integración del endpoint de agenda telefónica.
- [x] 3.3 Ejecutar `dotnet test` y corregir regresiones.
