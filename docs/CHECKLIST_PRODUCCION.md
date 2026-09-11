# Checklist para puesta en producción

Este documento separa lo implementado de las decisiones y credenciales necesarias antes de publicar.

## 1. Infraestructura

Azure y SMTP se excluyen por decisión del propietario. Los puntos de hosting, dominio,
HTTPS y respaldo se ejecutarán cuando se elija otro proveedor.

- [ ] Elegir proveedor para Next.js, la API .NET y SQL Server.
- [ ] Registrar o conectar el dominio definitivo y habilitar HTTPS.
- [ ] Crear un ambiente de prueba privado antes del público.
- [ ] Configurar copias de seguridad automáticas de la base y documentos.

## 2. Variables y secretos

No guardar secretos reales en GitHub. Configurarlos en el hosting:

| Variable | Uso |
|---|---|
| `ConnectionStrings__Default` | Conexión a SQL Server |
| `Jwt__Secret` | Firma de sesiones; usar un valor aleatorio largo |
| `Cors__AllowedOrigins__0` | Dominio público del frontend |
| `Sitio__BaseUrl` | URL pública del sitio |
| `Smtp__Host`, `Smtp__Port`, `Smtp__User`, `Smtp__Password`, `Smtp__From` | Correo |
| `Notificaciones__StaffEmail` | Correo que recibe avisos |
| `API_INTERNAL_URL` | URL privada de la API para el proxy de Next.js |
| `NEXT_PUBLIC_SITE_URL` | Dominio público usado por SEO |

## 3. Datos operativos

- [x] WhatsApp, teléfono y correo coinciden con el material proporcionado.
- [ ] Confirmar horarios reales de atención.
- [x] Se eliminaron credenciales predeterminadas; la primera contraseña se recibe como secreto.
- [ ] Mantener las citas presenciales como “ubicación por confirmar” mientras no exista sede.
- [x] El esquema de producción no contiene clientes, citas, expedientes ni tarifas ficticias.
- [ ] Cargar tarifas reales con la licenciada.

## 4. Privacidad y seguridad

- [ ] Completar el medio para notificaciones y revisar jurídicamente el aviso de privacidad.
- [ ] Validar procedimiento ARCO, transferencias y política de retención.
- [ ] Verificar la restauración de respaldos.
- [x] Swagger está desactivado por defecto fuera de desarrollo; no activar `Swagger__Enabled` públicamente.
- [x] La API incluye encabezados defensivos, respuestas de error seguras y `GET /health` para monitoreo.
- [x] El frontend incluye encabezados defensivos y reenvía `/api/*` directamente a la API interna.
- [x] Dependabot revisa semanalmente las dependencias de npm/NuGet y mensualmente GitHub Actions.
- [x] Existen imágenes reproducibles para web y API; CI valida Compose y publica ambas en GHCR.
- [x] Los documentos se almacenan en SQL y no dependen del disco efímero del contenedor.
- [x] La base se inicializa idempotentemente y crea la primera administradora desde secretos.

## 5. Prueba de aceptación

La compilación, lint, pruebas unitarias y construcción de imágenes están automatizadas y
aprobadas. Los recorridos siguientes requieren un ambiente con SQL y validación humana.

- [ ] Visitante solicita cita con modalidad y fecha propuesta.
- [ ] La abogada confirma, reprograma o cancela.
- [ ] Administración crea cliente y expediente.
- [ ] El cliente inicia sesión y solo ve sus asuntos.
- [ ] Ambas partes intercambian mensajes y documentos en el expediente correcto.
- [ ] Se registran etapas, requisitos, fechas, tarifas y pagos de control.
- [ ] Se comprueba recuperación de contraseña y expiración de enlaces.
- [ ] Se prueba en teléfono, tableta y escritorio.

## 6. Fuera del alcance inicial

- Cobros electrónicos y dirección física permanente.
- Automatización de WhatsApp.
- Operación de Comercializadora, conservada como “Próximamente”.
- Desarrollo editorial continuo de alertas y cambios del SAT.
