# Ejecución reproducible con contenedores

Esta receta permite probar el conjunto completo sin comprometerse con un proveedor. No sustituye HTTPS, respaldos ni la revisión legal requeridos para producción.

## Preparación

1. Instalar Docker Desktop o Docker Engine con Compose.
2. Copiar `.env.container.example` como `.env.container`.
3. Reemplazar `ECG_SQL_PASSWORD`, `ECG_JWT_SECRET` y `ECG_ADMIN_PASSWORD` por valores
   fuertes. Confirmar también `ECG_ADMIN_EMAIL` y `ECG_ADMIN_NAME`.
4. Construir e iniciar:

```bash
docker compose --env-file .env.container up -d --build
```

## Inicializar la base

Esperar a que SQL Server termine de iniciar y ejecutar el esquema una sola vez:

```bash
docker compose --env-file .env.container cp backend/database/schema.sql sql:/tmp/schema.sql
docker compose --env-file .env.container exec sql /bin/bash -c '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /tmp/schema.sql'
```

Si la instalación de SQL Server contiene las herramientas en `/opt/mssql-tools/bin`, usar esa ruta en el segundo comando.

Después de aplicar el esquema, la API se recupera automáticamente y crea la primera
administradora con las variables `ECG_ADMIN_*`, únicamente si la tabla de usuarios está
vacía. No es necesario insertar un hash o una contraseña de demostración.

Servicios locales:

- Sitio: `http://localhost:3000`
- API a través del sitio: `http://localhost:3000/api/*`
- Salud del conjunto: `http://localhost:3000/health`

La API y SQL Server no publican puertos al host. Next.js reenvía internamente `/api/*`
a la API. La base usa el volumen persistente `sql-data`; los documentos nuevos se
guardan dentro de SQL para conservarlos también en plataformas sin disco persistente.

## Detener

```bash
docker compose --env-file .env.container down
```

No usar `down -v` salvo que se pretenda borrar deliberadamente la base y los documentos del ambiente.
