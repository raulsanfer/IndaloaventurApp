## ADDED Requirements

### Requirement: Navegación a página Mi Cuenta desde el menú inferior
El sistema MUST ofrecer acceso a la página "Mi cuenta" desde el botón inferior "Mi cuenta" del shell autenticado.

#### Scenario: Apertura de Mi Cuenta desde home
- **WHEN** el usuario autenticado pulsa el botón inferior "Mi cuenta"
- **THEN** el sistema MUST navegar a la ruta de la página "Mi cuenta"

#### Scenario: Estado activo en menú inferior
- **WHEN** el usuario se encuentra en la página "Mi cuenta"
- **THEN** el sistema MUST mostrar el botón inferior "Mi cuenta" en estado visual activo

### Requirement: Renderizado de layout Mi Cuenta según diseño de referencia
El sistema MUST renderizar la pantalla "Mi cuenta" usando los componentes y jerarquía visual definidos en `openspec/design/mi_cuenta`.

#### Scenario: Visualización estructural de Mi Cuenta
- **WHEN** la página "Mi cuenta" se renderiza
- **THEN** el sistema MUST mostrar cabecera, bloque de miembro, tarjetas de métricas, listas de enlaces/acciones y módulo de puntos con estructura coherente al diseño

### Requirement: Carga de datos del miembro desde FichaSocio
El sistema MUST obtener la información del miembro para "Mi cuenta" consumiendo `GET /api/fichas-socio/me` mediante un servicio desacoplado de la UI.

#### Scenario: Recuperación de datos de perfil
- **WHEN** un usuario autenticado entra en "Mi cuenta"
- **THEN** el sistema MUST solicitar la ficha del miembro al endpoint `/api/fichas-socio/me` y mostrar los datos recuperados

### Requirement: Visibilidad condicional del componente Cargo
El sistema MUST mostrar el componente de Cargo solo cuando la ficha del miembro indique que tiene cargo; si no tiene cargo, ese bloque MUST ocultarse.

#### Scenario: Miembro con cargo
- **WHEN** la ficha del miembro contiene un cargo válido
- **THEN** el sistema MUST renderizar el componente visual de Cargo

#### Scenario: Miembro sin cargo
- **WHEN** la ficha del miembro no contiene cargo
- **THEN** el sistema MUST no renderizar el componente de Cargo

### Requirement: Enlaces de diseño en Mi Cuenta
El sistema MUST montar todos los enlaces previstos en el diseño de "Mi cuenta", incluyendo el acceso visual a "Ficha Socio" como funcionalidad diferida.

#### Scenario: Presencia de enlaces de cuenta
- **WHEN** se visualiza la página "Mi cuenta"
- **THEN** el sistema MUST mostrar `Mis Inscripciones`, `Mis Rutas Favoritas`, `Configuración`, `Ayuda`, `Cerrar Sesión`, `Ver Tienda` y marcar "Ficha Socio" como acceso no operativo en esta iteración

### Requirement: Componente de puntos y CTA visual
El sistema MUST incluir el bloque visual de programa de puntos y su CTA conforme al diseño de "Mi cuenta", aunque la acción final pueda quedar diferida.

#### Scenario: Render de bloque de puntos
- **WHEN** el usuario visualiza la parte inferior de "Mi cuenta"
- **THEN** el sistema MUST mostrar el bloque de puntos con texto de saldo y acción `Ver Tienda`

### Requirement: Localización y estilos globales para Mi Cuenta
El sistema MUST obtener los literales visibles de "Mi cuenta" desde recursos localizados ES y MUST definir sus estilos en SCSS global modular.

#### Scenario: Revisión de textos y estilos
- **WHEN** se revisa la implementación de la página "Mi cuenta"
- **THEN** el sistema MUST usar claves de recursos para textos y parciales SCSS globales para estilos
