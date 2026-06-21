## ADDED Requirements

### Requirement: Signal image storage MUST be configurable by filesystem root
El sistema MUST leer desde configuracion una ruta raiz de almacenamiento para las imagenes de `Signal` y SHALL usarla como ubicacion base de escritura y lectura en cada entorno.

#### Scenario: Configuracion valida de ruta de almacenamiento
- **WHEN** la aplicacion inicia con una ruta de almacenamiento de imagenes valida y accesible
- **THEN** el backend SHALL poder resolver el servicio de almacenamiento sin errores de configuracion

#### Scenario: Configuracion ausente o invalida
- **WHEN** la aplicacion inicia sin ruta configurada o con una ruta no valida para almacenamiento de imagenes
- **THEN** el sistema SHALL fallar de forma controlada durante el arranque

### Requirement: Signal image storage MUST manage file lifecycle safely
El sistema MUST guardar, leer, reemplazar y borrar las imagenes de `Signal` en filesystem de forma segura, evitando colisiones de nombre y referencias huerfanas.

#### Scenario: Escritura inicial de imagenes
- **WHEN** se crea una `Signal` con fotos validas
- **THEN** el sistema SHALL persistir los ficheros fisicos dentro de la ruta configurada y SHALL devolver referencias internas estables para asociarlas a la `Signal`

#### Scenario: Reemplazo de imagenes existentes
- **WHEN** una `Signal` existente reemplaza una o ambas fotos
- **THEN** el sistema SHALL actualizar las referencias persistidas y SHALL limpiar los ficheros anteriores que hayan quedado obsoletos

#### Scenario: Eliminacion de ficheros huérfanos ante fallo
- **WHEN** una operacion de almacenamiento de imagenes falla a mitad del proceso
- **THEN** el sistema SHALL ejecutar la limpieza compensatoria necesaria para no dejar residuos inconsistentes en filesystem
