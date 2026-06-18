using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using IndaloAventurApi.Application.Abstractions.Email;
using IndaloAventurApi.Application.Abstractions.Phonebook;
using IndaloAventurApi.Application.Abstractions.Persistence;
using IndaloAventurApi.Application.Abstractions.Security;
using IndaloAventurApi.Application.Abstractions.WordPress;
using IndaloAventurApi.Domain.FichasContacto;
using IndaloAventurApi.Infrastructure.Persistence;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IndaloAventurApi.IntegrationTests;

public sealed class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public ApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task Register_Then_Login_Should_ReturnToken()
    {
        await EnsureRolesAndAdminAsync();

        var email = $"user-{Guid.NewGuid():N}@club.test";
        var password = "Test1234A";

        var registerResponse = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.AccessToken));
        Assert.False(payload!.IsMember);
    }

    [Fact]
    public async Task Social_Login_Should_ReturnToken()
    {
        await EnsureRolesAndAdminAsync();

        var response = await _httpClient.PostAsJsonAsync("/api/auth/social-login", new
        {
            Provider = "google",
            Token = "social-test-token"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.AccessToken));
        Assert.False(payload!.IsMember);
    }

    [Fact]
    public async Task Login_ShouldReturnIsMemberAndEmitClaim_WhenUserIsMember()
    {
        await EnsureRolesAndAdminAsync();

        var email = $"member-claim-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";

        await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        await SetUserMembershipAsync(email, true);

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.NotNull(payload);
        Assert.True(payload!.IsMember);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(payload.AccessToken);
        Assert.Equal("true", token.Claims.Single(x => x.Type == AuthClaimNames.IsMember).Value);
    }

    [Fact]
    public async Task SocialLogin_ShouldEmitFalseIsMemberClaim_ForNewSocialUser()
    {
        await EnsureRolesAndAdminAsync();

        var response = await _httpClient.PostAsJsonAsync("/api/auth/social-login", new
        {
            Provider = "google",
            Token = "social-test-token"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.NotNull(payload);
        Assert.False(payload!.IsMember);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(payload.AccessToken);
        Assert.Equal("false", token.Claims.Single(x => x.Type == AuthClaimNames.IsMember).Value);
    }

    [Fact]
    public async Task UserManagement_WithoutToken_Should_ReturnUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserManagement_WithMemberRole_Should_ReturnForbidden()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();
        var response = await _httpClient.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UserManagement_Create_WithMemberRole_Should_ReturnForbidden()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.PostAsJsonAsync("/api/users", new
        {
            Email = $"managed-forbidden-{Guid.NewGuid():N}@club.test",
            Password = "Test1234A",            Roles = new[] { "Member" }
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UserManagement_WithAdminRole_Should_AllowCrudFlow()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var createResponse = await _httpClient.PostAsJsonAsync("/api/users", new
        {
            Email = $"managed-{Guid.NewGuid():N}@club.test",
            Password = "Test1234A",            Roles = new[] { "Member" }
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdUserId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var listResponse = await _httpClient.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var users = await listResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<ManagedUserPayload>>();
        Assert.Contains(users!, u => u.UserId == createdUserId);

        var updateResponse = await _httpClient.PutAsJsonAsync($"/api/users/{createdUserId:D}", new
        {
            Email = $"managed-updated-{Guid.NewGuid():N}@club.test",            Roles = new[] { "Member", "Admin" }
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact]
    public async Task UserManagement_ListWithoutEmailFilter_ShouldReturnAllUsersOrderedByEmail()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var thirdEmail = $"zeta-{Guid.NewGuid():N}@club.test";
        var firstEmail = $"alpha-{Guid.NewGuid():N}@club.test";
        var secondEmail = $"middle-{Guid.NewGuid():N}@club.test";

        await CreateManagedUserAsync(thirdEmail);
        await CreateManagedUserAsync(firstEmail);
        await CreateManagedUserAsync(secondEmail);

        var response = await _httpClient.GetAsync("/api/users?email=   ");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ManagedUserPayload>>();
        Assert.NotNull(users);
        Assert.Contains(users!, user => user.Email == firstEmail);
        Assert.Contains(users, user => user.Email == secondEmail);
        Assert.Contains(users, user => user.Email == thirdEmail);

        var orderedEmails = users.Select(user => user.Email).ToArray();
        var sortedEmails = orderedEmails.OrderBy(email => email, StringComparer.Ordinal).ToArray();
        Assert.Equal(sortedEmails, orderedEmails);
    }

    [Fact]
    public async Task UserManagement_ListWithEmailFilter_ShouldReturnPartialMatches_IgnoringCase()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var matchOneEmail = $"sender+alpha-{Guid.NewGuid():N}@club.test";
        var matchTwoEmail = $"ALPHA-team-{Guid.NewGuid():N}@club.test";
        var otherEmail = $"bravo-{Guid.NewGuid():N}@club.test";

        await CreateManagedUserAsync(matchOneEmail);
        await CreateManagedUserAsync(matchTwoEmail);
        await CreateManagedUserAsync(otherEmail);

        var response = await _httpClient.GetAsync("/api/users?email=%20%20AlPhA%20%20");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ManagedUserPayload>>();
        Assert.NotNull(users);
        Assert.Contains(users!, user => user.Email == matchOneEmail);
        Assert.Contains(users, user => user.Email == matchTwoEmail);
        Assert.DoesNotContain(users, user => user.Email == otherEmail);
    }

    [Fact]
    public async Task UserManagement_ListWithEmailFilter_ShouldReturnEmptyCollection_WhenNoMatchesExist()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        await CreateManagedUserAsync($"managed-{Guid.NewGuid():N}@club.test");

        var response = await _httpClient.GetAsync($"/api/users?email=no-match-{Guid.NewGuid():N}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ManagedUserPayload>>();
        Assert.NotNull(users);
        Assert.Empty(users!);
    }

    [Fact]
    public async Task UserManagement_Admin_CanDeactivateAndReactivate_UserLoginIsBlockedWhileDeactivated()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var managedEmail = $"managed-status-{Guid.NewGuid():N}@club.test";
        const string managedPassword = "Test1234A";

        var createResponse = await _httpClient.PostAsJsonAsync("/api/users", new
        {
            Email = managedEmail,
            Password = managedPassword,            Roles = new[] { "Member" }
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdUserId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var deactivateResponse = await _httpClient.PostAsync($"/api/users/{createdUserId:D}/deactivate", content: null);
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        _httpClient.DefaultRequestHeaders.Authorization = null;
        var deactivatedLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = managedEmail, Password = managedPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, deactivatedLoginResponse.StatusCode);

        await AuthenticateAsAdminAsync();
        var reactivateResponse = await _httpClient.PostAsync($"/api/users/{createdUserId:D}/reactivate", content: null);
        Assert.Equal(HttpStatusCode.NoContent, reactivateResponse.StatusCode);

        _httpClient.DefaultRequestHeaders.Authorization = null;
        var activeLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = managedEmail, Password = managedPassword });
        Assert.Equal(HttpStatusCode.OK, activeLoginResponse.StatusCode);
    }

    [Fact]
    public async Task Authorization_WithPreviouslyIssuedToken_ShouldBeDenied_WhenUserGetsDeactivated()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var managedEmail = $"managed-token-{Guid.NewGuid():N}@club.test";
        const string managedPassword = "Test1234A";

        var createResponse = await _httpClient.PostAsJsonAsync("/api/users", new
        {
            Email = managedEmail,
            Password = managedPassword,            Roles = new[] { "Member" }
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdUserId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        _httpClient.DefaultRequestHeaders.Authorization = null;
        var managedLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = managedEmail, Password = managedPassword });
        Assert.Equal(HttpStatusCode.OK, managedLoginResponse.StatusCode);
        var managedPayload = await managedLoginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        var previouslyIssuedToken = managedPayload!.AccessToken;

        await AuthenticateAsAdminAsync();
        var deactivateResponse = await _httpClient.PostAsync($"/api/users/{createdUserId:D}/deactivate", content: null);
        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", previouslyIssuedToken);
        var protectedResponse = await _httpClient.GetAsync("/api/agenda-telefonica");
        Assert.Equal(HttpStatusCode.Unauthorized, protectedResponse.StatusCode);
    }

    [Fact]
    public async Task AgendaTelefonica_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/agenda-telefonica");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AgendaTelefonica_WithMemberRole_ShouldAllowReadAndDenyWrite()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var createAsAdminResponse = await _httpClient.PostAsJsonAsync("/api/agenda-telefonica", new
        {
            Nombre = "Refugio de prueba",
            Telefono1 = "+34 600 111 222",
            Telefono2 = (string?)null,
            Email = "refugio@club.test",
            Direccion = "Calle Refugio 1",
            Observaciones = "Creado por admin"
        });
        Assert.Equal(HttpStatusCode.Created, createAsAdminResponse.StatusCode);

        await AuthenticateAsMemberAsync();

        var listResponse = await _httpClient.GetAsync("/api/agenda-telefonica");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var createAsMemberResponse = await _httpClient.PostAsJsonAsync("/api/agenda-telefonica", new
        {
            Nombre = "Intento no permitido",
            Telefono1 = "+34 600 999 888",
            Telefono2 = (string?)null,
            Email = "member@club.test",
            Direccion = "Calle Member 1",
            Observaciones = (string?)null
        });
        Assert.Equal(HttpStatusCode.Forbidden, createAsMemberResponse.StatusCode);
    }

    [Fact]
    public async Task AgendaTelefonica_WithAdminRole_ShouldAllowCrudFlow()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var createResponse = await _httpClient.PostAsJsonAsync("/api/agenda-telefonica", new
        {
            Nombre = "Guardia Civil MontaÃ±a",
            Telefono1 = "+34 600 123 456",
            Telefono2 = "+34 600 111 000",
            Email = "guardia@club.test",
            Direccion = "Plaza Guardia 2",
            Observaciones = "Emergencias"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var fichaId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var listResponse = await _httpClient.GetAsync("/api/agenda-telefonica");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var fichas = await listResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<FichaContactoPayload>>();
        Assert.Contains(fichas!, x => x.Id == fichaId);
        Assert.Contains(fichas!, x => x.Id == fichaId && x.Email == "guardia@club.test" && x.Direccion == "Plaza Guardia 2");

        var detailResponse = await _httpClient.GetAsync($"/api/agenda-telefonica/{fichaId:D}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        var detail = await detailResponse.Content.ReadFromJsonAsync<FichaContactoPayload>();
        Assert.NotNull(detail);
        Assert.Equal("guardia@club.test", detail!.Email);
        Assert.Equal("Plaza Guardia 2", detail.Direccion);

        var updateResponse = await _httpClient.PutAsJsonAsync($"/api/agenda-telefonica/{fichaId:D}", new
        {
            Nombre = "Guardia Civil MontaÃ±a Actualizado",
            Telefono1 = "+34 699 000 111",
            Telefono2 = (string?)null,
            Email = "guardia-actualizada@club.test",
            Direccion = "Avenida Actualizada 5",
            Observaciones = "Actualizado"
        });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var deleteResponse = await _httpClient.DeleteAsync($"/api/agenda-telefonica/{fichaId:D}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task FichaContactoRepository_ShouldSupportCrud()
    {
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IFichaContactoRepository>();

        var ficha = FichaContacto.Crear("Refugio", "+34 600111222", null, "refugio@club.test", "Calle Refugio 1", "Observaciones");
        await repository.AddAsync(ficha, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);

        var loaded = await repository.GetByIdAsync(ficha.Id, CancellationToken.None);
        Assert.NotNull(loaded);

        loaded!.Actualizar("Refugio actualizado", "+34 699000111", "+34 699000222", "actualizado@club.test", "Avenida Principal 10", "Obs");
        await repository.SaveChangesAsync(CancellationToken.None);

        var listed = await repository.ListAsync(CancellationToken.None);
        Assert.Contains(listed, x => x.Id == ficha.Id);

        repository.Remove(loaded);
        await repository.SaveChangesAsync(CancellationToken.None);

        var deleted = await repository.GetByIdAsync(ficha.Id, CancellationToken.None);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task TrailSignals_WithMemberRole_ShouldAllowCreateEditSearch_AndDenySignalTypeManagement()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var typeCreateAsAdmin = await _httpClient.PostAsJsonAsync("/api/signal-types", new
        {
            Nombre = "Incidencia",
            Icono = "alerta"
        });
        Assert.Equal(HttpStatusCode.Created, typeCreateAsAdmin.StatusCode);
        var signalTypeId = await typeCreateAsAdmin.Content.ReadFromJsonAsync<int>();

        await AuthenticateAsMemberAsync();

        var typeCreateAsMember = await _httpClient.PostAsJsonAsync("/api/signal-types", new
        {
            Nombre = "Aviso",
            Icono = "aviso"
        });
        Assert.Equal(HttpStatusCode.Forbidden, typeCreateAsMember.StatusCode);

        var getSignalTypesAsMember = await _httpClient.GetAsync("/api/signal-types");
        Assert.Equal(HttpStatusCode.OK, getSignalTypesAsMember.StatusCode);
        var signalTypes = await getSignalTypesAsMember.Content.ReadFromJsonAsync<IReadOnlyCollection<SignalTypePayload>>();
        Assert.Contains(signalTypes!, x => x.Id == signalTypeId && x.Nombre == "Incidencia");

        var createSignal = await _httpClient.PostAsJsonAsync("/api/signals", new
        {
            Latitud = 37.123f,
            Longitud = -2.456f,
            Titulo = "Piedras en sendero",
            Descripcion = "Piedras sueltas",
            Foto1 = new byte[] { 1, 2, 3 },
            Foto2 = new byte[] { 4, 5, 6 },
            Activo = true,
            Tipo = signalTypeId,
            Tags = "piedras,riesgo"
        });
        Assert.Equal(HttpStatusCode.Created, createSignal.StatusCode);
        var signalId = await createSignal.Content.ReadFromJsonAsync<Guid>();

        var updateSignal = await _httpClient.PutAsJsonAsync($"/api/signals/{signalId:D}", new
        {
            Titulo = "Piedras en sendero actualizado",
            Descripcion = "Piedras sueltas actualizadas",
            Activo = false
        });
        Assert.Equal(HttpStatusCode.NoContent, updateSignal.StatusCode);

        var search = await _httpClient.GetAsync($"/api/signals?tipo={signalTypeId}&activo=false");
        Assert.Equal(HttpStatusCode.OK, search.StatusCode);
        var items = await search.Content.ReadFromJsonAsync<IReadOnlyCollection<SignalPayload>>();
        Assert.Contains(items!, x => x.Id == signalId && x.Titulo == "Piedras en sendero actualizado" && x.Descripcion == "Piedras sueltas actualizadas");

        var imagesResponse = await _httpClient.GetAsync($"/api/signals/{signalId:D}/images");
        Assert.Equal(HttpStatusCode.OK, imagesResponse.StatusCode);
        var images = await imagesResponse.Content.ReadFromJsonAsync<SignalImagesPayload>();
        Assert.NotNull(images);
        Assert.Equal(signalId, images!.SignalId);
        Assert.Equal(new byte[] { 1, 2, 3 }, images.Foto1);
        Assert.Equal(new byte[] { 4, 5, 6 }, images.Foto2);

        var firstCommentResponse = await _httpClient.PostAsJsonAsync($"/api/signals/{signalId:D}/comments", new
        {
            Texto = "Paso estrecho pero transitable."
        });
        Assert.Equal(HttpStatusCode.Created, firstCommentResponse.StatusCode);

        var secondCommentResponse = await _httpClient.PostAsJsonAsync($"/api/signals/{signalId:D}/comments", new
        {
            Texto = "Mejor llevar precaucion."
        });
        Assert.Equal(HttpStatusCode.Created, secondCommentResponse.StatusCode);

        var commentsResponse = await _httpClient.GetAsync($"/api/signals/{signalId:D}/comments");
        Assert.Equal(HttpStatusCode.OK, commentsResponse.StatusCode);
        var comments = await commentsResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<SignalCommentPayload>>();
        Assert.NotNull(comments);
        Assert.Equal(2, comments!.Count);
        Assert.Collection(
            comments.OrderBy(x => x.FechaComentario),
            first => Assert.Equal("Paso estrecho pero transitable.", first.Texto),
            second => Assert.Equal("Mejor llevar precaucion.", second.Texto));
    }

    [Fact]
    public async Task TrailSignals_CreateWithoutSecondPhoto_ShouldPersistEmptySecondImage()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var typeCreateAsAdmin = await _httpClient.PostAsJsonAsync("/api/signal-types", new
        {
            Nombre = "Foto unica",
            Icono = "foto"
        });
        Assert.Equal(HttpStatusCode.Created, typeCreateAsAdmin.StatusCode);
        var signalTypeId = await typeCreateAsAdmin.Content.ReadFromJsonAsync<int>();

        await AuthenticateAsMemberAsync();

        var createSignal = await _httpClient.PostAsJsonAsync("/api/signals", new
        {
            Latitud = 37.123f,
            Longitud = -2.456f,
            Titulo = "Signal con una foto",
            Descripcion = "Solo una foto informada",
            Foto1 = new byte[] { 1, 2, 3 },
            Activo = true,
            Tipo = signalTypeId,
            Tags = "foto-unica"
        });
        Assert.Equal(HttpStatusCode.Created, createSignal.StatusCode);
        var signalId = await createSignal.Content.ReadFromJsonAsync<Guid>();

        var imagesResponse = await _httpClient.GetAsync($"/api/signals/{signalId:D}/images");
        Assert.Equal(HttpStatusCode.OK, imagesResponse.StatusCode);
        var images = await imagesResponse.Content.ReadFromJsonAsync<SignalImagesPayload>();
        Assert.NotNull(images);
        Assert.Equal(new byte[] { 1, 2, 3 }, images!.Foto1);
        Assert.Empty(images.Foto2);
    }

    [Fact]
    public async Task TrailSignals_EditOwnSignal_ShouldReturnForbidden_ForDifferentMember()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var typeCreateAsAdmin = await _httpClient.PostAsJsonAsync("/api/signal-types", new
        {
            Nombre = "Incidencia",
            Icono = "alerta"
        });
        Assert.Equal(HttpStatusCode.Created, typeCreateAsAdmin.StatusCode);
        var signalTypeId = await typeCreateAsAdmin.Content.ReadFromJsonAsync<int>();

        await AuthenticateAsMemberAsync();

        var createSignal = await _httpClient.PostAsJsonAsync("/api/signals", new
        {
            Latitud = 37.123f,
            Longitud = -2.456f,
            Titulo = "Piedras en sendero",
            Descripcion = "Piedras sueltas",
            Foto1 = new byte[] { 1, 2, 3 },
            Foto2 = new byte[] { 4, 5, 6 },
            Activo = true,
            Tipo = signalTypeId,
            Tags = "piedras"
        });
        Assert.Equal(HttpStatusCode.Created, createSignal.StatusCode);
        var signalId = await createSignal.Content.ReadFromJsonAsync<Guid>();

        await AuthenticateAsMemberAsync();

        var updateSignal = await _httpClient.PutAsJsonAsync($"/api/signals/{signalId:D}", new
        {
            Titulo = "Intento ajeno",
            Descripcion = "No deberia actualizarse",
            Activo = false
        });

        Assert.Equal(HttpStatusCode.Forbidden, updateSignal.StatusCode);
    }

    [Fact]
    public async Task TrailSignals_GetById_WithMemberRole_ShouldReturnSignalPayload()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var createType = await _httpClient.PostAsJsonAsync("/api/signal-types", new
        {
            Nombre = "Detalle",
            Icono = "detalle"
        });
        Assert.Equal(HttpStatusCode.Created, createType.StatusCode);
        var signalTypeId = await createType.Content.ReadFromJsonAsync<int>();

        await AuthenticateAsMemberAsync();

        var createSignal = await _httpClient.PostAsJsonAsync("/api/signals", new
        {
            Latitud = 36.789f,
            Longitud = -2.123f,
            Titulo = "Titulo detalle",
            Descripcion = "Signal para detalle",
            Foto1 = new byte[] { 31, 32, 33 },
            Foto2 = new byte[] { 34, 35, 36 },
            Activo = true,
            Tipo = signalTypeId,
            Tags = "detalle,prueba"
        });
        Assert.Equal(HttpStatusCode.Created, createSignal.StatusCode);
        var signalId = await createSignal.Content.ReadFromJsonAsync<Guid>();

        var response = await _httpClient.GetAsync($"/api/signals/{signalId:D}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<SignalPayload>();
        Assert.NotNull(payload);
        Assert.Equal(signalId, payload!.Id);
        Assert.Equal(36.789f, payload.Latitud);
        Assert.Equal(-2.123f, payload.Longitud);
        Assert.Equal("Titulo detalle", payload.Titulo);
        Assert.Equal("Signal para detalle", payload.Descripcion);
        Assert.True(payload.Activo);
        Assert.Equal(signalTypeId, payload.Tipo);
        Assert.Equal("detalle,prueba", payload.Tags);
        Assert.NotEqual(Guid.Empty, payload.UserIdAlta);
        Assert.NotEqual(Guid.Empty, payload.UserIdModificacion);
    }

    [Fact]
    public async Task TrailSignals_GetById_WhenSignalDoesNotExist_ShouldReturnNotFound()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.GetAsync($"/api/signals/{Guid.NewGuid():D}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TrailSignals_GetById_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _httpClient.GetAsync($"/api/signals/{Guid.NewGuid():D}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TrailSignalTypes_GetAll_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/signal-types");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TrailSignals_DeleteSignal_ShouldReturnConflict()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsAdminAsync();

        var createType = await _httpClient.PostAsJsonAsync("/api/signal-types", new
        {
            Nombre = "Obra",
            Icono = "obra"
        });
        Assert.Equal(HttpStatusCode.Created, createType.StatusCode);
        var signalTypeId = await createType.Content.ReadFromJsonAsync<int>();

        var createSignal = await _httpClient.PostAsJsonAsync("/api/signals", new
        {
            Latitud = 40.1f,
            Longitud = -3.2f,
            Titulo = "Obra en curso",
            Descripcion = "Zona en obras",
            Foto1 = new byte[] { 21, 22 },
            Foto2 = new byte[] { 23, 24 },
            Activo = true,
            Tipo = signalTypeId,
            Tags = "obra"
        });
        Assert.Equal(HttpStatusCode.Created, createSignal.StatusCode);
        var signalId = await createSignal.Content.ReadFromJsonAsync<Guid>();

        var deleteResponse = await _httpClient.DeleteAsync($"/api/signals/{signalId:D}");
        Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task TrailSignals_GetImages_WhenSignalDoesNotExist_ShouldReturnNotFound()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.GetAsync($"/api/signals/{Guid.NewGuid():D}/images");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task TrailSignals_Comments_ShouldReturnProblemDetails_ForInvalidOrMissingSignal()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var missingSignalResponse = await _httpClient.GetAsync($"/api/signals/{Guid.NewGuid():D}/comments");
        Assert.Equal(HttpStatusCode.NotFound, missingSignalResponse.StatusCode);

        var invalidCommentResponse = await _httpClient.PostAsJsonAsync($"/api/signals/{Guid.NewGuid():D}/comments", new
        {
            Texto = " "
        });
        Assert.Equal(HttpStatusCode.BadRequest, invalidCommentResponse.StatusCode);
    }

    [Fact]
    public async Task WordPressPosts_WithMemberRole_ShouldReturnMappedPosts()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();
        _factory.ResetWordPressService();

        var response = await _httpClient.GetAsync("/api/wordpress/posts?page=1&pageSize=2&search=montana");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var posts = JsonSerializer.Deserialize<IReadOnlyCollection<WordPressPostSummaryPayload>>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(posts);
        Assert.Equal(2, posts!.Count);
        Assert.Contains(posts, x => x.Slug == "ruta-de-prueba");
        Assert.Contains(posts, x => x.Slug == "ruta-de-prueba" && x.ImagenDestacadaUrl == "https://example.com/ruta-de-prueba.jpg");

        using var document = JsonDocument.Parse(content);
        var firstPost = document.RootElement[0];
        Assert.False(firstPost.TryGetProperty("contenido", out _));
        Assert.False(firstPost.TryGetProperty("enlace", out _));
        Assert.Equal(2, _factory.LastWordPressListPageSize);
        Assert.Equal("montana", _factory.LastWordPressListSearch);
    }

    [Fact]
    public async Task WordPressPosts_WithoutPageSize_ShouldUseConfiguredDefault()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();
        _factory.ResetWordPressService();

        var response = await _httpClient.GetAsync("/api/wordpress/posts?page=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(10, _factory.LastWordPressListPageSize);
    }

    [Fact]
    public async Task WordPressPostDetail_WithMemberRole_ShouldReturnPostBySlug()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.GetAsync("/api/wordpress/posts/ruta-de-prueba");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<WordPressPostPayload>();
        Assert.NotNull(post);
        Assert.Equal("ruta-de-prueba", post!.Slug);
        Assert.Equal("Contenido 1", post.Contenido);
        Assert.Equal("https://example.com/ruta-de-prueba.jpg", post.ImagenDestacadaUrl);
    }

    [Fact]
    public async Task WordPressPostDetail_ShouldReturnNotFound_WhenSlugDoesNotExist()
    {
        await EnsureRolesAndAdminAsync();
        await AuthenticateAsMemberAsync();

        var response = await _httpClient.GetAsync("/api/wordpress/posts/slug-inexistente");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task AuthenticateAsMemberAsync()
    {
        var email = $"member-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
    }

    private async Task AuthenticateAsAdminAsync()
    {
        await EnsureRolesAndAdminAsync();

        var loginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@indaloaventura.local",
            Password = "Admin1234A"
        });

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginPayload>();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload!.AccessToken);
    }

    private async Task EnsureRolesAndAdminAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

        if (!await roleManager.RoleExistsAsync(IdentityRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(IdentityRoles.Admin));
        }
        if (!await roleManager.RoleExistsAsync(IdentityRoles.Member))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(IdentityRoles.Member));
        }

        var admin = await userManager.FindByEmailAsync("admin@indaloaventura.local");
        if (admin is null)
        {
            admin = new Usuario
            {
                UserName = "admin@indaloaventura.local",
                Email = "admin@indaloaventura.local",
                EmailConfirmed = true,
                IsMember = false
            };
            await userManager.CreateAsync(admin, "Admin1234A");
        }

        if (!await userManager.IsInRoleAsync(admin, IdentityRoles.Admin))
        {
            await userManager.AddToRoleAsync(admin, IdentityRoles.Admin);
        }
    }

    private async Task SetUserMembershipAsync(string email, bool isMember)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);

        user!.IsMember = isMember;
        var result = await userManager.UpdateAsync(user);
        Assert.True(result.Succeeded, string.Join("; ", result.Errors.Select(x => x.Description)));
    }

    private async Task<Guid> CreateManagedUserAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users", new
        {
            Email = email,
            Password = "Test1234A",
            Roles = new[] { "Member" }
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private sealed record LoginPayload(string AccessToken, string TokenType, int ExpiresInSeconds, bool IsMember);
    private sealed record ManagedUserPayload(Guid UserId, string Email, bool IsMember, IReadOnlyCollection<string> Roles);
    private sealed record FichaContactoPayload(Guid Id, DateTime FechaAlta, string Nombre, string Telefono1, string? Telefono2, string? Email, string? Direccion, string? Observaciones);
    private sealed record SignalPayload(Guid Id, float Latitud, float Longitud, string Titulo, string Descripcion, bool Activo, Guid UserIdAlta, DateTime FechaAlta, DateTime FechaModificacion, Guid UserIdModificacion, int Tipo, string Tags);
    private sealed record SignalImagesPayload(Guid SignalId, byte[] Foto1, byte[] Foto2);
    private sealed record SignalCommentPayload(Guid Id, Guid SignalId, Guid UserId, DateTime FechaComentario, string Texto);
    private sealed record SignalTypePayload(int Id, string Nombre, string Icono);
    private sealed record WordPressPostSummaryPayload(long Id, string Slug, string Titulo, string Resumen, string? ImagenDestacadaUrl, DateTime FechaPublicacionUtc);
    private sealed record WordPressPostPayload(long Id, string Slug, string Titulo, string Resumen, string Contenido, string? ImagenDestacadaUrl, string Enlace, DateTime FechaPublicacionUtc);
}

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"indalo-api-tests-{Guid.NewGuid():N}";
    private readonly FakeEmailSender _fakeEmailSender = new();
    private readonly FakeWordPressService _fakeWordPressService = new();

    private string ConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public FakeEmailSender EmailSender => _fakeEmailSender;
    public int? LastWordPressListPageSize => _fakeWordPressService.LastPageSize;
    public string? LastWordPressListSearch => _fakeWordPressService.LastSearch;

    public void ResetWordPressService() => _fakeWordPressService.Reset();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:api_ContextConnection", ConnectionString);
        builder.UseSetting("ConnectionStrings:DefaultConnection", ConnectionString);
        builder.UseSetting("Testing:UseEnsureCreated", "true");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IQueryConnectionFactory>();

            var optionsConfigurationDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("IDbContextOptionsConfiguration") == true)
                .ToList();
            foreach (var descriptor in optionsConfigurationDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(ConnectionString));

            services.AddScoped<IQueryConnectionFactory>(_ => new SqlServerQueryConnectionFactory(ConnectionString));

            var socialValidatorDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISocialTokenValidator));
            if (socialValidatorDescriptor is not null)
            {
                services.Remove(socialValidatorDescriptor);
            }

            services.AddScoped<ISocialTokenValidator, FakeSocialTokenValidator>();

            var wordPressDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IWordPressService));
            if (wordPressDescriptor is not null)
            {
                services.Remove(wordPressDescriptor);
            }

            services.AddSingleton(_fakeWordPressService);
            services.AddSingleton<IWordPressService>(_fakeWordPressService);

            services.RemoveAll<IEmailSender>();
            services.AddSingleton(_fakeEmailSender);
            services.AddSingleton<IEmailSender>(_fakeEmailSender);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        try
        {
            using var connection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"""
                IF DB_ID(N'{_databaseName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_databaseName}];
                END
                """;
            command.ExecuteNonQuery();
        }
        catch
        {
            // Best effort cleanup for temporary integration-test databases.
        }
    }

    private sealed class FakeSocialTokenValidator : ISocialTokenValidator
    {
        public Task<SocialTokenValidationResult> ValidateAsync(string provider, string token, CancellationToken cancellationToken)
        {
            if (provider.Equals("google", StringComparison.OrdinalIgnoreCase) && token == "social-test-token")
            {
                return Task.FromResult(new SocialTokenValidationResult(
                    true,
                    "google",
                    "google-test-subject",
                    "social-user@club.test",
                    []));
            }

            return Task.FromResult(SocialTokenValidationResult.Failed("El token social no es valido."));
        }
    }

    private sealed class FakeWordPressService : IWordPressService
    {
        public int? LastPageSize { get; private set; }
        public string? LastSearch { get; private set; }

        public void Reset()
        {
            LastPageSize = null;
            LastSearch = null;
        }

        public Task<IReadOnlyCollection<WordPressPostSummaryDto>> GetPostsAsync(int page, int pageSize, string? search, CancellationToken cancellationToken)
        {
            LastPageSize = pageSize;
            LastSearch = search;

            IReadOnlyCollection<WordPressPostSummaryDto> items =
            [
                new WordPressPostSummaryDto(1, "ruta-de-prueba", "Ruta de prueba", "Resumen 1", "https://example.com/ruta-de-prueba.jpg", DateTime.UtcNow.AddDays(-1)),
                new WordPressPostSummaryDto(2, "material-recomendado", "Material recomendado", "Resumen 2", "https://example.com/material-recomendado.jpg", DateTime.UtcNow)
            ];
            return Task.FromResult(items);
        }

        public Task<WordPressPostDto> GetPostBySlugAsync(string slug, CancellationToken cancellationToken)
        {
            if (string.Equals(slug, "ruta-de-prueba", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new WordPressPostDto(
                    1,
                    "ruta-de-prueba",
                    "Ruta de prueba",
                    "Resumen 1",
                    "Contenido 1",
                    "https://example.com/ruta-de-prueba.jpg",
                    "https://example.com/ruta-de-prueba",
                    DateTime.UtcNow.AddDays(-1)));
            }

            throw new KeyNotFoundException("El post de WordPress no existe.");
        }
    }

    private sealed class SqlServerQueryConnectionFactory(string connectionString) : IQueryConnectionFactory
    {
        public async Task<System.Data.IDbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
    }
}

public sealed class FakeEmailSender : IEmailSender
{
    private readonly object _sync = new();
    private readonly List<EmailMessage> _messages = [];

    public IReadOnlyCollection<EmailMessage> Messages
    {
        get
        {
            lock (_sync)
            {
                return _messages.ToArray();
            }
        }
    }

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            _messages.Add(message);
        }

        return Task.CompletedTask;
    }

    public void Clear()
    {
        lock (_sync)
        {
            _messages.Clear();
        }
    }
}


