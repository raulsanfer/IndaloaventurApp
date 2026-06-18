namespace IndaloaventurApp.SharedUI.Components.Settings;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class AdminFederativeLicenseManagementView
{
    private static readonly string[] SupportedStatuses = ["Pendiente", "Confirmada", "Cancelada"];

    [Inject]
    private IAdminFederativeLicenseService AdminFederativeLicenseService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Parameter]
    public Guid? InitialUserId { get; set; }

    protected IReadOnlyList<AdminFederativeLicenseRequest> Requests { get; private set; } = Array.Empty<AdminFederativeLicenseRequest>();

    protected IReadOnlyList<string> AllowedStatuses => SupportedStatuses;

    protected string UserIdFilterText { get; private set; } = string.Empty;

    protected string SeasonFilterText { get; private set; } = string.Empty;

    protected string StatusFilter { get; private set; } = string.Empty;

    protected bool IsLoading { get; private set; } = true;

    protected Guid? UpdatingRequestId { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    private readonly Dictionary<Guid, string> _draftStatuses = [];
    private AdminFederativeLicenseQuery _currentQuery = new();

    protected override async Task OnInitializedAsync()
    {
        ApplyInitialFilters();
        var initialQuery = BuildCurrentQuery();
        if (initialQuery is null)
        {
            IsLoading = false;
            return;
        }

        await LoadAsync(initialQuery);
    }

    protected void HandleUserIdFilterChanged(ChangeEventArgs args)
    {
        UserIdFilterText = args.Value?.ToString()?.Trim() ?? string.Empty;
    }

    protected void HandleSeasonFilterChanged(ChangeEventArgs args)
    {
        SeasonFilterText = args.Value?.ToString()?.Trim() ?? string.Empty;
    }

    protected void HandleStatusFilterChanged(ChangeEventArgs args)
    {
        StatusFilter = args.Value?.ToString()?.Trim() ?? string.Empty;
    }

    protected async Task HandleFiltersSubmitAsync()
    {
        var query = BuildCurrentQuery();
        if (query is null)
        {
            return;
        }

        await LoadAsync(query);
    }

    protected async Task ClearFiltersAsync()
    {
        UserIdFilterText = string.Empty;
        SeasonFilterText = string.Empty;
        StatusFilter = string.Empty;
        ErrorMessageKey = null;

        await LoadAsync(new AdminFederativeLicenseQuery());
    }

    protected void HandleDraftStatusChanged(Guid requestId, ChangeEventArgs args)
    {
        _draftStatuses[requestId] = args.Value?.ToString()?.Trim() ?? string.Empty;
    }

    protected string GetDraftStatus(AdminFederativeLicenseRequest request)
    {
        return _draftStatuses.TryGetValue(request.Id, out var status) && !string.IsNullOrWhiteSpace(status)
            ? status
            : request.Estado;
    }

    protected bool CanSaveStatus(AdminFederativeLicenseRequest request)
    {
        if (UpdatingRequestId.HasValue || !_draftStatuses.TryGetValue(request.Id, out var draftStatus))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(draftStatus) &&
            SupportedStatuses.Contains(draftStatus, StringComparer.OrdinalIgnoreCase) &&
            !string.Equals(draftStatus, request.Estado, StringComparison.OrdinalIgnoreCase);
    }

    protected async Task SaveStatusAsync(AdminFederativeLicenseRequest request)
    {
        if (!CanSaveStatus(request))
        {
            return;
        }

        UpdatingRequestId = request.Id;
        ErrorMessageKey = null;
        StatusMessageKey = null;

        var result = await AdminFederativeLicenseService.UpdateFederativeLicenseStatusAsync(
            new UpdateAdminFederativeLicenseStatusRequest(
                request.UserId,
                request.Id,
                _draftStatuses[request.Id]));

        UpdatingRequestId = null;

        if (!result.IsSuccess)
        {
            ErrorMessageKey = MapUpdateError(result.Error?.Code);
            return;
        }

        await LoadAsync(_currentQuery, clearMessages: false);
        StatusMessageKey = "settings_federative_licenses_update_success";
    }

    protected string FormatTimestamp(DateTime timestampUtc)
    {
        return timestampUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture);
    }

    protected bool IsOwnRequest(AdminFederativeLicenseRequest request)
    {
        return SessionService.CurrentSession?.UserId == request.UserId;
    }

    private void ApplyInitialFilters()
    {
        if (InitialUserId.HasValue)
        {
            UserIdFilterText = InitialUserId.Value.ToString("D", CultureInfo.InvariantCulture);
        }
    }

    private AdminFederativeLicenseQuery? BuildCurrentQuery()
    {
        Guid? parsedUserId = null;
        if (!string.IsNullOrWhiteSpace(UserIdFilterText))
        {
            if (!Guid.TryParse(UserIdFilterText, out var userId))
            {
                ErrorMessageKey = "settings_federative_licenses_invalid_user";
                return null;
            }

            parsedUserId = userId;
        }

        int? parsedTemporada = null;
        if (!string.IsNullOrWhiteSpace(SeasonFilterText))
        {
            if (!int.TryParse(SeasonFilterText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var temporada))
            {
                ErrorMessageKey = "settings_federative_licenses_invalid_season";
                return null;
            }

            parsedTemporada = temporada;
        }

        ErrorMessageKey = null;

        return new AdminFederativeLicenseQuery(
            parsedUserId,
            parsedTemporada,
            string.IsNullOrWhiteSpace(StatusFilter) ? null : StatusFilter);
    }

    private async Task LoadAsync(AdminFederativeLicenseQuery query, bool clearMessages = true)
    {
        if (clearMessages)
        {
            ErrorMessageKey = null;
            StatusMessageKey = null;
        }

        IsLoading = true;
        _currentQuery = query;

        var result = await AdminFederativeLicenseService.GetFederativeLicensesAsync(query);

        IsLoading = false;

        if (!result.IsSuccess)
        {
            Requests = Array.Empty<AdminFederativeLicenseRequest>();
            _draftStatuses.Clear();
            ErrorMessageKey = "settings_federative_licenses_load_error";
            return;
        }

        Requests = result.Value ?? Array.Empty<AdminFederativeLicenseRequest>();
        _draftStatuses.Clear();
        foreach (var request in Requests)
        {
            _draftStatuses[request.Id] = request.Estado;
        }
    }

    private static string MapUpdateError(string? errorCode)
    {
        return errorCode switch
        {
            "licenses.admin_not_found" => "settings_federative_licenses_update_not_found",
            "licenses.admin_validation" => "settings_federative_licenses_update_validation",
            "licenses.admin_forbidden" => "settings_federative_licenses_update_forbidden",
            _ => "settings_federative_licenses_update_error"
        };
    }
}
