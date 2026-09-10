# Evaluación funcional vigente — ECG Abogados

Actualizada el 10 de septiembre de 2026 con base en el código versionado.

## Estado del producto

ECG Abogados es un MVP funcional a la medida de un despacho unipersonal. Integra captación, solicitudes de cita, administración de clientes y expedientes, portal del cliente, documentos, conversaciones por asunto, etapas, requisitos, plazos, tarifas, honorarios y pagos de control. Comercializadora permanece como módulo futuro y no se procesan cobros electrónicos.

## Funciones disponibles

- Sitio público y páginas por servicio con foco en divorcio incausado.
- Solicitud de cita con fecha, horario y modalidad propuesta, sujeta a confirmación.
- Acceso separado para clientes, asistentes y administración.
- Cada cliente consulta únicamente sus expedientes.
- Seguimiento por estatus, etapas, requisitos, fechas y plazos.
- Documentos y conversación separada por expediente.
- Tarifas por área y registro administrativo de honorarios/pagos, sin cobro en línea.
- Auditoría, recuperación de contraseña y notificaciones configurables por correo.
- Pruebas unitarias y validación continua en GitHub Actions.

## Fortalezas

- Identidad visual coherente con la marca y llamadas a la acción claras.
- Flujo desde prospecto hasta seguimiento del asunto.
- Arquitectura separada en frontend, API, aplicación, dominio e infraestructura.
- JWT, contraseñas BCrypt, autorización por rol y límites para solicitudes públicas.
- Sin dependencia obligatoria de servicios comerciales para ejecutarse.

## Pendientes antes de uso público

- Elegir hosting, dominio y almacenamiento persistente.
- Configurar SQL Server, secretos, SMTP, HTTPS, CORS y respaldos.
- Completar y revisar jurídicamente el aviso de privacidad.
- Cargar información operativa real y ejecutar pruebas de aceptación con la licenciada.
- Auditar accesibilidad y reforzar observabilidad y recuperación ante desastres.

## Limitaciones conocidas

- Los documentos se guardan en disco local; el hosting debe proporcionar volumen persistente o almacenamiento de objetos.
- No hay firma electrónica, facturación fiscal, control de horas ni sincronización con calendarios externos.
- WhatsApp es manual; no existe integración con WhatsApp Business API.
- La cobertura automatizada es representativa, no exhaustiva; faltan pruebas de integración y navegador.
- Está diseñado para ECG Abogados, no como plataforma multiempresa.

## Veredicto

El alcance funcional solicitado para el MVP está implementado. El proyecto aún no debe manejar expedientes reales hasta completar la configuración, privacidad, respaldo y prueba de aceptación de `docs/CHECKLIST_PRODUCCION.md`.
