## Context

El sistema ya sigue una arquitectura en capas con agregados de dominio, repositorios y persistencia EF Core sobre SQL Server. La nueva capacidad introduce dos conceptos nuevos relacionados entre si: un catalogo de tarifas federativas por temporada derivado del Excel oficial de la federacion, y una solicitud de licencia federativa que un usuario registra para una temporada concreta con estado de negocio.

El fichero `docs/Tarifas_Federacion_2026_Estructurado.xlsx` contiene una sola hoja con 25 filas de tarifas y las columnas `Licencia`, `Categoria`, `Clubes (EUR)`, `Independientes (EUR)` y `Territorio`. La implementacion debe convertir esta informacion en un modelo persistido del sistema, evitando depender del Excel como almacenamiento operativo.

## Goals / Non-Goals

**Goals:**
- Modelar en dominio el catalogo de tarifas federativas por temporada, usando `Temporada` como dato de vigencia anual.
- Modelar la solicitud de licencia federativa por usuario y temporada con estados `Pendiente`, `Confirmada` y `Cancelada`.
- Garantizar que las solicitudes antiguas sigan consultables como historico aunque existan nuevas tarifas o nuevas vigencias anuales.
- Persistir ambos modelos con EF Core y dejar preparada la migracion de base de datos.
- Definir reglas estructurales que permitan exponer endpoints CQRS en una fase posterior sin redisenar tablas ni invariantes.

**Non-Goals:**
- Exponer endpoints HTTP en esta fase.
- Implementar flujos administrativos completos de confirmacion/cancelacion.
- Automatizar una importacion generica de cualquier Excel subido por usuarios.
- Resolver reglas funcionales futuras como pagos, renovaciones o documentos adjuntos.

## Decisions

### 1. Separar catalogo y solicitudes
Se definiran dos modelos persistidos:
- `TarifaLicenciaFederativa` como catalogo de referencia por temporada.
- `SolicitudLicenciaFederativa` como agregado raiz de negocio asociado a un `UserId`.

Rationale: el Excel describe datos maestros, mientras que la solicitud representa una accion del usuario con ciclo de vida propio. Separarlos evita duplicar tarifas en cada solicitud y permite evolucionar el flujo operativo despues.

Alternativas consideradas:
- Guardar todo en una sola tabla de solicitudes con columnas denormalizadas. Se descarta porque mezcla datos maestros y transaccionales.
- Mantener el Excel como fuente leida bajo demanda. Se descarta por fragilidad operativa y por no ser consistente con la persistencia del resto del sistema.

### 2. Tratar el Excel 2026 como semilla de datos controlada
La carga inicial de tarifas se realizara como datos semilla o migracion controlada transcribiendo las 25 filas del Excel a registros persistidos para la temporada `2026`.

Rationale: el proyecto no necesita una dependencia de lectura de Excel en produccion para esta primera fase, y un seed controlado ofrece trazabilidad, despliegue determinista y facilidad de test.

Alternativas consideradas:
- Leer el archivo `.xlsx` en el arranque. Se descarta por complejidad operativa y dependencia innecesaria del sistema de archivos.
- Crear una utilidad de importacion ad hoc como paso manual. Se pospone hasta que exista una necesidad real de gestionar nuevos ficheros por administracion.

### 3. Mantener historico de solicitudes por usuario
Las solicitudes no se sobrescribiran ni se eliminaran al cargar nuevas tarifas de otros anios de vigencia; permaneceran vinculadas a la tarifa concreta con la que fueron creadas.

Rationale: el usuario necesita historial y la base tarifaria puede cambiar cada anio. Conservar la referencia a la tarifa vigente en el momento de la solicitud evita reinterpretar historicos con datos nuevos.

Alternativas consideradas:
- Recalcular solicitudes antiguas contra la tarifa vigente actual. Se descarta porque rompe la trazabilidad historica.
- Copiar todos los datos de tarifa dentro de la solicitud y prescindir del catalogo. Se descarta en esta fase por duplicacion innecesaria.

### 4. Unicidad de solicitud por usuario y temporada
`SolicitudLicenciaFederativa` tendra una restriccion unica por `UserId` y `Temporada`.

Rationale: el requisito funcional habla de una licencia por temporada solicitada por el usuario, en singular. Esta restriccion evita duplicados y simplifica la futura API.

Alternativas consideradas:
- Permitir varias solicitudes por temporada y resolver la vigente por estado. Se descarta porque introduce complejidad no pedida.

### 5. Referencia de solicitud a tarifa catalogada de la misma temporada
Cada solicitud almacenara `TarifaLicenciaFederativaId` y `Temporada`, y el flujo de creacion debera validar que la tarifa referenciada pertenece a la misma temporada.

Rationale: la seleccion de licencia debe apoyarse en el catalogo oficial cargado desde el Excel y no en texto libre.

Alternativas consideradas:
- Guardar solamente `Licencia` y `Categoria` como cadenas. Se descarta por falta de integridad referencial.

### 6. Estado de negocio con enum explicito
El estado de la solicitud se representara mediante un enum persistido con los valores `Pendiente`, `Confirmada` y `Cancelada`. Las nuevas solicitudes nacen en `Pendiente`.

Rationale: hace visibles las reglas del dominio y evita estados textuales inconsistentes.

Alternativas consideradas:
- Persistir el estado como string libre. Se descarta por menor seguridad y mayor riesgo de datos invalidos.

## Risks / Trade-offs

- [Cambios futuros en tarifas o temporadas] -> Mitigacion: modelar el catalogo por temporada y cargar 2026 como primer conjunto independiente.
- [Ambiguedad sobre que precio aplicar entre club e independiente] -> Mitigacion: persistir ambos importes en el catalogo y dejar la regla de seleccion como decision explicita de una fase posterior.
- [Necesidad futura de historico inmutable si se editan tarifas] -> Mitigacion: tratar el catalogo cargado desde Excel como referencia estable por temporada; si mas adelante se permiten cambios, se podra anadir snapshot en la solicitud.
- [Dependencia de datos manualmente transcritos desde el Excel] -> Mitigacion: verificar en tests o seeds el recuento esperado de 25 filas y algunos registros representativos.

## Migration Plan

1. Crear tablas nuevas para catalogo de tarifas y solicitudes de licencia federativa.
2. Aplicar indices y restricciones unicas:
   - Catalogo: unicidad por `Temporada`, `Licencia`, `Categoria`.
   - Solicitudes: unicidad por `UserId`, `Temporada`.
3. Insertar los 25 registros del Excel 2026 como seed o en la propia migracion.
4. Desplegar la migracion antes de habilitar endpoints futuros.
5. En rollback, eliminar los objetos nuevos de la base si el cambio no se ha usado todavia; si ya existen solicitudes, tratar el rollback como restauracion de backup por incluir datos de negocio nuevos.

## Open Questions

- La futura creacion de solicitudes, ?debe determinar el importe aplicable a partir de `IsMember` del usuario o permitir seleccion/override administrativa?
- La categoria (`Mayores`, `Juveniles`, `Infantiles`) ?debe derivarse automaticamente de la fecha de nacimiento de `FichaSocio` o seleccionarse de forma explicita?
- El catalogo 2026, ?debe quedar totalmente bloqueado a edicion manual o se preve un mantenimiento administrativo posterior?
