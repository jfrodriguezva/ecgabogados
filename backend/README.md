# ECG Abogados - Backend

Backend del sistema de gestión de casos para el despacho ECG Abogados, construido en .NET 10
con arquitectura por capas (Domain / Application / Infrastructure / Api) más un API Gateway
basado en Ocelot.

## Proyectos

- `src/ECAbogados.Domain` - Entidades del dominio.
- `src/ECAbogados.Application` - Casos de uso (MediatR), DTOs, validaciones (FluentValidation) y puertos.
- `src/ECAbogados.Infrastructure` - Implementaciones con Dapper/SQL Server, JWT y hashing de contraseñas.
- `src/ECAbogados.Api` - API REST (ASP.NET Core Web API), puerto `5080`.
- `src/ECAbogados.Gateway` - API Gateway (Ocelot), puerto `5000`. El frontend debe consumir este puerto.

## Requisitos

- .NET 10 SDK
- SQL Server local o accesible (Trusted Connection por defecto)

## Puesta en marcha

1. **Crear la base de datos y el esquema**

   Ejecutar el script `database/schema.sql` contra tu instancia de SQL Server (crea la base
   de datos `ECAbogados` si no existe, las tablas y datos de ejemplo):

   ```
   sqlcmd -S localhost -i database/schema.sql
   ```

> **Importante**: el script incluye un administrador de desarrollo. Cambia la contraseña
> inicial antes de usar datos reales y no cargues los datos de ejemplo en producción.

2. **Compilar la solución**

   ```
   dotnet build backend/ECAbogados.sln
   ```

3. **Revisar la cadena de conexión y el secreto JWT**

   Ver `src/ECAbogados.Api/appsettings.json` (`ConnectionStrings:Default` y sección `Jwt`).
   Ajustar según tu entorno.

4. **Ejecutar la API** (puerto `5080`)

   ```
   dotnet run --project backend/src/ECAbogados.Api
   ```

   Swagger UI queda disponible en `http://localhost:5080/swagger`.

5. **Ejecutar el Gateway** (puerto `5000`, en otra terminal)

   ```
   dotnet run --project backend/src/ECAbogados.Gateway
   ```

   El frontend (`frontend/web`, que corre en `http://localhost:3000`) debe apuntar a
   `http://localhost:5000/api/...`.

## Login por defecto (una vez reemplazado el hash)

- Email: `erika@ecabogados.mx`
- Password: la que hayas usado para generar el hash (sugerida en el script: `Cambiar123!`)

## Validación

```
dotnet test backend/ECAbogados.sln
```

El repositorio incluye pruebas xUnit y GitHub Actions para validar backend y frontend.

## Notas

- No se incluye aún una receta de despliegue porque el proveedor no está definido.
- Los archivos subidos por `POST /api/documentos` se guardan en
  `src/ECAbogados.Api/App_Data/documentos/{casoId}/{guid}_{nombreArchivo}`.
