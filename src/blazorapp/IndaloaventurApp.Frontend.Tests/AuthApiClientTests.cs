namespace IndaloaventurApp.Frontend.Tests;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.Web.Client.Infrastructure.Auth;

public sealed class AuthApiClientTests
{
    [Fact]
    public async Task LoginAsync_UsesPayloadIsMember_WhenJwtDoesNotContainClaim()
    {
        const string opaqueToken = "header.payload.signature";

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/auth/login", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadFromJsonAsync<LoginApiPayload>();
            Assert.NotNull(payload);
            Assert.Equal("member@club.test", payload.Email);
            Assert.Equal("Test1234A", payload.Password);

            var responseBody = JsonSerializer.Serialize(new
            {
                AccessToken = opaqueToken,
                TokenType = "Bearer",
                ExpiresInSeconds = 3600,
                IsMember = false
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AuthApiClient(httpClient);

        var result = await sut.LoginAsync(new LoginRequest("member@club.test", "Test1234A"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.False(result.Value.IsMember);
    }

    [Fact]
    public async Task LoginSocialAsync_PostsProviderAndToken_AndMapsSession()
    {
        var userId = Guid.Parse("4f2c2955-159b-46fd-a93a-a97789b03f92");
        var jwt = TestJwtFactory.Create(new Dictionary<string, object?>
        {
            ["IsMember"] = true,
            ["sub"] = userId
        });

        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/auth/social-login", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadFromJsonAsync<SocialLoginRequest>();
            Assert.NotNull(payload);
            Assert.Equal("google", payload.Provider);
            Assert.Equal("valid-id-token", payload.Token);

            var responseBody = JsonSerializer.Serialize(new
            {
                AccessToken = jwt,
                TokenType = "Bearer",
                ExpiresInSeconds = 3600,
                IsMember = false
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AuthApiClient(httpClient);

        var result = await sut.LoginSocialAsync(new SocialLoginRequest("google", "valid-id-token"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(jwt, result.Value.AccessToken);
        Assert.Equal("Bearer", result.Value.TokenType);
        Assert.Equal(3600, result.Value.ExpiresInSeconds);
        Assert.True(result.Value.IsMember);
        Assert.Equal(userId, result.Value.UserId);
    }

    [Fact]
    public async Task LoginSocialAsync_ReturnsInvalidSocialError_OnUnauthorized()
    {
        var handler = new StubHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)));

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AuthApiClient(httpClient);

        var result = await sut.LoginSocialAsync(new SocialLoginRequest("google", "bad-id-token"));

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("auth.social_invalid", result.Error.Code);
    }

    [Fact]
    public async Task LoginSocialAsync_ReadsAdminRoleFromJwtClaims()
    {
        var jwt = TestJwtFactory.Create(new Dictionary<string, object?>
        {
            ["IsMember"] = true,
            ["roles"] = new[] { "Admin", "Editor" }
        });

        var handler = new StubHttpMessageHandler(_ =>
        {
            var responseBody = JsonSerializer.Serialize(new
            {
                AccessToken = jwt,
                TokenType = "Bearer",
                ExpiresInSeconds = 3600,
                IsMember = true
            });

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AuthApiClient(httpClient);

        var result = await sut.LoginSocialAsync(new SocialLoginRequest("google", "valid-id-token"));

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.IsInRole("Admin"));
        Assert.Contains("Editor", result.Value.Roles);
    }

    [Fact]
    public async Task RequestPasswordRecoveryAsync_PostsEmail_AndReturnsNeutralMessage()
    {
        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("/api/auth/passrecovery", request.RequestUri?.AbsolutePath);

            var payload = await request.Content!.ReadFromJsonAsync<PasswordRecoveryPayload>();
            Assert.NotNull(payload);
            Assert.Equal("member@club.test", payload.Email);

            var responseBody = JsonSerializer.Serialize(new
            {
                Message = "Si existe una cuenta asociada al email indicado, te hemos enviado instrucciones para recuperar tu contraseña."
            });

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AuthApiClient(httpClient);

        var result = await sut.RequestPasswordRecoveryAsync(new PasswordRecoveryRequest("member@club.test"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Si existe una cuenta asociada al email indicado, te hemos enviado instrucciones para recuperar tu contraseña.", result.Value);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsBackendProblemDetail_OnConflict()
    {
        var handler = new StubHttpMessageHandler(_ =>
        {
            var responseBody = JsonSerializer.Serialize(new
            {
                Title = "Conflicto",
                Status = 409,
                Detail = "El token ha expirado o no es válido."
            });

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://localhost")
        };

        var sut = new AuthApiClient(httpClient);

        var result = await sut.ResetPasswordAsync(new ResetPasswordRequest("member@club.test", "token", "Nueva1234A", "Nueva1234A"));

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal("El token ha expirado o no es válido.", result.Error.Message);
    }

    private sealed record LoginApiPayload(string Email, string Password);
    private sealed record PasswordRecoveryPayload(string Email);
}
