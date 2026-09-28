## 1. Contrato y acceso administrativo

- [x] 1.1 Extender el contrato y cliente de gestión administrativa con la solicitud dedicada de cambio de contraseña, y verificar mediante prueba HTTP que se envían solo `newPassword` y el `userId` de ruta.
- [x] 1.2 Añadir la navegación desde `AdminMemberProfileView` y la nueva ruta administrativa, y verificar que el enlace conserva el `userId` objetivo.

## 2. Formulario seguro

- [x] 2.1 Crear el componente SharedUI y su modelo con campo de contraseña, validación alfanumérica de 1 a 20 caracteres, estado de carga y errores seguros; verificar los escenarios con pruebas bUnit.
- [x] 2.2 Añadir recursos localizados y estilos SCSS coherentes con Settings, y verificar que no se usan estilos inline ni se muestra/registra el secreto.

## 3. Documentación y validación

- [x] 3.1 Crear `.vscode/admin-user-password-change-backend.md` con el contrato, autorización, validación, auditoría, rate limiting, invalidación de sesión y respuestas seguras del endpoint; verificar que no contiene secretos ni código de hash propio.
- [x] 3.2 Actualizar la documentación funcional de administración y verificarla con `docfx docs/docfx.json`.
- [x] 3.3 Ejecutar `dotnet test IndaloaventurApp.Frontend.Tests/IndaloaventurApp.Frontend.Tests.csproj` y `openspec validate frontend-admin-user-password-change --strict`; corregir las regresiones atribuibles al cambio.
