## Why

La API ya tiene autenticacion JWT, roles y varios controles de acceso, pero la seguridad efectiva esta repartida entre atributos `[Authorize]`, comprobaciones de propiedad en handlers, el claim `IsMember` y reglas funcionales dispersas en distintos modulos. En este estado es dificil demostrar que todos los endpoints protegen correctamente la informacion segun rol, titularidad o condicion de socio, y ademas siguen abiertos riesgos tipicos como endurecimiento insuficiente frente a fuerza bruta o regresiones por cambios futuros.

## What Changes

- Crear una capacidad transversal de auditoria y verificacion de autorizacion que obligue a mantener una matriz de acceso por endpoint y pruebas automatizadas negativas/positivas para los casos anonimo, rol insuficiente y acceso cruzado entre usuarios cuando aplique.
- Modificar la especificacion de autenticacion JWT para exigir endurecimiento explicito de validacion de tokens y de los flujos de login, incluyendo bloqueo consistente de usuarios desactivados y protecciones frente a intentos repetidos de autenticacion.
- Formalizar la distincion entre rol tecnico `Member`, rol `Admin`, propiedad del recurso y claim `IsMember` para que los endpoints sensibles documenten y prueben cual de esas condiciones habilita realmente el acceso.
- Ampliar la suite automatizada para cubrir de forma sistematica los modulos expuestos por la API que manejan datos personales o acciones administrativas.

## Capabilities

### New Capabilities
- `api-authorization-security-audit`: define la matriz de acceso, los niveles de sensibilidad y la cobertura automatizada minima para verificar autenticacion, autorizacion por rol y aislamiento entre usuarios en todos los endpoints HTTP.

### Modified Capabilities
- `jwt-identity-authentication`: se endurecen los requisitos de validacion de token y de proteccion del login para reducir accesos indebidos por credenciales robadas, tokens obsoletos o intentos repetidos de autenticacion.

## Impact

- API: controladores y endpoints protegidos por autenticacion, roles, propiedad del recurso o claim `IsMember`.
- Application: handlers que hoy toman decisiones de acceso con `userId`, `isAdmin` o reglas de autoservicio.
- Infrastructure/Security: `IdentityService`, configuracion JWT/Identity y posibles politicas centralizadas de autorizacion.
- Tests: ampliacion de pruebas de integracion y aplicacion para cobertura de acceso por modulo y rutas sensibles.
- Documentacion OpenSpec: nueva capacidad transversal de seguridad y delta sobre autenticacion JWT.
