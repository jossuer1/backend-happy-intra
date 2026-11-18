using Intranet.Data;
using Microsoft.EntityFrameworkCore;
using Intranet.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Npgsql;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
// Registrar servicios
builder.Services.AddHttpClient<IEmailService, BrevoEmailService>();
builder.Services.AddControllers();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var espera))
            context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(espera.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
        await context.HttpContext.Response.WriteAsJsonAsync(new { mensaje = "Demasiados intentos. Espera unos minutos antes de volver a intentar." }, token);
    };
    void AgregarLimite(string nombre, int intentos, TimeSpan ventana)
        => options.AddPolicy(nombre, context => RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "sin-ip",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = intentos, Window = ventana, QueueLimit = 0,
                AutoReplenishment = true
            }));
    AgregarLimite("login", 10, TimeSpan.FromMinutes(1));
    AgregarLimite("cambio-clave", 5, TimeSpan.FromMinutes(5));
    AgregarLimite("recuperacion", 3, TimeSpan.FromMinutes(15));
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IImagenPerfilService, CloudinaryImagenPerfilService>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IImagenService, ImagenService>();

builder.Services.AddScoped<ICloudinaryUploadService, CloudinaryUploadService>();

builder.Services.AddScoped<IVacacionService, VacacionService>();

builder.Services.AddScoped<PerfilPropioAplicador>();
builder.Services.AddScoped<ISolicitudPerfilService, SolicitudPerfilService>();

builder.Services.AddScoped<DashboardRrhhService>();
builder.Services.AddScoped<CumpleanosService>();
builder.Services.AddHostedService<CumpleanosBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendLocal", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:3000",
            "https://intranet-happy.onrender.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Registrar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Swagger (UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//configuracion JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Cambiar a true en producción si usas HTTPS estricto
    options.SaveToken = true;
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            if (!long.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                context.Fail("Identidad inválida.");
                return;
            }
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var usuario = await db.Usuarios.AsNoTracking().Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id, context.HttpContext.RequestAborted);
            if (!AccesoToken.EsVigente(context.Principal!, usuario))
                context.Fail("La cuenta o sus permisos cambiaron. Inicia sesión nuevamente.");
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
var app = builder.Build();


app.MapHealthChecks("/health");

// Configurar Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendLocal");
app.UseRateLimiter();
app.Use(async (context, next) =>
{
    try { await next(); }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new { mensaje = "Ya existe un registro con esa cédula, correo u otro dato único. Actualiza la información e inténtalo nuevamente." });
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new { mensaje = "Un registro relacionado cambió o ya no existe. Actualiza la página y revisa los datos seleccionados." });
    }
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
