using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Application.Abstractions.LicenciasFederativas;
using IndaloAventurApi.Application.Abstractions.ClubPositions;
using IndaloAventurApi.Application.Abstractions.FichasSocio;
using IndaloAventurApi.Application.Abstractions.Phonebook;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Application.Features.Auth.PasswordRecovery;
using IndaloAventurApi.Application.Features.LicenciasFederativas.CreateSolicitudLicenciaFederativa;
using IndaloAventurApi.Infrastructure.Email;
using IndaloAventurApi.Infrastructure.Media;
using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Application.Abstractions.WordPress;
using IndaloAventurApi.Infrastructure.Persistence;
using IndaloAventurApi.Infrastructure.Persistence.ClubPositions;
using IndaloAventurApi.Infrastructure.Persistence.FichasSocio;
using IndaloAventurApi.Infrastructure.Persistence.LicenciasFederativas;
using IndaloAventurApi.Infrastructure.Persistence.Phonebook;
using IndaloAventurApi.Infrastructure.Persistence.TrailSignals;
using IndaloAventurApi.Infrastructure.Security;
using IndaloAventurApi.Infrastructure.WordPress;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace IndaloAventurApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AdminSeedOptions>(configuration.GetSection(AdminSeedOptions.SectionName));
        services.Configure<SocialAuthOptions>(configuration.GetSection(SocialAuthOptions.SectionName));
        services.Configure<PasswordRecoveryOptions>(configuration.GetSection(PasswordRecoveryOptions.SectionName));
        services.Configure<SmtpEmailSenderOptions>(configuration.GetSection(SmtpEmailSenderOptions.SectionName));
        services.Configure<FederativeLicenseRequestNotificationOptions>(configuration.GetSection(FederativeLicenseRequestNotificationOptions.SectionName));
        services.Configure<WordPressOptions>(configuration.GetSection(WordPressOptions.SectionName));
        services.Configure<SignalImageStorageOptions>(configuration.GetSection(SignalImageStorageOptions.SectionName));

        var wordPressOptions = configuration.GetSection(WordPressOptions.SectionName).Get<WordPressOptions>() ?? new WordPressOptions();
        ValidateWordPressOptions(wordPressOptions);
        var signalImageStorageOptions = configuration.GetSection(SignalImageStorageOptions.SectionName).Get<SignalImageStorageOptions>() ?? new SignalImageStorageOptions();
        ValidateSignalImageStorageOptions(signalImageStorageOptions);

        var connectionString = configuration.GetConnectionString("api_ContextConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No database connection string configured. Expected 'api_ContextConnection' or 'DefaultConnection'.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDataProtection();

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IQueryConnectionFactory, SqlQueryConnectionFactory>();
        services.AddScoped<ICargoRepository, CargoRepository>();
        services.AddScoped<IFichaSocioRepository, FichaSocioRepository>();
        services.AddScoped<IFichaContactoRepository, FichaContactoRepository>();
        services.AddScoped<ITarifaLicenciaFederativaRepository, TarifaLicenciaFederativaRepository>();
        services.AddScoped<ISolicitudLicenciaFederativaRepository, SolicitudLicenciaFederativaRepository>();
        services.AddScoped<ISignalTypeRepository, SignalTypeRepository>();
        services.AddScoped<ISignalRepository, SignalRepository>();
        services.AddSingleton<ISignalImageStorage, FileSystemSignalImageStorage>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ISocialTokenValidator, GoogleSocialTokenValidator>();
        services.AddHttpClient<IWordPressService, WordPressService>(client =>
            {
                client.BaseAddress = new Uri(wordPressOptions.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(wordPressOptions.TimeoutSeconds);
                var authRaw = $"{wordPressOptions.Username}:{wordPressOptions.ApplicationPassword}";
                var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authRaw));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authBase64);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // Some Windows dev machines cannot reach the certificate revocation endpoint
                // of the remote site. Keep the workaround opt-in and scoped to this client.
                CheckCertificateRevocationList = !wordPressOptions.DisableCertificateRevocationCheck
            });

        services.AddIdentityCore<Usuario>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<Usuario>>(TokenOptions.DefaultProvider);

        return services;
    }

    private static void ValidateWordPressOptions(WordPressOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            throw new InvalidOperationException("La configuracion de WordPress requiere 'BaseUrl'.");
        }

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("La configuracion de WordPress tiene un 'BaseUrl' invalido.");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            throw new InvalidOperationException("La configuracion de WordPress requiere 'Username'.");
        }

        if (string.IsNullOrWhiteSpace(options.ApplicationPassword))
        {
            throw new InvalidOperationException("La configuracion de WordPress requiere 'ApplicationPassword'.");
        }

        if (string.IsNullOrWhiteSpace(options.PostsEndpoint))
        {
            throw new InvalidOperationException("La configuracion de WordPress requiere 'PostsEndpoint'.");
        }

        if (options.TimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("La configuracion de WordPress requiere 'TimeoutSeconds' mayor que 0.");
        }

        if (options.DefaultPostsPageSize <= 0 || options.DefaultPostsPageSize > 100)
        {
            throw new InvalidOperationException("La configuracion de WordPress requiere 'DefaultPostsPageSize' entre 1 y 100.");
        }
    }

    private static void ValidateSignalImageStorageOptions(SignalImageStorageOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.RootPath))
        {
            throw new InvalidOperationException("La configuracion de imagenes de senales requiere 'RootPath'.");
        }
    }
}
