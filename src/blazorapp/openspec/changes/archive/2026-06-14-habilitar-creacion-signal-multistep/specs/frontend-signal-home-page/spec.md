## ADDED Requirements

### Requirement: SignalHome MUST mostrar un FAB para crear nuevas señales
El sistema MUST mostrar en `SignalHome` un botón flotante de acción en la esquina inferior derecha para iniciar la creación de una nueva señal, manteniendo una estética alineada con DaisyUI y con el color primario de la aplicación.

#### Scenario: FAB visible en SignalHome
- **WHEN** la pantalla `SignalHome` se renderiza para un usuario autenticado
- **THEN** el sistema MUST mostrar un FAB en la esquina inferior derecha
- **AND** el sistema MUST mostrar dicho FAB con fondo primario de la aplicación
- **AND** el sistema MUST mostrar un icono `+` en color blanco dentro del botón

#### Scenario: Navegación desde el FAB
- **WHEN** el usuario pulsa el FAB de creación en `SignalHome`
- **THEN** el sistema MUST navegar al flujo de creación de señales
- **AND** el sistema MUST conservar la coherencia visual del shell autenticado
