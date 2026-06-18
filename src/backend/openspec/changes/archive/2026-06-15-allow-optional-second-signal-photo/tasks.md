## 1. Regla de negocio y contratos

- [x] 1.1 Ajustar el agregado `Signal` para permitir que la creacion acepte ausencia de `Foto2` manteniendo `Foto1` como obligatoria.
- [x] 1.2 Adaptar el comando, validador y contrato API de creacion de signals para que `Foto2` no sea obligatoria al dar de alta una senal.
- [x] 1.3 Revisar el impacto en actualizacion y en la consulta de imagenes para mantener un comportamiento coherente cuando la segunda foto este ausente.

## 2. Verificacion

- [x] 2.1 Actualizar pruebas de dominio/aplicacion para cubrir la creacion valida con una sola foto y los rechazos que deben seguir vigentes.
- [x] 2.2 Actualizar pruebas de integracion del endpoint de signals para verificar el alta con `Foto2` omitida o vacia.
- [x] 2.3 Ejecutar la bateria relevante de pruebas automatizadas antes de marcar las tareas como completadas.
