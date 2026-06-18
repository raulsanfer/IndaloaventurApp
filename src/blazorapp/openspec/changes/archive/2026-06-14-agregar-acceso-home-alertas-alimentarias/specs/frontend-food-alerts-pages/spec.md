## ADDED Requirements

### Requirement: HomeDashboard MUST ofrecer acceso a Alertas Alimentarias sustituyendo la tarjeta de Actividades
El sistema MUST mostrar en `HomeDashboard` un acceso a `Alertas Alimentarias` en el lugar conceptual ocupado por la tarjeta actual de `Actividades` y MUST usar ese acceso como punto de entrada desde Home a la funcionalidad de alertas alimentarias.

#### Scenario: Acceso visible desde Home
- **WHEN** el usuario visualiza `HomeDashboard`
- **THEN** el sistema MUST mostrar un acceso a `Alertas Alimentarias` en sustitución del contenido asociado a `home_card_activities_title`
- **AND** el sistema MUST no mantener `Actividades` como título visible de ese acceso

#### Scenario: Navegación desde Home a Alertas Alimentarias
- **WHEN** el usuario pulsa el acceso de `Alertas Alimentarias` en `HomeDashboard`
- **THEN** el sistema MUST navegar a la página principal de `Alertas Alimentarias`

### Requirement: El acceso de Home MUST mostrar icono a la izquierda y titulo a la derecha
El sistema MUST representar el acceso de `Alertas Alimentarias` en `HomeDashboard` con el icono `bi bi-basket2` situado a la izquierda y el título situado a la derecha dentro de la misma composición visual.

#### Scenario: Composición visual del acceso en Home
- **WHEN** el acceso de `Alertas Alimentarias` se renderiza en `HomeDashboard`
- **THEN** el sistema MUST mostrar el icono `bi bi-basket2` a la izquierda
- **AND** el sistema MUST mostrar el título de `Alertas Alimentarias` a la derecha
