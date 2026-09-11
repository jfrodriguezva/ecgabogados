# Arquitectura inicial de costo mínimo

> Este documento describe la ejecución completa en un VPS. Para el piloto en las
> capas gratuitas de Azure, consultar `DESPLIEGUE_COSTO_CERO.md`.

## Componentes

Solo se mantienen tres servicios Docker:

1. `web`: Next.js, sitio público, panel y proxy interno `/api`.
2. `api`: ASP.NET Core, sin puerto público.
3. `sql`: SQL Server Express, sin puerto público.

El dominio `ecgabogados.mx` apunta únicamente al frontend. No se necesita `api.ecgabogados.mx` ni Gateway.

## Recursos eliminados de la etapa inicial

- API Gateway/Ocelot.
- Balanceador dedicado.
- Base de datos administrada.
- Container Registry de pago: las imágenes pueden construirse directamente en el VPS.
- Key Vault de pago: los secretos se guardan en un archivo de entorno con permisos exclusivos del administrador del servidor.
- Blob Storage operativo: los documentos permanecen en un volumen Docker persistente.
- Azure DNS: puede conservarse el DNS del registrador o usar Cloudflare gratuito.
- Application Insights: inicialmente bastan logs rotados, alertas de disponibilidad y espacio.
- Subdominio público para la API.

## Elementos que no deben eliminarse

- HTTPS y renovación automática del certificado.
- Firewall; solo se exponen 80/443 y SSH restringido.
- Respaldo diario cifrado fuera del VPS de SQL y documentos.
- Prueba periódica de restauración.
- Actualizaciones de seguridad.
- Monitoreo de disponibilidad, disco y memoria.

## Capacidad mínima

- 2 vCPU.
- 4 GB RAM (2 GB no es recomendable al compartir servidor con SQL Server).
- 60–80 GB SSD.
- Ubuntu LTS.
- SQL Server Express.

## Presupuesto operativo objetivo

Con un VPS económico de 4 GB:

| Concepto | Estimación mensual MXN |
|---|---:|
| VPS | $250–$550 |
| Respaldo externo cifrado | $50–$150 |
| Monitoreo y certificado | $0 |
| Correo, inicialmente | $0–$150 |
| **Total objetivo** | **$300–$850** |

El dominio se paga anualmente y no forma parte del servidor. El costo exacto depende del proveedor, impuestos y tipo de cambio.

## Momento para migrar a servicios administrados

Separar SQL y documentos cuando el servidor supere de forma sostenida 70% de memoria, existan varias personas trabajando simultáneamente, se requiera alta disponibilidad o la recuperación manual deje de ser aceptable para la operación.
