## Why

La pantalla `Mi Cuenta` ya distingue entre usuarios que son socios y usuarios autenticados que todavía no lo son, pero a estos últimos no les ofrece una vía clara para iniciar el alta en el club. Añadir una acción directa de `Hazte socio` reduce fricción y conecta la app con el proceso real de captación ya disponible en la web pública.

## What Changes

- Añadir una nueva opción visible en `Mi Cuenta` para usuarios autenticados con rol `Member` cuyo claim `IsMember` sea `false`.
- Hacer que la nueva opción abra la URL `https://indaloaventura.com/hazte-socio/` fuera de la navegación interna de la app.
- Garantizar que el enlace se abra en una nueva pestaña del navegador o en un navegador externo cuando la app esté instalada como PWA.
- Mantener la coherencia visual de la nueva opción con el resto de accesos mostrados en `Mi Cuenta`.
- Asegurar que la opción no se muestre a usuarios que ya sean socios ni a perfiles administrativos donde no aplique.

## Capabilities

### New Capabilities
- `frontend-my-account-member-join-link`: Define una acción visible en `Mi Cuenta` para que usuarios `Member` no socios puedan iniciar el alta del club desde un enlace externo seguro.

### Modified Capabilities

## Impact

- Afecta a `IndaloaventurApp.SharedUI/Components/MyAccount` para ajustar la composición y visibilidad condicional de enlaces en `Mi Cuenta`.
- Afecta a recursos localizados ES para el nuevo literal de acción y, si hace falta, textos de ayuda asociados.
- Puede requerir pequeños ajustes de SCSS global en `Mi Cuenta` para mantener consistencia visual del nuevo acceso.
- No requiere cambios en API backend ni en `endpoints.json`, ya que la acción navega a una URL pública externa.
