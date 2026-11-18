using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace IndaloAventurApi.IntegrationTests;

[Trait("Category", "SqlServer")]
public sealed class ManagedUserNameSearchSqlServerTests
{
    [Fact]
    public async Task List_ShouldExecuteNameAndEmailFiltersInSqlServer_WithLiteralSubstrings()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();
        var configured = configuration.GetConnectionString("api_ContextConnection")
            ?? configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(configured))
            throw SkipException.ForSkip("La prueba de busqueda requiere SQL Server con permiso para crear una base aislada.");

        const string prefix = "indalo_user_search_tests_";
        var connection = new SqlConnectionStringBuilder(configured) { InitialCatalog = prefix + Guid.NewGuid().ToString("N") };
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connection.ConnectionString).Options;
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(connection.ConnectionString));
        services.AddIdentityCore<Usuario>().AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddSingleton<ISocialTokenValidator, UnusedSocialTokenValidator>();
        services.AddScoped<IIdentityService, IdentityService>();
        await using var provider = services.BuildServiceProvider();

        try
        {
            await using (var db = new ApplicationDbContext(options))
                await db.Database.EnsureCreatedAsync();

            var zeta = await ManagedUserSearchData.CreateAsync(provider, "zeta@match.test", "Ana Marina", "García Montes");
            var alpha = await ManagedUserSearchData.CreateAsync(provider, "alpha@match.test", "Luis", "Marinero");
            await ManagedUserSearchData.CreateAsync(provider, "email-only@match.test", "Pedro", "Santos");
            var nameOnly = await ManagedUserSearchData.CreateAsync(provider, "name-only@other.test", "Ana Marina", "Torres");
            var noFicha = await ManagedUserSearchData.CreateAsync(provider, "marina@match.test");
            var wildcards = new Dictionary<string, Guid>();
            foreach (var (filter, email) in new[] { ("%", "percent"), ("_", "underscore"), ("[", "bracket") })
                wildcards[filter] = await ManagedUserSearchData.CreateAsync(provider, $"{email}@literal.test", $"Ana{filter}Marina", "Santos");

            using var scope = provider.CreateScope();
            var identity = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            var combined = await identity.ListUsersAsync("  MATCH.TEST  ", "  aRI  ", CancellationToken.None);
            Assert.Equal(new[] { alpha, zeta }, combined.Select(user => user.UserId));
            var nameMatches = await identity.ListUsersAsync(null, "ari", CancellationToken.None);
            Assert.Contains(nameMatches, user => user.UserId == nameOnly);
            Assert.DoesNotContain(nameMatches, user => user.UserId == noFicha);
            Assert.All(combined, user => Assert.False(user.IsMember));

            foreach (var filter in new[] { "CÍA", "ont" })
                Assert.Equal(zeta, Assert.Single(await identity.ListUsersAsync(null, filter, CancellationToken.None)).UserId);
            foreach (var (filter, userId) in wildcards)
                Assert.Equal(userId, Assert.Single(await identity.ListUsersAsync(null, filter, CancellationToken.None)).UserId);
            Assert.Empty(await identity.ListUsersAsync("match.test", "no-match", CancellationToken.None));
            Assert.Contains(await identity.ListUsersAsync(null, "  ", CancellationToken.None), user => user.UserId == noFicha);
        }
        finally
        {
            var target = new SqlConnectionStringBuilder(connection.ConnectionString).InitialCatalog;
            if (!target.StartsWith(prefix, StringComparison.Ordinal) || target.Length != prefix.Length + 32)
                throw new InvalidOperationException("La limpieza solo admite la base de pruebas aislada.");
            await using var db = new ApplicationDbContext(options);
            await db.Database.EnsureDeletedAsync();
        }
    }

    private sealed class UnusedSocialTokenValidator : ISocialTokenValidator
    {
        public Task<SocialTokenValidationResult> ValidateAsync(string provider, string token, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }
}
