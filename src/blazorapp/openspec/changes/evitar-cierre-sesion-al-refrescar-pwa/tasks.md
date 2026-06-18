## 1. Persistencia de sesi�n

- [x] 1.1 Extender el modelo/servicio de sesi�n para representar una instant�nea persistible con expiraci�n absoluta y metadatos suficientes para reconstruir `AuthSession`.
- [x] 1.2 Implementar el acceso a `sessionStorage` y `localStorage` desde el cliente para guardar la sesi�n seg�n el valor de `RememberMe`.
- [x] 1.3 Adaptar los flujos de login existentes para persistir la sesi�n restaurable adem�s de la sesi�n en memoria.

## 2. Rehidrataci�n y guardas de navegaci�n

- [x] 2.1 A�adir una fase de inicializaci�n de sesi�n en el arranque interactivo de la app que restaure una sesi�n persistida si sigue vigente.
- [x] 2.2 Ajustar la protecci�n de rutas/p�ginas para no redirigir al login hasta que la comprobaci�n de restauraci�n haya finalizado.
- [x] 2.3 Limpiar autom�ticamente las sesiones persistidas expiradas y confirmar que las rutas protegidas vuelven al login cuando no exista una sesi�n reutilizable.

## 3. Cierre de sesi�n y UX asociada

- [x] 3.1 Adaptar `SignOut` para borrar tanto la sesi�n en memoria como cualquier sesi�n persistida en navegador.
- [x] 3.2 Revisar el comportamiento de `Recordarme` y del login social para que la persistencia resultante sea coherente con la UX definida en la spec.

## 4. Verificaci�n

- [x] 4.1 A�adir o actualizar tests de cliente para cubrir persistencia tras login, rehidrataci�n en refresh y limpieza al cerrar sesi�n.
- [ ] 4.2 Verificar manualmente en navegador que `F5` y el refresco por gesto en m�vil/PWA no env�an al login mientras el token siga vigente.
- [x] 4.3 Verificar manualmente o con tests que una sesi�n expirada o ausente sigue redirigiendo correctamente al login tras finalizar la inicializaci�n.
