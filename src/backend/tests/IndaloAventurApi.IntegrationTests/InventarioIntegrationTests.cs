using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IndaloAventurApi.IntegrationTests;

public sealed class InventarioIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public InventarioIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Inventario_AnonimoNoPuedeConsultarMaestros()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await _httpClient.GetAsync("/api/inventario-categorias")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _httpClient.GetAsync("/api/inventario-estados")).StatusCode);
    }

    [Fact]
    public async Task Inventario_AdminGestionaMaestros_YMemberSoloConsulta()
    {
        await _factory.AuthenticateAsAdminAsync(_httpClient);
        var suffix = Guid.NewGuid().ToString("N");

        var createRoot = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Material {suffix}", CategoriaPadreId = (int?)null });
        Assert.Equal(HttpStatusCode.Created, createRoot.StatusCode);
        var rootId = await createRoot.Content.ReadFromJsonAsync<int>();

        var createChild = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Cascos {suffix}", CategoriaPadreId = rootId });
        Assert.Equal(HttpStatusCode.Created, createChild.StatusCode);
        var childId = await createChild.Content.ReadFromJsonAsync<int>();

        var thirdLevel = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Integral {suffix}", CategoriaPadreId = childId });
        Assert.Equal(HttpStatusCode.Conflict, thirdLevel.StatusCode);
        Assert.Contains("Una subcategoria no puede tener subcategorias.", await thirdLevel.Content.ReadAsStringAsync(), StringComparison.Ordinal);

        var missingParent = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Invalida {suffix}", CategoriaPadreId = 999999 });
        Assert.Equal(HttpStatusCode.NotFound, missingParent.StatusCode);
        Assert.Contains("La categoria padre no existe.", await missingParent.Content.ReadAsStringAsync(), StringComparison.Ordinal);

        var duplicateRoot = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $" material {suffix} ", CategoriaPadreId = (int?)null });
        Assert.Equal(HttpStatusCode.Conflict, duplicateRoot.StatusCode);

        var treeResponse = await _httpClient.GetAsync("/api/inventario-categorias");
        Assert.True(treeResponse.IsSuccessStatusCode, await treeResponse.Content.ReadAsStringAsync());
        var tree = await treeResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<InventarioCategoriaPayload>>();
        Assert.Contains(tree!, x => x.Id == rootId && x.Subcategorias.Count == 1);

        var deleteRoot = await _httpClient.DeleteAsync($"/api/inventario-categorias/{rootId}");
        Assert.Equal(HttpStatusCode.Conflict, deleteRoot.StatusCode);

        var createEstado = await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $"Disponible {suffix}" });
        Assert.Equal(HttpStatusCode.Created, createEstado.StatusCode);

        var duplicateEstado = await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $" disponible {suffix} " });
        Assert.Equal(HttpStatusCode.Conflict, duplicateEstado.StatusCode);
        Assert.Contains("Ya existe un estado de inventario con ese nombre.", await duplicateEstado.Content.ReadAsStringAsync(), StringComparison.Ordinal);

        await AuthenticateAsMemberAsync();
        Assert.Equal(HttpStatusCode.OK, (await _httpClient.GetAsync("/api/inventario-categorias")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _httpClient.GetAsync("/api/inventario-estados")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"No permitido {suffix}", CategoriaPadreId = (int?)null })).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $"Bloqueado {suffix}" })).StatusCode);
    }

    [Fact]
    public async Task Inventario_AdminActualizaYEliminaRegistrosHoja()
    {
        await _factory.AuthenticateAsAdminAsync(_httpClient);
        var suffix = Guid.NewGuid().ToString("N");

        var createRoot = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Material {suffix}", CategoriaPadreId = (int?)null });
        var rootId = await createRoot.Content.ReadFromJsonAsync<int>();
        var createChild = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Cascos {suffix}", CategoriaPadreId = rootId });
        var childId = await createChild.Content.ReadFromJsonAsync<int>();

        var updateChild = await _httpClient.PutAsJsonAsync($"/api/inventario-categorias/{childId}", new { Nombre = $"Cascos de escalada {suffix}", CategoriaPadreId = rootId });
        Assert.Equal(HttpStatusCode.NoContent, updateChild.StatusCode);

        var childDetail = await _httpClient.GetFromJsonAsync<InventarioCategoriaPayload>($"/api/inventario-categorias/{childId}");
        Assert.NotNull(childDetail);
        Assert.Equal($"Cascos de escalada {suffix}", childDetail!.Nombre);

        Assert.Equal(HttpStatusCode.NoContent, (await _httpClient.DeleteAsync($"/api/inventario-categorias/{childId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _httpClient.GetAsync($"/api/inventario-categorias/{childId}")).StatusCode);

        var createEstado = await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $"Disponible {suffix}" });
        var estadoId = await createEstado.Content.ReadFromJsonAsync<int>();
        var updateEstado = await _httpClient.PutAsJsonAsync($"/api/inventario-estados/{estadoId}", new { Nombre = $"En revision {suffix}" });
        Assert.Equal(HttpStatusCode.NoContent, updateEstado.StatusCode);

        var estadoDetail = await _httpClient.GetFromJsonAsync<InventarioEstadoPayload>($"/api/inventario-estados/{estadoId}");
        Assert.NotNull(estadoDetail);
        Assert.Equal($"En revision {suffix}", estadoDetail!.Nombre);

        Assert.Equal(HttpStatusCode.NoContent, (await _httpClient.DeleteAsync($"/api/inventario-estados/{estadoId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _httpClient.GetAsync($"/api/inventario-estados/{estadoId}")).StatusCode);
    }

    [Fact]
    public async Task Inventario_NombreInvalidoDevuelveProblemDetailsEnEspanol()
    {
        await _factory.AuthenticateAsAdminAsync(_httpClient);

        var response = await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = string.Empty });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("El nombre es obligatorio.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Inventario_ListadosDevuelvenArbolYEstadosOrdenadosPorNombre()
    {
        await _factory.AuthenticateAsAdminAsync(_httpClient);
        var suffix = Guid.NewGuid().ToString("N");

        var rootZResponse = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Zeta {suffix}", CategoriaPadreId = (int?)null });
        var rootAResponse = await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Alfa {suffix}", CategoriaPadreId = (int?)null });
        Assert.Equal(HttpStatusCode.Created, rootZResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, rootAResponse.StatusCode);
        var rootAId = await rootAResponse.Content.ReadFromJsonAsync<int>();
        Assert.Equal(HttpStatusCode.Created, (await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Zumo {suffix}", CategoriaPadreId = rootAId })).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await _httpClient.PostAsJsonAsync("/api/inventario-categorias", new { Nombre = $"Agua {suffix}", CategoriaPadreId = rootAId })).StatusCode);

        var categorias = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<InventarioCategoriaPayload>>("/api/inventario-categorias");
        Assert.NotNull(categorias);
        var raicesCreadas = categorias!.Where(x => x.Nombre.EndsWith(suffix, StringComparison.Ordinal)).ToArray();
        Assert.Equal([$"Alfa {suffix}", $"Zeta {suffix}"], raicesCreadas.Select(x => x.Nombre));
        Assert.Equal([$"Agua {suffix}", $"Zumo {suffix}"], raicesCreadas[0].Subcategorias.Select(x => x.Nombre));

        Assert.Equal(HttpStatusCode.Created, (await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $"Zeta {suffix}" })).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await _httpClient.PostAsJsonAsync("/api/inventario-estados", new { Nombre = $"Alfa {suffix}" })).StatusCode);

        var estados = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<InventarioEstadoPayload>>("/api/inventario-estados");
        Assert.NotNull(estados);
        Assert.Equal([$"Alfa {suffix}", $"Zeta {suffix}"], estados!.Where(x => x.Nombre.EndsWith(suffix, StringComparison.Ordinal)).Select(x => x.Nombre));
    }

    private async Task AuthenticateAsMemberAsync()
    {
        var email = $"inventario-member-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        var login = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var payload = await login.Content.ReadFromJsonAsync<LoginPayload>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
    }

    private sealed record LoginPayload(string AccessToken);
    private sealed record InventarioCategoriaPayload(int Id, string Nombre, int? CategoriaPadreId, IReadOnlyCollection<InventarioSubcategoriaPayload> Subcategorias);
    private sealed record InventarioSubcategoriaPayload(int Id, string Nombre);
    private sealed record InventarioEstadoPayload(int Id, string Nombre);
}
