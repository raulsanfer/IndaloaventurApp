namespace IndaloaventurApp.Frontend.Tests;

using System.Text.Json;
using Bunit;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.Web.Client.Infrastructure.Session;

public sealed class SessionServiceTests : BunitContext
{
    [Fact]
    public async Task SetSessionAsync_PersistsToSessionStorage_WhenRememberMeIsDisabled()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var service = new SessionService(JSInterop.JSRuntime);

        await service.SetSessionAsync(TestSessions.MemberSession, rememberMe: false);

        Assert.True(service.IsInitialized);
        Assert.True(service.IsAuthenticated);

        var writeInvocation = Assert.Single(JSInterop.Invocations, invocation =>
            invocation.Identifier == "indaloSessionStorage.write");

        Assert.Equal("session", writeInvocation.Arguments[0]?.ToString());
        Assert.Equal("indalo.auth.session", writeInvocation.Arguments[1]?.ToString());

        using var json = JsonDocument.Parse(writeInvocation.Arguments[2]?.ToString() ?? "{}");
        Assert.False(json.RootElement.GetProperty("rememberMe").GetBoolean());

        Assert.Contains(JSInterop.Invocations, invocation =>
            invocation.Identifier == "indaloSessionStorage.remove" &&
            string.Equals(invocation.Arguments[0]?.ToString(), "local", StringComparison.Ordinal));
    }

    [Fact]
    public async Task EnsureInitializedAsync_RestoresValidSessionFromLocalStorage()
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var localSnapshotJson = CreateSnapshotJson(TestSessions.AdminSession, rememberMe: true, nowUtc.AddMinutes(-5), nowUtc.AddMinutes(55));

        JSInterop.Setup<string?>("indaloSessionStorage.read", invocation =>
                invocation.Arguments[0]?.ToString() == "session")
            .SetResult((string?)null);

        JSInterop.Setup<string?>("indaloSessionStorage.read", invocation =>
                invocation.Arguments[0]?.ToString() == "local")
            .SetResult(localSnapshotJson);

        JSInterop.Mode = JSRuntimeMode.Loose;

        var service = new SessionService(JSInterop.JSRuntime);

        await service.EnsureInitializedAsync();

        Assert.True(service.IsInitialized);
        Assert.True(service.IsAuthenticated);
        Assert.NotNull(service.CurrentSession);
        Assert.Equal(TestSessions.AdminSession.AccessToken, service.CurrentSession!.AccessToken);
        Assert.True(service.CurrentSession.IsInRole("Admin"));
        Assert.Equal(TestSessions.AdminSession.UserId, service.CurrentSession.UserId);
    }

    [Fact]
    public async Task EnsureInitializedAsync_RemovesExpiredPersistedSessions()
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var expiredSnapshotJson = CreateSnapshotJson(TestSessions.MemberSession, rememberMe: false, nowUtc.AddHours(-2), nowUtc.AddMinutes(-1));

        JSInterop.Setup<string?>("indaloSessionStorage.read", invocation => true)
            .SetResult(expiredSnapshotJson);

        JSInterop.Mode = JSRuntimeMode.Loose;

        var service = new SessionService(JSInterop.JSRuntime);

        await service.EnsureInitializedAsync();

        Assert.True(service.IsInitialized);
        Assert.False(service.IsAuthenticated);
        Assert.Null(service.CurrentSession);

        Assert.Equal(2, JSInterop.Invocations.Count(invocation =>
            invocation.Identifier == "indaloSessionStorage.remove"));
    }

    [Fact]
    public async Task SignOutAsync_RemovesPersistedSessionsFromBothStorages()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var service = new SessionService(JSInterop.JSRuntime);

        await service.SignOutAsync();

        Assert.True(service.IsInitialized);
        Assert.False(service.IsAuthenticated);

        Assert.Contains(JSInterop.Invocations, invocation =>
            invocation.Identifier == "indaloSessionStorage.remove" &&
            string.Equals(invocation.Arguments[0]?.ToString(), "local", StringComparison.Ordinal));

        Assert.Contains(JSInterop.Invocations, invocation =>
            invocation.Identifier == "indaloSessionStorage.remove" &&
            string.Equals(invocation.Arguments[0]?.ToString(), "session", StringComparison.Ordinal));
    }

    private static string CreateSnapshotJson(
        AuthSession session,
        bool rememberMe,
        DateTimeOffset persistedAtUtc,
        DateTimeOffset expiresAtUtc)
    {
        return JsonSerializer.Serialize(new
        {
            accessToken = session.AccessToken,
            tokenType = session.TokenType,
            expiresInSeconds = session.ExpiresInSeconds,
            isMember = session.IsMember,
            userId = session.UserId,
            roles = session.Roles,
            persistedAtUtc,
            expiresAtUtc,
            rememberMe
        });
    }
}
