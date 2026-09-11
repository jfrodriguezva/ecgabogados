using System.Text;
using System.Threading.RateLimiting;
using ECAbogados.Application;
using ECAbogados.Application.Interfaces;
using ECAbogados.Domain.Entities;
using ECAbogados.Infrastructure;
using ECAbogados.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// CORS — orígenes configurables (appsettings "Cors:AllowedOrigins"), listo para
// agregar el dominio real sin tocar código cuando se despliegue.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:3000"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Rate limiting (built-in de .NET, sin paquetes): ventanas fijas por IP para
// proteger login (fuerza bruta) y los endpoints públicos anónimos (spam).
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("auth", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "sin-ip",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));

    options.AddPolicy("public", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "sin-ip",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 20,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32 || jwtSecret.StartsWith("CAMBIAR-", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "No se encontró 'Jwt:Secret'. Configúralo con 'dotnet user-secrets set \"Jwt:Secret\" \"<valor>\"' " +
        "en desarrollo, o con la variable de entorno Jwt__Secret en cualquier otro ambiente.");
}
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ECG Abogados API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT precedido de la palabra 'Bearer' (ej: Bearer eyJhbGci...)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Database:Initialize"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var connectionFactory = scope.ServiceProvider.GetRequiredService<SqlConnectionFactory>();
    var schemaPath = Path.Combine(AppContext.BaseDirectory, "database", "schema.sql");
    await DatabaseInitializer.InitializeAsync(connectionFactory, schemaPath);

    var usuarios = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    if ((await usuarios.GetAllAsync()).Count == 0)
    {
        var email = builder.Configuration["BootstrapAdmin:Email"];
        var password = builder.Configuration["BootstrapAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || password.Length < 12 ||
            password.StartsWith("CAMBIAR-", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Para inicializar producción define BootstrapAdmin__Email y una BootstrapAdmin__Password de al menos 12 caracteres.");

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await usuarios.CreateAsync(new Usuario
        {
            Email = email.Trim().ToLowerInvariant(),
            Nombre = builder.Configuration["BootstrapAdmin:Nombre"] ?? "Erika Cruz García",
            PasswordHash = hasher.Hash(password),
            Rol = "Administrador"
        });
    }
}

if (!app.Environment.IsDevelopment() && builder.Configuration.GetValue("Security:UseHttpsRedirection", true))
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Encabezados defensivos para la API y Swagger. La política de contenido del
// frontend se administra por separado en Next.js.
app.Use(async (context, next) =>
{
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.XFrameOptions = "DENY";
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
    await next();
});

// La documentación interactiva se expone en desarrollo. En otro ambiente debe
// habilitarse deliberadamente con Swagger__Enabled=true.
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ECG Abogados API v1");
    });
}

app.UseCors("Frontend");

// Convierte los ValidationException de FluentValidation (lanzados por el Sender
// propio en ECAbogados.Application.Mediation) en un 400 con el detalle de campos.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new
        {
            title = "One or more validation errors occurred.",
            status = 400,
            errors
        });
    }
    catch (KeyNotFoundException)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Recurso no encontrado.",
            status = 404
        });
    }
    catch (UnauthorizedAccessException)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new
        {
            title = "No fue posible autorizar la solicitud.",
            status = 401
        });
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error no controlado al procesar {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Ocurrió un error interno.",
            status = 500
        });
    }
});

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .AllowAnonymous()
    .ExcludeFromDescription();

app.Run();
