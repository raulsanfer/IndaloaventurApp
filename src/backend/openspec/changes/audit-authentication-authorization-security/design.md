## Context

El backend ya protege la mayor parte de sus endpoints con JWT e Identity, pero el control de acceso esta distribuido entre varias capas: atributos `[Authorize]` con cadenas de rol, comprobaciones de propiedad en handlers, el claim `IsMember` para algunos flujos de licencias, y pruebas de integracion escritas por modulo. Durante la revision actual aparecen dos problemas estructurales:

- No existe una matriz unica y verificable que diga que tipo de acceso requiere cada endpoint de la API.
- La aplicacion distingue entre rol tecnico `Member` e indicador funcional `IsMember`, y esa diferencia es facil de aplicar mal cuando se añade o modifica un endpoint.

Ademas, el login por credenciales valida password pero no refleja una politica explicita de endurecimiento ante intentos repetidos, y la proteccion de autenticacion/autorizacion no se comprueba hoy con un enfoque uniforme para toda la superficie HTTP.

## Goals / Non-Goals

**Goals:**
- Definir una fuente de verdad versionada para la clasificacion de acceso de cada endpoint HTTP.
- Endurecer los flujos de autenticacion para reducir riesgo de uso indebido por intentos repetidos o tokens ya no validos.
- Centralizar y hacer mas expresivas las reglas de autorizacion para distinguir autenticacion basica, rol administrativo, propiedad del recurso y condicion de socio.
- Aumentar la cobertura automatizada con pruebas de acceso negativas y positivas por modulo.

**Non-Goals:**
- Realizar un pentest externo o una auditoria de infraestructura fuera del codigo del backend.
- Redisenar el dominio funcional de roles del club mas alla de aclarar y hacer cumplir las reglas ya necesarias.
- Cambiar el modelo de JWT a refresh tokens, sesiones servidor o proveedores externos distintos de los ya integrados.

## Decisions

1. Introducir una matriz de acceso de API mantenida junto al codigo y validada por pruebas.
- Rationale: la autorizacion actual es correcta en varios puntos, pero esta repartida y no es auditable de un vistazo. Una matriz versionada permite revisar altas/bajas de endpoints y obliga a decidir de forma explicita si una ruta es publica, autenticada, `Admin`, `IsMember` o `OwnerOrAdmin`.
- Alternative considered: depender solo de lectura manual de controladores y specs de modulo. Se descarta porque no detecta bien regresiones ni inconsistencias transversales.

2. Convertir reglas frecuentes de autorizacion en politicas o helpers centralizados.
- Rationale: repetir cadenas como `Roles = "Admin"` o pasar booleanos `isAdmin` desde controladores funciona, pero dispersa reglas y aumenta el riesgo de errores al introducir nuevos casos. La implementacion debera converger hacia politicas o abstracciones equivalentes para `Admin`, usuario autenticado, aislamiento propietario/admin y operaciones ligadas a `IsMember`.
- Alternative considered: mantener el modelo actual solo con convenciones y revision manual. Se descarta por fragilidad y baja trazabilidad.

3. Endurecer login por credenciales con mecanismos nativos de Identity y validaciones JWT explicitas.
- Rationale: el backend ya usa ASP.NET Identity y es el lugar correcto para aplicar contadores de fallo, lockout temporal y reseteo del contador en autenticacion correcta. En paralelo, la configuracion JWT debe dejar explicitamente cubiertas firma, emisor, audiencia, caducidad y revalidacion del estado activo del usuario.
- Alternative considered: implementar protecciones ad hoc fuera de Identity. Se descarta por duplicar capacidades y elevar el riesgo de errores de seguridad.

4. Verificar la autorizacion desde la perspectiva del consumidor HTTP, no solo desde handlers individuales.
- Rationale: el riesgo real de fuga de informacion aparece en el comportamiento observable de rutas y middleware. Las pruebas de integracion deben cubrir acceso anonimo, rol insuficiente, acceso cruzado a recursos de terceros y casos donde `IsMember` manda sobre el rol tecnico.
- Alternative considered: conformarse con pruebas unitarias de handlers. Se descarta porque no cubren atributos de controlador, middleware JWT ni composicion completa del pipeline.

## Risks / Trade-offs

- [Riesgo] La matriz de acceso puede quedarse desactualizada si no se ata a pruebas y revisiones de cambio. -> Mitigacion: hacer que los nuevos endpoints deban aparecer en la matriz y añadir tests de regresion asociados.
- [Riesgo] Endurecer login con lockout temporal puede aumentar soporte funcional por bloqueos legitimos. -> Mitigacion: usar umbrales razonables, mensajes neutros y pruebas claras de reactivacion del acceso.
- [Riesgo] Al revisar acceso modulo por modulo pueden aparecer discrepancias entre la intencion de negocio y el comportamiento actual. -> Mitigacion: tratar esas discrepancias como decisiones explicitas en specs y no como cambios accidentales de codigo.
- [Trade-off] Centralizar politicas añade algo de infraestructura y refactor, pero reduce deuda de seguridad y facilita auditoria futura.

## Open Questions

- Que endpoints autenticados deben seguir siendo accesibles para cualquier usuario registrado y cuales deben pasar a exigir `IsMember = true` por sensibilidad funcional o por exponer informacion privada del club.
- Si el endurecimiento de autenticacion debe limitarse a lockout por usuario en login clasico o extenderse tambien con rate limiting explicito para endpoints anonimos como `login`, `register` y `passrecovery`.
