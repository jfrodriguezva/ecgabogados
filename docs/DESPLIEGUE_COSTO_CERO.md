# Despliegue piloto con objetivo de costo cero

## Alcance y límite de la garantía

Azure Container Apps es un servicio de consumo: sus cuotas gratuitas reducen el costo,
pero un presupuesto de Azure solo alerta y no constituye un interruptor de gasto. Por
ello no es técnicamente posible garantizar que una aplicación pública, dinámica y con
tráfico ilimitado permanezca siempre en $0 dentro de una suscripción de pago por uso.

La garantía absoluta de no facturación exige una de estas decisiones:

1. publicar únicamente un sitio estático en un servicio gratuito y dejar fuera API,
   autenticación, expedientes, mensajes, citas persistentes y documentos; o
2. apagar/eliminar los recursos de consumo antes de rebasar la cuota; o
3. ejecutar el sistema en equipo propio, aceptando electricidad, conectividad y
   mantenimiento fuera de Azure.

La configuración siguiente busca $0 durante un piloto de bajo tráfico y evita que Azure
SQL continúe cobrando al agotar su cuota.

## Componentes

- `web`: Azure Container App en plan Consumption, mínimo 0 y máximo 1 réplica.
- `api`: Azure Container App en plan Consumption, mínimo 0 y máximo 1 réplica.
- `sql`: Azure SQL Database Free Offer con pausa al alcanzar el límite gratuito.
- imágenes: GitHub Container Registry (GHCR), paquetes públicos.
- DNS: proveedor del dominio; no contratar Azure DNS.
- TLS: certificado administrado gratuito de Container Apps.

No crear Azure Container Registry, Gateway, Front Door, Key Vault, App Service Plan,
máquinas virtuales, IP pública independiente ni almacenamiento premium.

## Publicación de imágenes

El flujo `.github/workflows/ci.yml` valida el código y, después de un `push` correcto a
`main`, publica:

- `ghcr.io/jfrodriguezva/ecgabogados-web:latest`
- `ghcr.io/jfrodriguezva/ecgabogados-api:latest`

También publica una etiqueta inmutable con el SHA del commit. En GitHub, abrir cada
paquete y establecer su visibilidad como pública para que Azure pueda descargarlo sin
mantener credenciales de registro. Ningún secreto debe incluirse en las imágenes.

## Barreras de costo

1. En Azure SQL seleccionar **Auto-pause the database until next month** al alcanzar el
   límite gratuito. No seleccionar la opción que continúa con cargos.
2. En ambas Container Apps configurar `minReplicas: 0` y `maxReplicas: 1`, usando el
   tamaño mínimo que soporte la aplicación.
3. Crear un presupuesto mensual de $1 MXN y alertas al 50%, 80% y 100%. Esto avisa; no
   detiene automáticamente el consumo.
4. No habilitar réplicas siempre activas, zonas de disponibilidad, almacenamiento de
   logs prolongado ni servicios complementarios.
5. Revisar semanalmente Cost Analysis y las cuotas durante el piloto.
6. Usar etiquetas SHA para poder regresar a una versión anterior sin reconstruir.

Para un corte automático aproximado se puede crear una automatización que, al recibir
una alerta, establezca las aplicaciones en cero o las deshabilite. No es garantía
absoluta: la información de costos puede llegar con retraso y ya podría existir consumo
facturable antes de ejecutarse la acción.

## Funciones incompatibles con un cero estricto

- procesos permanentes y recordatorios automáticos;
- correo o WhatsApp transaccional de pago;
- archivos ilimitados o almacenamiento durable adicional;
- base de datos siempre activa;
- alta disponibilidad y múltiples réplicas;
- retención extensa de logs y respaldos;
- garantías de disponibilidad o respuesta inmediata sin arranque en frío.

El sitio público, solicitudes de cita, usuarios, expedientes, etapas, tarifas, control
manual de pagos y conversaciones pueden conservarse mientras el consumo permanezca
dentro de las cuotas. Los documentos requieren almacenamiento durable: para el piloto
deben limitarse o migrarse a la base gratuita; el disco local de Container Apps no es
un repositorio permanente.
