# ECG Abogados

Nueva plataforma independiente para la Lic. Erika Cruz García.

## Alcance

- Sitio público de captación con foco en divorcio incausado.
- Catálogo jurídico, contratación de orientación SAT y módulo futuro de comercializadora.
- Solicitudes de cita con fecha, horario y modalidad, sujetas a confirmación.
- Portal de clientes por usuario y contraseña, organizado por expediente.
- Panel para la abogada/administración: prospectos, agenda, expedientes, documentos, conversaciones, tarifas, honorarios y pagos.

## Perfiles

- Invitado: consulta servicios y solicita una valoración indicando servicio, modalidad, fecha y horario preferidos. Puede avisar manualmente por WhatsApp con un mensaje prellenado.
- Cliente: inicia sesión con correo y contraseña, visualiza únicamente sus expedientes y consulta etapa, requisitos, documentos, fechas, pagos y conversaciones separadas.
- Abogada/Administración: gestiona prospectos, crea accesos, vincula expedientes, carga información, administra tarifas y atiende citas.

## Decisiones operativas

- Las solicitudes no son citas confirmadas.
- La atención principal es digital; la modalidad presencial queda sujeta a confirmación.
- No se procesan cobros electrónicos.
- Las tarifas se organizan por área (Jurídico, SAT y Comercializadora) y sirven como importes base.
- Comercializadora se mantiene como módulo futuro.
- El aviso de privacidad es un borrador editable y no debe publicarse sin completar el domicilio para notificaciones y revisión responsable.

## Posicionamiento

ECG Abogados se presenta como despacho jurídico digital en Ecatepec, con atención en línea y citas presenciales sujetas a confirmación. No se publica una dirección física mientras no exista una sede de atención.

## Tecnología

Next.js 16, React 19, TypeScript, Tailwind CSS, ASP.NET Core 10, SQL Server y Dapper.

El sistema anterior permanece en la carpeta Lawyer y no forma parte de este proyecto.

## Estado

El MVP funcional está implementado. Antes de utilizar datos reales deben completarse el
hosting, secretos, correo, respaldos, aviso de privacidad y prueba de aceptación indicados
en [`docs/CHECKLIST_PRODUCCION.md`](docs/CHECKLIST_PRODUCCION.md).

Para levantar un ambiente reproducible con contenedores, consultar
[`docs/CONTENEDORES.md`](docs/CONTENEDORES.md).

La alternativa inicial de tres servicios y costo mínimo está documentada en
[`docs/ARQUITECTURA_ECONOMICA.md`](docs/ARQUITECTURA_ECONOMICA.md).

La publicación gratuita de imágenes y los controles para evitar cargos accidentales
en Azure están documentados en [`docs/DESPLIEGUE_COSTO_CERO.md`](docs/DESPLIEGUE_COSTO_CERO.md).
