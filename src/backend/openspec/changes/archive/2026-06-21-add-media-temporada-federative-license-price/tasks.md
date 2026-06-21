## 1. Modelo y persistencia

- [x] 1.1 Ampliar `TarifaLicenciaFederativa` para incluir `PrecioClubMediaTemporada` con sus validaciones de dominio y actualizar las pruebas unitarias asociadas.
- [x] 1.2 Actualizar la configuracion EF Core, la migracion y el snapshot para agregar la nueva columna en `TarifasLicenciasFederativas`.
- [x] 1.3 Completar o ajustar los datos semilla del catalogo federativo para persistir `PrecioClubMediaTemporada` en todas las tarifas existentes.

## 2. Contratos y consultas de licencias

- [x] 2.1 Ampliar los DTOs y mappings de tarifas y solicitudes para exponer `PrecioClubMediaTemporada` en catalogo, autoservicio y vistas administrativas.
- [x] 2.2 Extender `GetTarifasLicenciasFederativasQuery`, su handler y el controlador para aceptar `MediaTemporada` opcional y devolver `PrecioClubAplicable` segun corresponda.
- [x] 2.3 Ajustar las consultas de solicitudes del usuario y administrativas para incluir el nuevo campo de tarifa sin alterar sus filtros actuales.

## 3. Verificacion

- [x] 3.1 Actualizar o crear pruebas de aplicacion e integracion para validar que el catalogo devuelve `PrecioClubAplicable` correcto cuando `MediaTemporada` es `true` y `false`.
- [x] 3.2 Actualizar o crear pruebas de integracion para comprobar que las respuestas de solicitudes propias y administrativas incluyen `PrecioClubMediaTemporada`.
- [x] 3.3 Ejecutar la bateria de pruebas relevante del modulo de licencias federativas y dejar evidencia de verificacion antes de cerrar el cambio.
