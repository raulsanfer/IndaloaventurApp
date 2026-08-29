## ADDED Requirements

### Requirement: Aplicar una politica comun a toda entrada de texto del front
El sistema MUST clasificar cada campo textual recibido por la API en una politica de validacion adecuada a su uso funcional, como minimo `TextoLibre`, `EtiquetaCorta` o `FiltroBusqueda`. Antes de persistir datos o ejecutar consultas, el sistema SHALL normalizar espacios de borde y finales de linea segun la politica aplicable y MUST rechazar caracteres de control no permitidos, valores vacios tras normalizacion o longitudes superiores al limite definido para el campo.

#### Scenario: Texto libre valido se normaliza antes de persistirse
- **WHEN** un cliente envia un comentario o descripcion con espacios de borde y finales de linea admitidos
- **THEN** el sistema acepta la solicitud y persiste el texto ya normalizado segun la politica de `TextoLibre`

#### Scenario: Entrada con caracteres de control no permitidos es rechazada
- **WHEN** un cliente envia un campo textual con caracteres de control fuera del conjunto admitido para su politica
- **THEN** el sistema rechaza la solicitud con error de validacion y SHALL no persistir el dato

### Requirement: El acceso SQL que consuma texto de usuario MUST mantenerse parametrizado
El sistema MUST ejecutar mediante acceso parametrizado cualquier consulta o comando SQL Server que consuma texto o filtros recibidos del cliente. Ningun flujo que use texto de usuario SHALL construir SQL por concatenacion o interpolacion directa del valor entrante.

#### Scenario: Filtro textual llega a una consulta segura
- **WHEN** un endpoint ejecuta una lectura o escritura en SQL Server usando texto recibido del cliente
- **THEN** el sistema envia el valor mediante parametros del proveedor de acceso a datos y SHALL no incrustarlo en el SQL literal

### Requirement: La suite automatizada MUST cubrir regresiones de endurecimiento textual
El sistema MUST disponer de pruebas automatizadas para los endpoints y handlers endurecidos, cubriendo al menos normalizacion esperada, rechazo de texto invalido y mantenimiento del comportamiento seguro de acceso a datos cuando intervienen filtros textuales.

#### Scenario: Regresion de seguridad detectada por pruebas
- **WHEN** una modificacion futura relaja la validacion textual o introduce un acceso SQL no parametrizado en un flujo cubierto
- **THEN** la suite automatizada falla antes de considerar el cambio como valido
