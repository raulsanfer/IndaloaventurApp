# SharedUI y componentes

`IndaloaventurApp.SharedUI` contiene la UI reutilizable y las abstracciones que permiten que esa UI no dependa directamente del host web.

## Componentes

Los componentes estan agrupados por dominio:

- `Components/Club`: `ClubIndexView`, `ClubPhonebookView`, `FederativeLicensesView`.
- `Components/Login`: `LoginView`, recuperacion y reseteo de password.
- `Components/MyAccount`: cuenta, ficha de socio y cargo.
- `Components/Signals`: home, detalle y creacion de signals.
- `Components/FoodAlerts`: listados, categorias y detalle de alertas.
- `Components/Settings`: pantallas administrativas.
- `Components/Shell`: navegacion inferior, cabecera y shell autenticado.
- `Components/Home` y `Components/WordPress`: dashboard y noticias.

## Code-behind

La convencion del proyecto es separar markup y C#:

- `.razor` contiene estructura visual y binding.
- `.razor.cs` contiene estado, parametros, inyecciones, carga asincrona y manejadores.

Ejemplo: `Components/Club/ClubPhonebookView.razor` y `Components/Club/ClubPhonebookView.razor.cs`.

## Abstracciones

Los componentes consumen interfaces de `SharedUI/Abstractions`, por ejemplo `IPhonebookService`, `ISignalService`, `IFederativeLicenseService` o `IMemberProfileService`. Esto permite probar componentes con dobles y cambiar la infraestructura sin reescribir UI.

## Modelos

Los modelos de `SharedUI/Models` representan contratos de formularios, peticiones, resultados de API y objetos listos para renderizar. Mantenerlos aqui evita duplicacion entre componentes y clientes.

## Localizacion

Los literales se resuelven mediante `IStringLocalizer<SharedTexts>` y recursos en `SharedUI/Resources`. Las claves deben ser cortas y estables.

## Regla de frontera

`SharedUI` no debe conocer URLs base, configuracion de host ni detalles de transporte HTTP. Para eso existen las abstracciones y las implementaciones registradas por el cliente o el host.
