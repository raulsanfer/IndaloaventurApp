# Project Context: IndaloAventurAPI

IndaloAventurAPI es un API REST desarrollada sobre stack .Net y base de datos SqlServer que permitira de forma privada a traves de validacion JWT el acceso a informacion y procesos para nutrir a una aplicacion movil del club de montanismo indaloaventura.

## Stack
- Language: C#
- Runtime: .Net 9
- Database: SQL Server 2025 Express (persistence)

## Claves a tener en cuenta
- Todos los literales usados en el API que puedan representarse en un front o cliente, deben ir en Espanol, no hacemos uso de localizacion.
- En identidad, el rol `Member` es el rol tecnico por defecto para cualquier usuario registrado o creado en su primer login social.
- El flag `IsMember` no equivale a ese rol: solo lo marca administracion cuando el usuario pasa a ser socio del club tras validar el pago de su cuota.
