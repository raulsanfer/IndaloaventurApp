using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using IndaloAventurApi.Application.Abstractions.Identity;
using IndaloAventurApi.Domain.FichasSocio;
using IndaloAventurApi.Infrastructure.Persistence.DbContext;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class ManagedUserNameSearchTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    [Theory]
    [InlineData("Daniela", "García Montes", "Dan")]
    [InlineData("Daniela", "García Montes", "nie")]
    [InlineData("Daniela", "García Montes", "ela")]
    [InlineData("Daniela", "García Montes", "Daniela")]
    [InlineData("Daniela", "García Montes", "Gar")]
    [InlineData("Daniela", "García Montes", "arc")]
    [InlineData("Daniela", "García Montes", "cía")]
    [InlineData("Daniela", "García Montes", "García")]
    [InlineData("Daniela", "García Montes", "  CÍA  ")]
    [InlineData("Daniela", "GARCÍA Montes", "cía")]
    [InlineData("Daniela", "García Montes", "Mon")]
    [InlineData("Daniela", "García Montes", "ont")]
    [InlineData("Daniela", "García Montes", "tes")]
    [InlineData("Daniela", "García Montes", "Montes")]
    [InlineData("Daniela", "García Montes", "  nIE  ")]
    [InlineData("Ana Marina", "Torres Vidal", "  aRI  ")]
    public async Task List_WithName_ShouldMatchAnyPositionInNameOrEitherSurname(
        string nombre, string apellidos, string filter)
    {
        using var client = factory.CreateClient();
        await factory.AuthenticateAsAdminAsync(client);
        var match = await ManagedUserSearchData.CreateAsync(factory.Services, Email(), nombre, apellidos);
        var other = await ManagedUserSearchData.CreateAsync(factory.Services, Email(), "Luis", "Santos Ruiz");

        var response = await client.GetAsync($"/api/users?nombre={Uri.EscapeDataString(filter)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<ManagedUserDto[]>();
        Assert.NotNull(users);
        Assert.Contains(users, user => user.UserId == match);
        Assert.DoesNotContain(users, user => user.UserId == other);
    }

    [Theory]
    [InlineData("%")]
    [InlineData("_")]
    [InlineData("[")]
    public async Task List_WithName_ShouldTreatSqlWildcardsAsLiteralText(string filter)
    {
        using var client = factory.CreateClient();
        await factory.AuthenticateAsAdminAsync(client);
        var match = await ManagedUserSearchData.CreateAsync(factory.Services, Email(), $"Ana{filter}Marina", "Santos");
        var other = await ManagedUserSearchData.CreateAsync(factory.Services, Email(), "AnaXMarina", "Santos");

        var users = await client.GetFromJsonAsync<ManagedUserDto[]>($"/api/users?nombre={Uri.EscapeDataString(filter)}");

        Assert.NotNull(users);
        Assert.Contains(users, user => user.UserId == match);
        Assert.DoesNotContain(users, user => user.UserId == other);
    }

    [Fact]
    public async Task List_WithBothFilters_ShouldRequireBothAndPreserveContractAndOrdering()
    {
        using var client = factory.CreateClient();
        await factory.AuthenticateAsAdminAsync(client);
        var group = Guid.NewGuid().ToString("N");
        var zeta = await ManagedUserSearchData.CreateAsync(factory.Services, $"zeta-{group}@club.test", "Ana Marina", "Torres");
        var alpha = await ManagedUserSearchData.CreateAsync(factory.Services, $"alpha-{group}@club.test", "Luis", "Marinero");
        await ManagedUserSearchData.CreateAsync(factory.Services, $"other-{group}@club.test", "Luis", "Santos");
        await ManagedUserSearchData.CreateAsync(factory.Services, Email(), "Ana Marina", "Torres");
        await ManagedUserSearchData.CreateAsync(factory.Services, $"marina-{group}@club.test");

        var response = await client.GetAsync($"/api/users?email=%20{group.ToUpperInvariant()}%20&nombre=%20mARIn%20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        foreach (var user in document.RootElement.EnumerateArray())
        {
            Assert.Equal(new[] { "email", "isMember", "roles", "userId" },
                user.EnumerateObject().Select(property => property.Name).OrderBy(name => name).ToArray());
        }
        var users = await response.Content.ReadFromJsonAsync<ManagedUserDto[]>();
        Assert.NotNull(users);
        Assert.Equal(new[] { alpha, zeta }, users.Select(user => user.UserId));
        Assert.All(users, user =>
        {
            Assert.False(user.IsMember);
            Assert.Empty(user.Roles);
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task List_WithoutEffectiveName_ShouldIncludeUsersWithoutFicha(string? nombre)
    {
        using var client = factory.CreateClient();
        await factory.AuthenticateAsAdminAsync(client);
        var email = Email();
        var userId = await ManagedUserSearchData.CreateAsync(factory.Services, email);
        var nameQuery = nombre is null ? "" : $"nombre={Uri.EscapeDataString(nombre)}&";

        var all = await client.GetFromJsonAsync<ManagedUserDto[]>($"/api/users?{nameQuery}");
        var filtered = await client.GetFromJsonAsync<ManagedUserDto[]>($"/api/users?{nameQuery}email={email.ToUpperInvariant()}");

        Assert.NotNull(all);
        Assert.Contains(all, user => user.UserId == userId);
        Assert.Equal(all.Select(user => user.Email).OrderBy(value => value, StringComparer.Ordinal), all.Select(user => user.Email));
        Assert.Equal(userId, Assert.Single(filtered!).UserId);
    }

    [Fact]
    public async Task List_WithName_ShouldExcludeUsersWithoutFichaEvenWhenEmailMatches()
    {
        using var client = factory.CreateClient();
        await factory.AuthenticateAsAdminAsync(client);
        var fragment = Guid.NewGuid().ToString("N");
        await ManagedUserSearchData.CreateAsync(factory.Services, $"{fragment}@club.test");

        foreach (var emailQuery in new[] { "", $"&email={fragment}" })
        {
            var response = await client.GetAsync($"/api/users?nombre={fragment}{emailQuery}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty((await response.Content.ReadFromJsonAsync<ManagedUserDto[]>())!);
        }
    }

    [Fact]
    public async Task List_WithName_ShouldRemainAdminOnly()
    {
        using var client = factory.CreateClient();
        var anonymousResponse = await client.GetAsync("/api/users?nombre=ana");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        var email = Email();
        var password = $"A1a{Guid.NewGuid():N}";
        await factory.EnsureRolesAndAdminAsync();
        await ManagedUserSearchData.CreateAsync(factory.Services, email, password: password);
        var login = await client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var token = await login.Content.ReadFromJsonAsync<LoginPayload>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);

        var forbiddenResponse = await client.GetAsync("/api/users?nombre=ana");
        Assert.Equal(HttpStatusCode.Forbidden, forbiddenResponse.StatusCode);
    }

    private static string Email() => $"name-search-{Guid.NewGuid():N}@club.test";

    private sealed record LoginPayload(string AccessToken);
}

internal static class ManagedUserSearchData
{
    public static async Task<Guid> CreateAsync(
        IServiceProvider services, string email, string? nombre = null, string? apellidos = null, string? password = null)
    {
        using var scope = services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = new Usuario { UserName = email, Email = email, IsMember = false };
        var result = password is null ? await manager.CreateAsync(user) : await manager.CreateAsync(user, password);
        Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(error => error.Description)));

        if (nombre is not null)
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.FichasSocio.Add(FichaSocio.Crear(user.Id, null, nombre, apellidos!, "12345678Z",
                new DateOnly(1990, 1, 1), "Calle A", "04001", "Almeria", "Almeria", "+34600111222",
                email, null, true, false, false));
            await db.SaveChangesAsync();
        }

        return user.Id;
    }
}
