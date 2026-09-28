using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using IndaloAventurApi.Api.Common;
using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Infrastructure;
using IndaloAventurApi.Infrastructure.Media;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        if (context.HttpContext.Request.Path.StartsWithSegments("/api/users") && context.HttpContext.Request.Path.Value?.EndsWith("/password", StringComparison.OrdinalIgnoreCase) == true)
        {
            var actorId = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "desconocido";
            var targetUserId = context.HttpContext.Request.RouteValues["userId"]?.ToString() ?? "desconocido";
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("PasswordChangeAudit");
            logger.LogWarning("Cambio de contrasena administrativo rechazado. ActorId={ActorId} TargetUserId={TargetUserId} StatusCode={StatusCode} CorrelationId={CorrelationId}", actorId, targetUserId, StatusCodes.Status400BadRequest, context.HttpContext.TraceIdentifier);
        }

        return new BadRequestObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Solicitud no valida",
            Detail = "Los datos enviados no son validos.",
            Instance = context.HttpContext.Request.Path
        });
    };
});
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, _) =>
    {
        var httpContext = context.HttpContext;
        var actorId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonimo";
        var targetUserId = httpContext.Request.RouteValues["userId"]?.ToString() ?? "desconocido";
        var logger = httpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("PasswordChangeAudit");
        logger.LogWarning("Cambio de contrasena administrativo limitado. ActorId={ActorId} TargetUserId={TargetUserId} StatusCode={StatusCode} CorrelationId={CorrelationId}", actorId, targetUserId, StatusCodes.Status429TooManyRequests, httpContext.TraceIdentifier);
        httpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return ValueTask.CompletedTask;
    };
    options.AddPolicy(ApiRateLimitPolicies.AdministrativePasswordChange, httpContext =>
    {
        var actorId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonimo";
        var targetUserId = httpContext.Request.RouteValues["userId"]?.ToString() ?? "desconocido";
        return RateLimitPartition.GetFixedWindowLimiter(
            $"{actorId}:{targetUserId}",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? [];

    options.AddPolicy("FrontendDevCors", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var nameIdentifier = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(nameIdentifier, out var userId))
                {
                    context.HttpContext.Items["AuthFailureDetail"] = "Token de usuario invalido.";
                    context.Fail("Token de usuario invalido.");
                    return;
                }

                var identityService = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
                var isActive = await identityService.IsUserActiveAsync(userId, context.HttpContext.RequestAborted);
                if (!isActive)
                {
                    context.HttpContext.Items["AuthFailureDetail"] = "El usuario esta inactivo.";
                    context.Fail("El usuario esta inactivo.");
                    return;
                }

                var securityStamp = context.Principal?.FindFirst(AuthClaimNames.SecurityStamp)?.Value;
                if (!await identityService.IsSecurityStampValidAsync(userId, securityStamp ?? string.Empty, context.HttpContext.RequestAborted))
                {
                    context.HttpContext.Items["AuthFailureDetail"] = "La sesion ya no es valida.";
                    context.Fail("La sesion ya no es valida.");
                }
            },
            OnChallenge = async context =>
            {
                if (context.Response.HasStarted)
                {
                    return;
                }

                context.HandleResponse();
                await WriteProblemDetailsAsync(
                    context.HttpContext,
                    StatusCodes.Status401Unauthorized,
                    "No autorizado",
                    ResolveUnauthorizedDetail(context.HttpContext, context.AuthenticateFailure));
            },
            OnForbidden = context =>
                WriteProblemDetailsAsync(
                    context.HttpContext,
                    StatusCodes.Status403Forbidden,
                    "Acceso denegado",
                    "El usuario autenticado no tiene permisos suficientes para este recurso.")
        };
    });

builder.Services.AddAuthorization(AuthorizationPolicies.Configure());

static string ResolveUnauthorizedDetail(HttpContext httpContext, Exception? failure)
{
    if (httpContext.Items.TryGetValue("AuthFailureDetail", out var detail) && detail is string failureDetail)
    {
        return failureDetail;
    }

    return failure switch
    {
        SecurityTokenExpiredException => "El token de acceso ha expirado.",
        SecurityTokenException => "El token de acceso no es valido.",
        _ => "Se requiere autenticacion valida."
    };
}

static Task WriteProblemDetailsAsync(HttpContext httpContext, int statusCode, string title, string detail)
{
    httpContext.Response.StatusCode = statusCode;
    httpContext.Response.ContentType = "application/problem+json";

    return httpContext.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = statusCode,
        Title = title,
        Detail = detail,
        Instance = httpContext.Request.Path
    });
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("FrontendDevCors");
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();
app.MapControllers();

await app.Services.InitializeIdentityAsync();
await app.Services.InitializeSignalImageStorageAsync();

app.Run();

/// <summary>
/// Punto de entrada de la aplicacion web.
/// </summary>
public partial class Program;
