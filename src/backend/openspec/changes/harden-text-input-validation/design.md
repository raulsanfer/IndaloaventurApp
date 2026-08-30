## Context

La revision del backend muestra que los comentarios de `Signal` ya se persisten con EF Core y que las consultas Dapper inspeccionadas usan `CommandDefinition` con parametros, por lo que el riesgo actual no esta en una inyeccion SQL directa en ese flujo concreto. El problema real es la falta de una politica uniforme para texto entrante: `SignalComment` y `Signal` solo hacen `Trim()`, mientras otros modulos aplican reglas parciales e inconsistentes (`FichaSocio` trim sin value objects de texto, `Cargo` y `SignalType` solo validan no vacio, agenda telefonica ya tiene algunos value objects y limites).

La solicitud del cambio se centra en comentarios de `Signal`, pero pide ademas una solucion extensible al resto de la aplicacion. Eso obliga a definir una politica transversal y no una validacion ad hoc solo para un endpoint.

## Goals / Non-Goals

**Goals:**
- Establecer una regla comun y reutilizable para normalizar y validar entradas de texto del front.
- Mantener `Signal` y `SignalComment` como primer alcance obligatorio por ser el caso de negocio priorizado.
- Garantizar que cualquier consulta SQL que consuma texto de usuario siga un patron parametrizado y comprobable.
- Dejar inventariado el resto de puntos de entrada textual de la aplicacion para completar el endurecimiento sin soluciones divergentes.

**Non-Goals:**
- No intentar detectar "malicia" semantica mediante listas negras de palabras SQL o heuristicas fragiles.
- No sustituir el escape/encoding de salida que deba aplicar el frontend al renderizar texto plano.
- No redisenar todos los agregados del dominio en una sola iteracion si no aportan riesgo equivalente al caso principal.

## Decisions

### Decision: Definir politicas de texto por categoria funcional

Se introducira una abstraccion comun para texto entrante con varias categorias:
- `TextoLibre`: comentarios y campos descriptivos. Permitira texto plano con letras, numeros, puntuacion y saltos de linea normalizados, pero rechazara caracteres de control no permitidos.
- `EtiquetaCorta`: nombres, titulos y descripciones cortas. No permitira saltos de linea y aplicara trim, longitud maxima y rechazo de controles.
- `FiltroBusqueda`: parametros textuales usados en queries. Aplicara trim, longitud maxima, rechazo de controles y normalizacion estable antes de llegar a la capa de datos.

La normalizacion comun SHALL incluir, como minimo, trim de bordes, estabilizacion de finales de linea y rechazo de caracteres de control fuera del conjunto permitido por cada categoria.

Alternativas consideradas:
- Lista negra de palabras como `SELECT`, `DROP` o `UNION`: rechazada por producir falsos positivos y no resolver la seguridad real frente a SQL injection.
- Sanitizar eliminando caracteres arbitrariamente: rechazada porque altera silenciosamente contenido del usuario y dificulta el soporte.

### Decision: Validar en dos capas

La primera barrera estara en `FluentValidation` o en validadores de request para devolver `400` con mensajes claros. La segunda barrera estara en el dominio mediante value objects o validacion explicita en entidades/agregados para impedir que texto invalido llegue a persistencia aunque cambie el punto de entrada.

Alternativas consideradas:
- Validar solo en controladores o DTOs: rechazada porque no protege usos internos o futuros handlers.
- Validar solo en dominio: rechazada porque degrada la experiencia API y hace menos expresivos los errores de contrato.

### Decision: Mantener la proteccion SQL por construccion, no por escape de texto

La mitigacion principal frente a SQL injection SHALL ser el uso exclusivo de EF Core, Dapper parametrizado y `SqlCommand` con parametros cuando intervenga texto de usuario. El cambio documentara la prohibicion de concatenar o interpolar SQL con filtros o texto entrante, y anadira pruebas para proteger ese comportamiento en los flujos revisados.

Alternativas consideradas:
- Escapar comillas o limpiar simbolos "peligrosos": rechazada porque es facil romperla y no cubre todos los vectores.

### Decision: Despliegue por fases con alcance obligatorio y alcance auditado

La primera fase implementara la infraestructura comun y la aplicara a `SignalComment`, `Signal` y `signal-search`. La segunda fase auditara y adaptara los siguientes puntos:
- `FichaSocio`: `Nombre`, `Apellidos`, `Direccion`, `Poblacion`, `Provincia`, `Alergias`.
- `AgendaTelefonica`: `Nombre`, `Direccion`, `Observaciones`.
- `Cargo` y `SignalType`: `Descripcion`, `Nombre`, `Icono`.
- Administracion de usuarios y autenticacion: `Email`, filtros de usuario y otros campos textuales con contratos poco reforzados.

Esta division permite cerrar el riesgo prioritario sin perder la consistencia del diseno.

## Risks / Trade-offs

- [Reglas demasiado restrictivas] → Mitigacion: definir politicas por categoria y cubrir casos validos reales en castellano, incluidos acentos y puntuacion habitual.
- [Normalizacion visible para el usuario] → Mitigacion: limitar la normalizacion a operaciones estables y predecibles como trim y estandarizacion de finales de linea.
- [Cobertura desigual entre modulos] → Mitigacion: incluir inventario obligatorio de entradas textuales y tareas explicitas de extension.
- [Confundir validacion de entrada con proteccion XSS] → Mitigacion: documentar que el backend almacena texto plano validado y que el cliente debe seguir renderizando con escape seguro.

## Migration Plan

- Introducir la abstraccion comun de texto y aplicarla primero a `SignalComment`, `Signal` y `signal-search`.
- Ampliar pruebas unitarias e integracion con casos de texto vacio tras normalizacion, caracteres de control, longitudes maximas y cadenas tipicas de ataque.
- Revisar los accesos a datos que reciban filtros textuales para confirmar que siguen parametrizados.
- Extender la misma politica al resto de modulos priorizados sin necesidad de cambios contractuales no relacionados.

El cambio es compatible a nivel de esquema salvo que alguna entidad necesite alinear longitudes ya existentes; en principio deberia desplegarse como endurecimiento de validacion sin migracion destructiva.

## Open Questions

- Si los comentarios de `Signal` deben permitir multiples lineas o si deben limitarse a una sola linea normalizada.
- Si conviene rechazar marcado HTML explicito en comentarios y descripciones o si basta con tratarlos como texto plano y delegar el escaping al cliente.
- Si la extension al resto de modulos debe cerrarse dentro del mismo cambio o dividirse en cambios posteriores una vez completado el inventario.
