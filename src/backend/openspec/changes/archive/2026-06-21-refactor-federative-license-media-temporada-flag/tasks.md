## 1. Modelo y persistencia

- [x] 1.1 Refactorizar `TarifaLicenciaFederativa` para sustituir `PrecioClubMediaTemporada` por `MediaTemporada`, manteniendo `false` como valor por defecto y actualizando sus pruebas de dominio.
- [x] 1.2 Ajustar la configuracion EF Core y crear una migracion que agregue `MediaTemporada`, duplique las tarifas existentes para conservar los precios de media temporada, elimine la columna antigua y actualice el indice unico.
- [x] 1.3 Actualizar los datos semilla y fixtures de tarifas federativas para representar temporada completa y media temporada como filas distintas.

## 2. Contratos y consultas

- [x] 2.1 Actualizar los DTOs y mappings de tarifas para exponer `MediaTemporada` y eliminar dependencias de `PrecioClubMediaTemporada`.
- [x] 2.2 Adaptar `GetTarifasLicenciasFederativas` para filtrar por `MediaTemporada` como variante de tarifa y devolver las filas del catalogo con orden determinista.
- [x] 2.3 Ajustar las consultas de solicitudes propias y administrativas para incluir `MediaTemporada` en la informacion funcional de la tarifa asociada.

## 3. Verificacion

- [x] 3.1 Actualizar o crear pruebas unitarias e integracion que validen la migracion logica del catalogo y la nueva unicidad por `MediaTemporada`.
- [x] 3.2 Actualizar o crear pruebas de aplicacion para comprobar que el catalogo, el autoservicio y las consultas administrativas devuelven `MediaTemporada` correctamente.
- [x] 3.3 Ejecutar la bateria de pruebas relevante del modulo de licencias federativas y dejar evidencia de verificacion antes de cerrar el cambio.
