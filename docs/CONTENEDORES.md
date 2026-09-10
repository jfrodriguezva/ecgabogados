# Ejecución reproducible con contenedores

Esta receta permite probar el conjunto completo sin comprometerse con un proveedor. No sustituye HTTPS, respaldos ni la revisión legal requeridos para producción.

## Preparación

1. Instalar Docker Desktop o Docker Engine con Compose.
2. Copiar `.env.container.example` como `.env.container`.
3. Reemplazar `ECG_SQL_PASSWORD` y `ECG_JWT_SECRET` por valores fuertes.
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

Servicios locales:

- Sitio: `http://localhost:3000`
- Gateway: `http://localhost:5000`
- Salud del Gateway: `http://localhost:5000/health`

La API y SQL Server no publican puertos al host. Los documentos y la base usan volúmenes persistentes llamados `documentos` y `sql-data`.

## Detener

```bash
docker compose --env-file .env.container down
```

No usar `down -v` salvo que se pretenda borrar deliberadamente la base y los documentos del ambiente.
