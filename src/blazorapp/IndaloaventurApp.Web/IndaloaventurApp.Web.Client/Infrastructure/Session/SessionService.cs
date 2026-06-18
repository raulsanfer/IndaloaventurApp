namespace IndaloaventurApp.Web.Client.Infrastructure.Session;

using System.Text.Json;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Auth;
using Microsoft.JSInterop;

public sealed class SessionService(IJSRuntime jsRuntime) : ISessionService
{
    private const string LocalStorageKind = "local";
    private const string SessionStorageKind = "session";
    private const string StorageKey = "indalo.auth.session";

    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private Task? initializationTask;

    public bool IsInitialized { get; private set; }

    public bool IsAuthenticated => CurrentSession is not null;

    public AuthSession? CurrentSession { get; private set; }

    public Task EnsureInitializedAsync()
    {
        if (IsInitialized)
        {
            return Task.CompletedTask;
        }

        initializationTask ??= EnsureInitializedCoreAsync();
        return initializationTask;
    }

    public async Task SetSessionAsync(AuthSession session, bool rememberMe)
    {
        ArgumentNullException.ThrowIfNull(session);

        var nowUtc = DateTimeOffset.UtcNow;
        var snapshot = PersistedSessionSnapshot.Create(session, rememberMe, nowUtc);
        CurrentSession = session;

        await RemovePersistedSessionAsync(rememberMe ? SessionStorageKind : LocalStorageKind);
        await WritePersistedSessionAsync(rememberMe ? LocalStorageKind : SessionStorageKind, snapshot);

        IsInitialized = true;
        initializationTask = Task.CompletedTask;
    }

    public async Task SignOutAsync()
    {
        CurrentSession = null;

        await RemovePersistedSessionAsync(LocalStorageKind);
        await RemovePersistedSessionAsync(SessionStorageKind);

        IsInitialized = true;
        initializationTask = Task.CompletedTask;
    }

    private async Task EnsureInitializedCoreAsync()
    {
        try
        {
            var nowUtc = DateTimeOffset.UtcNow;
            var validSnapshots = new List<PersistedSessionSnapshot>();

            await CollectSnapshotAsync(SessionStorageKind, nowUtc, validSnapshots);
            await CollectSnapshotAsync(LocalStorageKind, nowUtc, validSnapshots);

            CurrentSession = validSnapshots
                .OrderByDescending(static snapshot => snapshot.PersistedAtUtc)
                .Select(snapshot => snapshot.ToAuthSession(nowUtc))
                .FirstOrDefault();
        }
        finally
        {
            IsInitialized = true;
        }
    }

    private async Task CollectSnapshotAsync(
        string storageKind,
        DateTimeOffset nowUtc,
        ICollection<PersistedSessionSnapshot> validSnapshots)
    {
        var snapshot = await ReadPersistedSessionAsync(storageKind);
        if (snapshot is null)
        {
            return;
        }

        if (snapshot.IsExpired(nowUtc))
        {
            await RemovePersistedSessionAsync(storageKind);
            return;
        }

        validSnapshots.Add(snapshot);
    }

    private async Task<PersistedSessionSnapshot?> ReadPersistedSessionAsync(string storageKind)
    {
        try
        {
            var json = await jsRuntime.InvokeAsync<string?>("indaloSessionStorage.read", storageKind, StorageKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<PersistedSessionSnapshot>(json, SerializerOptions);
        }
        catch (JsonException)
        {
            await RemovePersistedSessionAsync(storageKind);
            return null;
        }
        catch (JSException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    private async Task WritePersistedSessionAsync(string storageKind, PersistedSessionSnapshot snapshot)
    {
        try
        {
            var json = JsonSerializer.Serialize(snapshot, SerializerOptions);
            await jsRuntime.InvokeVoidAsync("indaloSessionStorage.write", storageKind, StorageKey, json);
        }
        catch (JSException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }

    private async Task RemovePersistedSessionAsync(string storageKind)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("indaloSessionStorage.remove", storageKind, StorageKey);
        }
        catch (JSException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }
}
