# IndaloAventurApp

## a. Descripción general del proyecto

IndaloAventurApp es una aplicación web para la gestión y comunicación del club IndaloAventura que nace en un primer momento como TFM del Master de Desarrollo con IA de BigSchool. Ha sido realizada desde su concepción con IA y SDD bajo mi supervisión.
La app permite centralizar la información de sus socios y ofrece un espacio digital para consultar recursos del club, gestionar datos personales y participar en la comunicación relacionada con las actividades de montaña.

La solución está formada por un frontend web y una API backend independientes. El frontend proporciona la experiencia de usuario y consume los servicios de la API, mientras que el backend gestiona la autenticación, la lógica de negocio, la persistencia y las integraciones externas.

El despliegue se ha automatizado sobre el mismo hosting compartido de la web principal, configurando Github Actions para front y back que lanzan la publicación por FTP al hosting. 

Información detallada sobre la aplicación se puede encontrar en la carpeta blazorapp/docs/index.md mediante DocFx 

## b. Stack tecnológico utilizado

- **.NET 9** como plataforma común.
- **Blazor WebAssembly** para la interfaz web interactiva.
- **ASP.NET Core** como host del frontend y como framework de la API REST.
- **C# y Razor** para la implementación de la aplicación.
- **Entity Framework Core** y **SQL Server** para la persistencia de datos.
- **ASP.NET Core Identity** para la gestión de usuarios.
- **JWT Bearer** para la autenticación de las peticiones a la API.
- **Swagger/OpenAPI** para describir y consultar la API.
- **Dapper** para determinadas consultas de infraestructura.
- **Google APIs** para la autenticación social.
- **xUnit y bUnit** para las pruebas automatizadas del backend y del frontend.

## c. Información sobre su instalación y ejecución

La aplicación está publicada en producción y se puede probar desde la siguiente dirección:

**[https://app.indaloaventura.com](https://app.indaloaventura.com)**

Para utilizarla, basta con acceder a la URL desde un navegador compatible. Desde la aplicación se puede iniciar sesión, registrarse o recuperar la contraseña, según las opciones disponibles para cada usuario.

Este repositorio documenta el código fuente y la arquitectura del proyecto; la instalación y ejecución descritas para su uso corresponden al entorno de producción publicado en la dirección anterior.

## d. Estructura del proyecto

```text
src/
├── backend/
│   ├── IndaloAventurApi.sln
│   ├── src/
│   │   ├── IndaloAventurApi.Api/            # Entrada HTTP, controllers y configuración de la API
│   │   ├── IndaloAventurApi.Application/    # Casos de uso, comandos, consultas y contratos
│   │   ├── IndaloAventurApi.Domain/         # Entidades y reglas del dominio
│   │   └── IndaloAventurApi.Infrastructure/ # Persistencia e integraciones externas
│   └── tests/
│       ├── IndaloAventurApi.Application.Tests/
│       ├── IndaloAventurApi.Architecture.Tests/
│       ├── IndaloAventurApi.Domain.Tests/
│       └── IndaloAventurApi.IntegrationTests/
│
└── blazorapp/
    ├── IndaloaventurApp.sln
    ├── IndaloaventurApp.Web/
    │   ├── IndaloaventurApp.Web/             # Host ASP.NET Core de la aplicación Blazor
    │   └── IndaloaventurApp.Web.Client/     # Cliente Blazor WebAssembly
    ├── IndaloaventurApp.SharedUI/           # Componentes, modelos y servicios compartidos
    ├── IndaloaventurApp.Frontend.Tests/     # Pruebas del frontend
    └── docs/                                # Documentación técnica
```

## e. Funcionalidades principales

- Registro, inicio de sesión, cierre de sesión y recuperación de contraseña.
- Autenticación social mediante Google.
- Gestión del perfil y de la ficha del socio.
- Consulta de la información del club y agenda de teléfonos de interés.
- Consulta y solicitud de licencias federativas.
- Publicación, consulta, edición y comentarios de señales de montaña.
- Gestión de categorías y fotografías asociadas a las señales.
- Consulta de alertas alimentarias, esta opción realiza una consulta a un API externa que devuelve las ultimas alertas.
- Área de administración para gestionar usuarios, cargos, categorías y licencias federativas.
- Navegación adaptada a dispositivos móviles y posibilidad de instalar la aplicación como PWA.
- API documentada mediante Swagger/OpenAPI y separación por capas siguiendo una arquitectura limpia.

### Seguridad y revisión OWASP

La aplicación se ha revisado tomando como referencia el [OWASP Top 10:2025](https://top10.owasp.org/2025/) y el [OWASP Application Security Verification Standard (ASVS)](https://owasp.org/projects/asvs). El fichero [`src/blazorapp/security.md`](src/blazorapp/security.md) mantiene la línea base de seguridad del repositorio.

La revisión del código y de las pruebas existentes permite confirmar una alineación clara con los siguientes controles:

- **A01:2025 – Broken Access Control:** autorización en servidor mediante políticas para usuarios autenticados, socios y administradores; matriz de autorización de endpoints; comprobaciones de acceso a recursos propios; y pruebas de solicitudes anónimas, roles insuficientes, acceso de terceros y tokens caducados.
- **A02:2025 – Security Misconfiguration:** redirección a HTTPS, HSTS en el frontend, Swagger habilitado únicamente en desarrollo, CORS limitado al origen de producción y respuestas de error centralizadas. La configuración de producción mantiene las credenciales y claves sensibles fuera de los valores versionados.
- **A03:2025 – Software Supply Chain Failures:** dependencias con versiones fijadas en los proyectos .NET y preferencia por componentes oficiales del ecosistema Microsoft. La auditoría continua de vulnerabilidades de dependencias y de la infraestructura de despliegue debe realizarse en el pipeline y en el entorno de producción.
- **A04:2025 – Cryptographic Failures:** uso de ASP.NET Core Identity y Data Protection para las operaciones sensibles, tokens JWT firmados y validados mediante emisor, audiencia, expiración y clave de firma, además de comunicaciones HTTPS.
- **A05:2025 – Injection:** validación de entradas en el servidor con FluentValidation, acceso mediante Entity Framework Core y consultas Dapper parametrizadas. El contenido HTML procedente de WordPress debe considerarse contenido confiable del sistema externo o someterse a sanitización antes de renderizarse.
- **A06:2025 – Insecure Design:** reglas de negocio y permisos aplicados en backend, reautenticación reciente para cambios administrativos de contraseña, limitación de intentos en operaciones sensibles y separación entre usuarios, socios y administradores.
- **A07:2025 – Authentication Failures:** política de contraseñas, bloqueo temporal tras intentos fallidos, expiración de tokens, invalidación mediante usuario activo y `security stamp`, recuperación de contraseña y autenticación social mediante Google.
- **A08:2025 – Software or Data Integrity Failures:** validación de tamaño de imágenes, almacenamiento con nombres internos no controlados por el usuario, operaciones con cancelación y pruebas de integridad de flujos de persistencia. La comprobación del tipo real de archivo y otros controles del entorno de despliegue deben mantenerse en la revisión operativa.
- **A09:2025 – Security Logging and Alerting Failures:** registro de errores y de acciones sensibles, incluidos rechazos y limitaciones de cambios administrativos de contraseña, sin devolver excepciones internas al usuario. El sistema de alertas y monitorización de producción requiere una validación específica de la plataforma de despliegue.
- **A10:2025 – Mishandling of Exceptional Conditions:** manejador global de excepciones, respuestas `ProblemDetails`, códigos HTTP coherentes y mensajes públicos que no exponen trazas ni secretos.

Esta revisión confirma controles implementados y una alineación con OWASP.