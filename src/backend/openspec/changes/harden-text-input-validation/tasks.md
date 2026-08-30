## 1. Politica comun de endurecimiento textual

- [ ] 1.1 Inventariar los campos de texto de entrada de la API y asignar a cada uno una politica (`TextoLibre`, `EtiquetaCorta` o `FiltroBusqueda`) con sus limites y caracteres permitidos.
- [ ] 1.2 Implementar la abstraccion comun de normalizacion y validacion textual reutilizable en Application/Domain.
- [ ] 1.3 Documentar y aplicar la regla de acceso SQL parametrizado para cualquier flujo que consuma texto del cliente.

## 2. Aplicacion obligatoria en signals

- [ ] 2.1 Endurecer la creacion de comentarios de `Signal` en request validation, handler y dominio para normalizar texto y rechazar contenido no permitido.
- [ ] 2.2 Endurecer alta y edicion de `Signal` para `Titulo`, `Descripcion` y `Tags` reutilizando la politica comun.
- [ ] 2.3 Endurecer `signal-search` para normalizar y validar `Tags` y `Descripcion` antes de consultar persistencia.

## 3. Extension al resto de la aplicacion

- [ ] 3.1 Aplicar la politica comun a `FichaSocio` y agenda telefonica en los campos de texto libre y etiquetas cortas priorizados por el diseno.
- [ ] 3.2 Aplicar la politica comun a `Cargo`, `SignalType`, administracion de usuarios, autenticacion y filtros textuales restantes que hoy no tienen validacion homogena.
- [ ] 3.3 Revisar los puntos pendientes no cubiertos por este cambio y dejar cerrada la brecha restante en OpenSpec o en tareas tecnicas explicitamente enlazadas.

## 4. Pruebas y validacion

- [ ] 4.1 Anadir pruebas unitarias para la politica comun de texto y para las invariantes de dominio afectadas.
- [ ] 4.2 Ampliar pruebas de integracion de `signals` con casos de espacios de borde, caracteres de control, longitudes maximas y cadenas tipicas de ataque.
- [ ] 4.3 Ejecutar la suite automatizada relevante y actualizar el estado de las tareas solo cuando las validaciones pasen.
