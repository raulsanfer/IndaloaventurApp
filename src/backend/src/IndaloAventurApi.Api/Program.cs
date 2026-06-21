using System.Text;
using IndaloAventurApi.Api.Common;
using IndaloAventurApi.Api.Security;
using IndaloAventurApi.Application;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Infrastructure;
using IndaloAventurApi.Infrastructure.Media;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
app.UseAuthorization();
app.MapControllers();

await app.Services.InitializeIdentityAsync();
await app.Services.InitializeSignalImageStorageAsync();

app.Run();

public partial class Program;
