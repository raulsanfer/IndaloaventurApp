## 1. Ajuste de lectura de ficha

- [ ] 1.1 Refactorizar `GetFichaSocioQueryHandler` para que no lance excepcion cuando la ficha no exista.
- [ ] 1.2 Ajustar contratos (`Query`/DTO/controlador) para permitir respuesta vacia de ficha.
- [ ] 1.3 Mantener comportamiento de seguridad existente para accesos no autorizados.

## 2. Pruebas

- [ ] 2.1 Agregar prueba de aplicacion para consulta de usuario sin ficha con resultado exitoso sin excepcion.
- [ ] 2.2 Ajustar/crear prueba de integracion del endpoint de "Mi Cuenta" validando carga sin ficha.

## 3. Verificacion

- [ ] 3.1 Ejecutar `dotnet test` y verificar que no hay regresiones.
- [ ] 3.2 Validar manualmente contrato HTTP final para caso sin ficha (`200` con payload nulo o `204`, segun decision final).
