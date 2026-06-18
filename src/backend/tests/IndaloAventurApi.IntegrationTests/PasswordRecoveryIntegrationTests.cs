using System.Net;
using System.Net.Http.Json;
using IndaloAventurApi.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace IndaloAventurApi.IntegrationTests;

public sealed class PasswordRecoveryIntegrationTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private const string NeutralMessage = "Si existe una cuenta asociada al email indicado, te hemos enviado instrucciones para recuperar tu contrasena.";
    private const string SuccessMessage = "La contrasena se ha actualizado correctamente.";
    private readonly CustomWebApplicationFactory _factory = factory;
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly IServiceProvider _services = factory.Services;

    [Fact]
    public async Task PassRecovery_ShouldReturnNeutralResponse_AndSendEmail_WhenUserExists()
    {
        await EnsureRolesAndAdminAsync();
        var email = $"recovery-{Guid.NewGuid():N}@club.test";
        const string password = "Test1234A";
        await RegisterAsync(email, password);
        _factory.EmailSender.Clear();

        var response = await _httpClient.PostAsJsonAsync("/api/auth/passrecovery", new { Email = email });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PasswordRecoveryPayload>();
        Assert.NotNull(payload);
        Assert.Equal(NeutralMessage, payload!.Message);

        var sentMessage = Assert.Single(_factory.EmailSender.Messages);
        Assert.Equal(email, sentMessage.To);
        Assert.Contains("http://localhost:5173/reset-password", sentMessage.HtmlBody, StringComparison.Ordinal);
        Assert.Contains("Definir nueva contrasena", sentMessage.HtmlBody, StringComparison.Ordinal);

        var resetLink = ExtractResetLink(sentMessage.HtmlBody);
        var parameters = QueryHelpers.ParseQuery(resetLink.Query);
        Assert.Equal(email, parameters["email"].ToString());
        Assert.False(string.IsNullOrWhiteSpace(parameters["token"].ToString()));
    }

    [Fact]
    public async Task PassRecovery_ShouldReturnNeutralResponse_AndNotSendEmail_WhenUserDoesNotExist()
    {
        _factory.EmailSender.Clear();

        var response = await _httpClient.PostAsJsonAsync("/api/auth/passrecovery", new
        {
            Email = $"missing-{Guid.NewGuid():N}@club.test"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PasswordRecoveryPayload>();
        Assert.NotNull(payload);
        Assert.Equal(NeutralMessage, payload!.Message);
        Assert.Empty(_factory.EmailSender.Messages);
    }

    [Fact]
    public async Task ResetPassword_ShouldAllowLoginWithNewPassword_AfterValidRecoveryFlow()
    {
        await EnsureRolesAndAdminAsync();
        var email = $"reset-ok-{Guid.NewGuid():N}@club.test";
        const string oldPassword = "Test1234A";
        const string newPassword = "NuevaClave123A";
        await RegisterAsync(email, oldPassword);
        _factory.EmailSender.Clear();

        await _httpClient.PostAsJsonAsync("/api/auth/passrecovery", new { Email = email });
        var sentMessage = Assert.Single(_factory.EmailSender.Messages);
        var token = QueryHelpers.ParseQuery(ExtractResetLink(sentMessage.HtmlBody).Query)["token"].ToString();

        var resetResponse = await _httpClient.PostAsJsonAsync("/api/auth/reset-password", new
        {
            Email = email,
            Token = token,
            NewPassword = newPassword,
            ConfirmPassword = newPassword
        });

        Assert.Equal(HttpStatusCode.OK, resetResponse.StatusCode);
        var resetPayload = await resetResponse.Content.ReadFromJsonAsync<PasswordRecoveryPayload>();
        Assert.NotNull(resetPayload);
        Assert.Equal(SuccessMessage, resetPayload!.Message);

        var oldLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = oldPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, oldLoginResponse.StatusCode);

        var newLoginResponse = await _httpClient.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = newPassword });
        Assert.Equal(HttpStatusCode.OK, newLoginResponse.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnConflict_WhenTokenIsInvalid()
    {
        await EnsureRolesAndAdminAsync();
        var email = $"reset-invalid-token-{Guid.NewGuid():N}@club.test";
        await RegisterAsync(email, "Test1234A");

        var response = await _httpClient.PostAsJsonAsync("/api/auth/reset-password", new
        {
            Email = email,
            Token = "token-invalido",
            NewPassword = "NuevaClave123A",
            ConfirmPassword = "NuevaClave123A"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsPayload>();
        Assert.NotNull(problem);
        Assert.Contains("token", problem!.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenConfirmationDoesNotMatch()
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/reset-password", new
        {
            Email = $"reset-mismatch-{Guid.NewGuid():N}@club.test",
            Token = "abc",
            NewPassword = "NuevaClave123A",
            ConfirmPassword = "OtraClave123A"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetailsPayload>();
        Assert.NotNull(problem);
        Assert.Contains(
            problem!.Errors,
            error => string.Equals(error.ErrorMessage, "La confirmacion de la nueva contrasena no coincide.", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnConflict_WhenPasswordDoesNotMeetPolicy()
    {
        await EnsureRolesAndAdminAsync();
        var email = $"reset-policy-{Guid.NewGuid():N}@club.test";
        const string currentPassword = "Test1234A";
        await RegisterAsync(email, currentPassword);
        _factory.EmailSender.Clear();

        await _httpClient.PostAsJsonAsync("/api/auth/passrecovery", new { Email = email });
        var sentMessage = Assert.Single(_factory.EmailSender.Messages);
        var token = QueryHelpers.ParseQuery(ExtractResetLink(sentMessage.HtmlBody).Query)["token"].ToString();

        var response = await _httpClient.PostAsJsonAsync("/api/auth/reset-password", new
        {
            Email = email,
            Token = token,
            NewPassword = "corta",
            ConfirmPassword = "corta"
        });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsPayload>();
        Assert.NotNull(problem);
        Assert.Contains("contrasena", problem!.Detail, StringComparison.OrdinalIgnoreCase);
    }

    private async Task RegisterAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task EnsureRolesAndAdminAsync()
    {
        using var scope = _services.CreateScope();
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

    private static Uri ExtractResetLink(string htmlBody)
    {
        const string marker = "href=\"";
        var startIndex = htmlBody.IndexOf(marker, StringComparison.Ordinal);
        Assert.True(startIndex >= 0, "El correo no contiene enlace de recuperacion.");

        startIndex += marker.Length;
        var endIndex = htmlBody.IndexOf('"', startIndex);
        Assert.True(endIndex > startIndex, "El correo no contiene un href valido.");

        var url = WebUtility.HtmlDecode(htmlBody[startIndex..endIndex]);
        return new Uri(url, UriKind.Absolute);
    }

    private sealed record PasswordRecoveryPayload(string Message);
    private sealed record ProblemDetailsPayload(string Title, int Status, string Detail);
    private sealed record ValidationProblemDetailsPayload(string Title, int Status, ValidationErrorPayload[] Errors);
    private sealed record ValidationErrorPayload(string PropertyName, string ErrorMessage);
}
