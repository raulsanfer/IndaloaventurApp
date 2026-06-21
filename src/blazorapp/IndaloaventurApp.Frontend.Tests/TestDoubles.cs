namespace IndaloaventurApp.Frontend.Tests;

using System.Collections;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bunit;
using IndaloaventurApp.SharedUI.Abstractions.Auth;
using IndaloaventurApp.SharedUI.Abstractions.Cargos;
using IndaloaventurApp.SharedUI.Abstractions.FoodAlerts;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Member;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.WordPress;
using IndaloaventurApp.SharedUI.Models.Auth;
using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.FoodAlerts;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Models.Member;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.SharedUI.Models.WordPress;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

internal static class ServiceCollectionExtensions
{
    public static void AddSingletonAuthDependencies(
        this IServiceCollection services,
        IAuthService authService,
        ISessionService sessionService,
        string clientId)
    {
        services.AddSingleton(authService);
        services.AddSingleton(sessionService);
        services.AddSingleton<IStringLocalizer<SharedTexts>, TestStringLocalizer<SharedTexts>>();
        services.AddSingleton(new GoogleAuthOptions(clientId));
    }
}

internal sealed class RecordingAuthService : IAuthService
{
    public Func<LoginRequest, Task<ServiceResult<AuthSession>>>? LoginHandler { get; init; }

    public Func<SocialLoginRequest, Task<ServiceResult<AuthSession>>>? SocialLoginHandler { get; init; }

    public Func<PasswordRecoveryRequest, Task<ServiceResult<string>>>? PasswordRecoveryHandler { get; init; }

    public Func<ResetPasswordRequest, Task<ServiceResult<string>>>? ResetPasswordHandler { get; init; }

    public LoginRequest? LastLoginRequest { get; private set; }

    public SocialLoginRequest? LastSocialLoginRequest { get; private set; }

    public PasswordRecoveryRequest? LastPasswordRecoveryRequest { get; private set; }

    public ResetPasswordRequest? LastResetPasswordRequest { get; private set; }

    public Task<ServiceResult<AuthSession>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        LastLoginRequest = request;
        return LoginHandler?.Invoke(request)
            ?? Task.FromResult(ServiceResult<AuthSession>.Failure(new ServiceError("auth.unavailable", "Missing login handler")));
    }

    public Task<ServiceResult<AuthSession>> LoginSocialAsync(SocialLoginRequest request, CancellationToken cancellationToken = default)
    {
        LastSocialLoginRequest = request;
        return SocialLoginHandler?.Invoke(request)
            ?? Task.FromResult(ServiceResult<AuthSession>.Failure(new ServiceError("auth.unavailable", "Missing social handler")));
    }

    public Task<ServiceResult<string>> RequestPasswordRecoveryAsync(PasswordRecoveryRequest request, CancellationToken cancellationToken = default)
    {
        LastPasswordRecoveryRequest = request;
        return PasswordRecoveryHandler?.Invoke(request)
            ?? Task.FromResult(ServiceResult<string>.Failure(new ServiceError("auth.unavailable", "Missing password recovery handler")));
    }

    public Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        LastResetPasswordRequest = request;
        return ResetPasswordHandler?.Invoke(request)
            ?? Task.FromResult(ServiceResult<string>.Failure(new ServiceError("auth.unavailable", "Missing reset password handler")));
    }
}

internal sealed class RecordingSessionService : ISessionService
{
    public Func<Task>? EnsureInitializedHandler { get; set; }

    public bool IsInitialized { get; set; } = true;

    public bool IsAuthenticated => CurrentSession is not null;

    public AuthSession? CurrentSession { get; private set; }

    public bool? LastRememberMe { get; private set; }

    public int EnsureInitializedCallCount { get; private set; }

    public int SignOutCallCount { get; private set; }

    public void SetSession(AuthSession session)
    {
        CurrentSession = session;
        IsInitialized = true;
    }

    public Task EnsureInitializedAsync()
    {
        EnsureInitializedCallCount++;
        return EnsureInitializedHandler?.Invoke() ?? Task.CompletedTask;
    }

    public Task SetSessionAsync(AuthSession session, bool rememberMe)
    {
        LastRememberMe = rememberMe;
        SetSession(session);
        return Task.CompletedTask;
    }

    public Task SignOutAsync()
    {
        SignOutCallCount++;
        CurrentSession = null;
        return Task.CompletedTask;
    }
}

internal sealed class RecordingMemberProfileService : IMemberProfileService
{
    public Func<CancellationToken, Task<ServiceResult<MemberProfile>>>? GetMyProfileHandler { get; init; }

    public Func<CancellationToken, Task<ServiceResult<MemberSelfProfile>>>? GetMyMemberFileHandler { get; init; }

    public Func<UpdateMemberSelfProfileRequest, CancellationToken, Task<ServiceResult<MemberSelfProfile>>>? UpdateMyMemberFileHandler { get; init; }

    public Task<ServiceResult<MemberProfile>> GetMyProfileAsync(CancellationToken cancellationToken = default)
    {
        return GetMyProfileHandler?.Invoke(cancellationToken)
            ?? Task.FromResult(ServiceResult<MemberProfile>.Failure(new ServiceError("profile.unavailable", "Missing profile handler")));
    }

    public Task<ServiceResult<MemberSelfProfile>> GetMyMemberFileAsync(CancellationToken cancellationToken = default)
    {
        return GetMyMemberFileHandler?.Invoke(cancellationToken)
            ?? Task.FromResult(ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "Missing member file handler")));
    }

    public Task<ServiceResult<MemberSelfProfile>> UpdateMyMemberFileAsync(UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default)
    {
        return UpdateMyMemberFileHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "Missing member file update handler")));
    }
}

internal sealed class RecordingFederativeLicenseService : IFederativeLicenseService
{
    public Func<CancellationToken, Task<ServiceResult<IReadOnlyList<FederativeLicenseRequest>>>>? GetMyFederativeLicensesHandler { get; init; }

    public Func<int, bool, CancellationToken, Task<ServiceResult<IReadOnlyList<FederativeLicenseRate>>>>? GetAvailableRatesHandler { get; init; }

    public Func<CreateFederativeLicenseRequest, CancellationToken, Task<ServiceResult<FederativeLicenseRequest>>>? CreateFederativeLicenseRequestHandler { get; init; }

    public CreateFederativeLicenseRequest? LastCreateRequest { get; private set; }

    public Task<ServiceResult<IReadOnlyList<FederativeLicenseRequest>>> GetMyFederativeLicensesAsync(CancellationToken cancellationToken = default)
    {
        return GetMyFederativeLicensesHandler?.Invoke(cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRequest>>.Success(Array.Empty<FederativeLicenseRequest>()));
    }

    public Task<ServiceResult<IReadOnlyList<FederativeLicenseRate>>> GetAvailableRatesAsync(int temporada, bool mediaTemporada = false, CancellationToken cancellationToken = default)
    {
        return GetAvailableRatesHandler?.Invoke(temporada, mediaTemporada, cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<FederativeLicenseRate>>.Success(Array.Empty<FederativeLicenseRate>()));
    }

    public Task<ServiceResult<FederativeLicenseRequest>> CreateFederativeLicenseRequestAsync(CreateFederativeLicenseRequest request, CancellationToken cancellationToken = default)
    {
        LastCreateRequest = request;
        return CreateFederativeLicenseRequestHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<FederativeLicenseRequest>.Failure(new ServiceError("licenses.request_missing_handler", "Missing create federative license request handler")));
    }
}

internal sealed class RecordingAdminFederativeLicenseService : IAdminFederativeLicenseService
{
    public Func<AdminFederativeLicenseQuery, CancellationToken, Task<ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>>>? GetFederativeLicensesHandler { get; init; }

    public Func<UpdateAdminFederativeLicenseStatusRequest, CancellationToken, Task<ServiceResult<AdminFederativeLicenseRequest>>>? UpdateFederativeLicenseStatusHandler { get; init; }

    public AdminFederativeLicenseQuery? LastQuery { get; private set; }

    public UpdateAdminFederativeLicenseStatusRequest? LastUpdateRequest { get; private set; }

    public Task<ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>> GetFederativeLicensesAsync(
        AdminFederativeLicenseQuery query,
        CancellationToken cancellationToken = default)
    {
        LastQuery = query;
        return GetFederativeLicensesHandler?.Invoke(query, cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>.Success(Array.Empty<AdminFederativeLicenseRequest>()));
    }

    public Task<ServiceResult<AdminFederativeLicenseRequest>> UpdateFederativeLicenseStatusAsync(
        UpdateAdminFederativeLicenseStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        LastUpdateRequest = request;
        return UpdateFederativeLicenseStatusHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<AdminFederativeLicenseRequest>.Failure(new ServiceError("licenses.admin_missing_handler", "Missing admin federative license update handler")));
    }
}

internal sealed class RecordingSignalService : ISignalService
{
    public Func<SignalListQuery, CancellationToken, Task<ServiceResult<SignalHomeData>>>? GetSignalHomeDataHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<SignalDetailItem>>>? GetSignalHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<SignalImagesItem>>>? GetSignalImagesHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<IReadOnlyList<SignalCommentItem>>>>? GetSignalCommentsHandler { get; init; }

    public Func<CancellationToken, Task<ServiceResult<IReadOnlyList<SignalCategoryItem>>>>? GetSignalCategoriesHandler { get; init; }

    public Func<CreateSignalCategoryRequest, CancellationToken, Task<ServiceResult<int>>>? CreateSignalCategoryHandler { get; init; }

    public Func<UpdateSignalCategoryRequest, CancellationToken, Task<ServiceResult<bool>>>? UpdateSignalCategoryHandler { get; init; }

    public Func<int, CancellationToken, Task<ServiceResult<bool>>>? DeleteSignalCategoryHandler { get; init; }

    public Func<SignalCreateRequest, CancellationToken, Task<ServiceResult<Guid>>>? CreateSignalHandler { get; init; }

    public Func<UpdateSignalRequest, CancellationToken, Task<ServiceResult<bool>>>? UpdateSignalHandler { get; init; }

    public Task<ServiceResult<SignalHomeData>> GetSignalHomeDataAsync(SignalListQuery query, CancellationToken cancellationToken = default)
    {
        return GetSignalHomeDataHandler?.Invoke(query, cancellationToken)
            ?? Task.FromResult(ServiceResult<SignalHomeData>.Success(new SignalHomeData(Array.Empty<SignalCategoryItem>(), Array.Empty<SignalCardItem>())));
    }

    public Task<ServiceResult<SignalDetailItem>> GetSignalAsync(Guid signalId, CancellationToken cancellationToken = default)
    {
        return GetSignalHandler?.Invoke(signalId, cancellationToken)
            ?? Task.FromResult(ServiceResult<SignalDetailItem>.Failure(new ServiceError("signals.not_found", "Missing signal detail handler")));
    }

    public Task<ServiceResult<SignalImagesItem>> GetSignalImagesAsync(Guid signalId, CancellationToken cancellationToken = default)
    {
        return GetSignalImagesHandler?.Invoke(signalId, cancellationToken)
            ?? Task.FromResult(ServiceResult<SignalImagesItem>.Success(new SignalImagesItem(signalId, null, null)));
    }

    public Task<ServiceResult<IReadOnlyList<SignalCommentItem>>> GetSignalCommentsAsync(Guid signalId, CancellationToken cancellationToken = default)
    {
        return GetSignalCommentsHandler?.Invoke(signalId, cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<SignalCommentItem>>.Success(Array.Empty<SignalCommentItem>()));
    }

    public Task<ServiceResult<IReadOnlyList<SignalCategoryItem>>> GetSignalCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return GetSignalCategoriesHandler?.Invoke(cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<SignalCategoryItem>>.Success(Array.Empty<SignalCategoryItem>()));
    }

    public Task<ServiceResult<int>> CreateSignalCategoryAsync(CreateSignalCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return CreateSignalCategoryHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<int>.Failure(new ServiceError("signals.categories.create_missing_handler", "Missing signal category create handler")));
    }

    public Task<ServiceResult<bool>> UpdateSignalCategoryAsync(UpdateSignalCategoryRequest request, CancellationToken cancellationToken = default)
    {
        return UpdateSignalCategoryHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("signals.categories.update_missing_handler", "Missing signal category update handler")));
    }

    public Task<ServiceResult<bool>> DeleteSignalCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteSignalCategoryHandler?.Invoke(id, cancellationToken)
            ?? Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("signals.categories.delete_missing_handler", "Missing signal category delete handler")));
    }

    public Task<ServiceResult<Guid>> CreateSignalAsync(SignalCreateRequest request, CancellationToken cancellationToken = default)
    {
        return CreateSignalHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<Guid>.Failure(new ServiceError("signals.create_missing_handler", "Missing signal create handler")));
    }

    public Task<ServiceResult<bool>> UpdateSignalAsync(UpdateSignalRequest request, CancellationToken cancellationToken = default)
    {
        return UpdateSignalHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("signals.update_missing_handler", "Missing signal update handler")));
    }
}

internal sealed class RecordingFoodAlertService : IFoodAlertService
{
    public Func<string, CancellationToken, Task<ServiceResult<IReadOnlyList<FoodAlertListItem>>>>? GetAlertsHandler { get; init; }

    public Func<string, CancellationToken, Task<ServiceResult<FoodAlertDetailItem>>>? GetAlertHandler { get; init; }

    public Task<ServiceResult<IReadOnlyList<FoodAlertListItem>>> GetAlertsAsync(string categoryCode, CancellationToken cancellationToken = default)
    {
        return GetAlertsHandler?.Invoke(categoryCode, cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<FoodAlertListItem>>.Success(Array.Empty<FoodAlertListItem>()));
    }

    public Task<ServiceResult<FoodAlertDetailItem>> GetAlertAsync(string alertId, CancellationToken cancellationToken = default)
    {
        return GetAlertHandler?.Invoke(alertId, cancellationToken)
            ?? Task.FromResult(ServiceResult<FoodAlertDetailItem>.Failure(new ServiceError("food_alerts.not_found", "Missing food alert detail handler")));
    }
}

internal sealed class RecordingWordPressPostService : IWordPressPostService
{
    public Func<CancellationToken, Task<ServiceResult<IReadOnlyList<WordPressPost>>>>? GetLatestPostsHandler { get; init; }

    public Func<string, CancellationToken, Task<ServiceResult<WordPressPost>>>? GetPostBySlugHandler { get; init; }

    public Task<ServiceResult<IReadOnlyList<WordPressPost>>> GetLatestPostsAsync(CancellationToken cancellationToken = default)
    {
        return GetLatestPostsHandler?.Invoke(cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<WordPressPost>>.Success(Array.Empty<WordPressPost>()));
    }

    public Task<ServiceResult<WordPressPost>> GetPostBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return GetPostBySlugHandler?.Invoke(slug, cancellationToken)
            ?? Task.FromResult(ServiceResult<WordPressPost>.Failure(new ServiceError("wordpress.not_found", "Missing WordPress post handler")));
    }
}

internal sealed class RecordingAdminUserManagementService : IAdminUserManagementService
{
    public Func<string?, CancellationToken, Task<ServiceResult<IReadOnlyList<ManagedUserItem>>>>? GetUsersHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<ManagedUserItem>>>? GetUserHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<MemberSelfProfile>>>? GetMemberFileHandler { get; init; }

    public Func<Guid, string, CancellationToken, Task<ServiceResult<MemberSelfProfile>>>? CreateMemberFileHandler { get; init; }

    public Func<Guid, UpdateMemberSelfProfileRequest, CancellationToken, Task<ServiceResult<MemberSelfProfile>>>? UpdateMemberFileHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<bool>>>? DeactivateUserHandler { get; init; }

    public Func<Guid, CancellationToken, Task<ServiceResult<bool>>>? ReactivateUserHandler { get; init; }

    public Task<ServiceResult<IReadOnlyList<ManagedUserItem>>> GetUsersAsync(string? email = null, CancellationToken cancellationToken = default)
    {
        return GetUsersHandler?.Invoke(email, cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<ManagedUserItem>>.Success(Array.Empty<ManagedUserItem>()));
    }

    public Task<ServiceResult<ManagedUserItem>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return GetUserHandler?.Invoke(userId, cancellationToken)
            ?? Task.FromResult(ServiceResult<ManagedUserItem>.Failure(new ServiceError("users.not_found", "Missing admin user handler")));
    }

    public Task<ServiceResult<MemberSelfProfile>> GetMemberFileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return GetMemberFileHandler?.Invoke(userId, cancellationToken)
            ?? Task.FromResult(ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "Missing admin member file handler")));
    }

    public Task<ServiceResult<MemberSelfProfile>> CreateMemberFileAsync(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        return CreateMemberFileHandler?.Invoke(userId, email, cancellationToken)
            ?? Task.FromResult(ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "Missing admin create member file handler")));
    }

    public Task<ServiceResult<MemberSelfProfile>> UpdateMemberFileAsync(Guid userId, UpdateMemberSelfProfileRequest request, CancellationToken cancellationToken = default)
    {
        return UpdateMemberFileHandler?.Invoke(userId, request, cancellationToken)
            ?? Task.FromResult(ServiceResult<MemberSelfProfile>.Failure(new ServiceError("profile.unavailable", "Missing admin update member file handler")));
    }

    public Task<ServiceResult<bool>> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DeactivateUserHandler?.Invoke(userId, cancellationToken)
            ?? Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("users.unavailable", "Missing admin deactivate handler")));
    }

    public Task<ServiceResult<bool>> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return ReactivateUserHandler?.Invoke(userId, cancellationToken)
            ?? Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("users.unavailable", "Missing admin reactivate handler")));
    }
}

internal sealed class RecordingCargoAdminService : ICargoAdminService
{
    public Func<CancellationToken, Task<ServiceResult<IReadOnlyList<CargoItem>>>>? GetCargosHandler { get; init; }

    public Func<CreateCargoRequest, CancellationToken, Task<ServiceResult<CargoItem>>>? CreateCargoHandler { get; init; }

    public Func<UpdateCargoRequest, CancellationToken, Task<ServiceResult<CargoItem>>>? UpdateCargoHandler { get; init; }

    public Func<int, CancellationToken, Task<ServiceResult<bool>>>? DeleteCargoHandler { get; init; }

    public Task<ServiceResult<IReadOnlyList<CargoItem>>> GetCargosAsync(CancellationToken cancellationToken = default)
    {
        return GetCargosHandler?.Invoke(cancellationToken)
            ?? Task.FromResult(ServiceResult<IReadOnlyList<CargoItem>>.Success(Array.Empty<CargoItem>()));
    }

    public Task<ServiceResult<CargoItem>> CreateCargoAsync(CreateCargoRequest request, CancellationToken cancellationToken = default)
    {
        return CreateCargoHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<CargoItem>.Failure(new ServiceError("cargos.unavailable", "Missing create handler")));
    }

    public Task<ServiceResult<CargoItem>> UpdateCargoAsync(UpdateCargoRequest request, CancellationToken cancellationToken = default)
    {
        return UpdateCargoHandler?.Invoke(request, cancellationToken)
            ?? Task.FromResult(ServiceResult<CargoItem>.Failure(new ServiceError("cargos.unavailable", "Missing update handler")));
    }

    public Task<ServiceResult<bool>> DeleteCargoAsync(int id, CancellationToken cancellationToken = default)
    {
        return DeleteCargoHandler?.Invoke(id, cancellationToken)
            ?? Task.FromResult(ServiceResult<bool>.Failure(new ServiceError("cargos.unavailable", "Missing delete handler")));
    }
}

internal sealed class TestStringLocalizer<T> : IStringLocalizer<T>
{
    public LocalizedString this[string name] => new(name, name, resourceNotFound: true);

    public LocalizedString this[string name, params object[] arguments]
        => new(name, string.Format(CultureInfo.InvariantCulture, name, arguments), resourceNotFound: true);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => Array.Empty<LocalizedString>();

    public IStringLocalizer WithCulture(CultureInfo culture)
        => this;
}

internal static class TestSessions
{
    public static readonly AuthSession MemberSession = new(
        "access-token",
        "Bearer",
        3600,
        true,
        userId: Guid.Parse("5f099de7-2b37-4237-8e16-f48d31a56267"));

    public static readonly AuthSession AdminSession = new(
        "admin-token",
        "Bearer",
        3600,
        true,
        new[] { "Admin" },
        Guid.Parse("6b46bb4c-9bff-43b9-b31d-5098b76f115e"));
}

internal sealed class StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => handler(request);
}

internal static class TestJwtFactory
{
    public static string Create(IReadOnlyDictionary<string, object?> payload)
    {
        var header = Encode(new Dictionary<string, object?>
        {
            ["alg"] = "none",
            ["typ"] = "JWT"
        });

        var body = Encode(payload);
        return $"{header}.{body}.signature";
    }

    private static string Encode(IReadOnlyDictionary<string, object?> value)
    {
        var json = JsonSerializer.Serialize(value);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
