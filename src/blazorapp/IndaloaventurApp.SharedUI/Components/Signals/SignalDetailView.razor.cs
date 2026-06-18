namespace IndaloaventurApp.SharedUI.Components.Signals;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class SignalDetailView
{
    [Parameter]
    public Guid SignalId { get; set; }

    [Inject]
    private ISignalService SignalService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected SignalDetailItem? Signal { get; private set; }

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessageKey { get; private set; }

    protected string? StatusMessageKey { get; private set; }

    protected string? SaveErrorMessageKey { get; private set; }

    protected SignalDetailTab ActiveTab { get; private set; } = SignalDetailTab.Data;

    protected bool IsEditing { get; private set; }

    protected bool IsSaving { get; private set; }

    protected SignalDetailEditorModel Editor { get; private set; } = new();

    protected IReadOnlyList<SignalCommentItem> Comments { get; private set; } = Array.Empty<SignalCommentItem>();

    protected IReadOnlyList<string> Tags =>
        string.IsNullOrWhiteSpace(Signal?.Tags)
            ? Array.Empty<string>()
            : Signal.Tags
                .Split([',', ';', '|'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(static tag => !string.IsNullOrWhiteSpace(tag))
                .ToArray();

    protected bool HasCoordinates =>
        Signal is not null &&
        Signal.Latitude >= -90 &&
        Signal.Latitude <= 90 &&
        Signal.Longitude >= -180 &&
        Signal.Longitude <= 180 &&
        (Signal.Latitude != 0 || Signal.Longitude != 0);

    protected string CoordinateText =>
        Signal is null
            ? string.Empty
            : string.Format(
                CultureInfo.InvariantCulture,
                "{0:0.000000}, {1:0.000000}",
                Signal.Latitude,
                Signal.Longitude);

    protected string MapEmbedUrl => Signal is null
        ? string.Empty
        : string.Create(
            CultureInfo.InvariantCulture,
            $"https://www.openstreetmap.org/export/embed.html?bbox={Signal.Longitude - 0.01:0.000000}%2C{Signal.Latitude - 0.01:0.000000}%2C{Signal.Longitude + 0.01:0.000000}%2C{Signal.Latitude + 0.01:0.000000}&layer=mapnik&marker={Signal.Latitude:0.000000}%2C{Signal.Longitude:0.000000}");

    protected string MapExternalUrl => Signal is null
        ? string.Empty
        : string.Create(
            CultureInfo.InvariantCulture,
            $"https://www.openstreetmap.org/?mlat={Signal.Latitude:0.000000}&mlon={Signal.Longitude:0.000000}#map=16/{Signal.Latitude:0.000000}/{Signal.Longitude:0.000000}");

    protected bool CanEditSignal =>
        Signal is not null &&
        SessionService.CurrentSession?.UserId is Guid currentUserId &&
        currentUserId == Signal.OwnerUserId;

    protected override async Task OnParametersSetAsync()
    {
        await SessionService.EnsureInitializedAsync();
        await LoadAsync();
    }

    protected void SelectTab(SignalDetailTab tab)
    {
        ActiveTab = tab;
    }

    protected string FormatTimestamp(DateTime timestamp)
        => timestamp.ToLocalTime().ToString("dd MMM yyyy, HH:mm", CultureInfo.GetCultureInfo("es-ES"));

    protected string GetHeaderSummary()
    {
        if (Signal is null)
        {
            return L["signal_detail_summary"];
        }

        return string.Format(L["signal_detail_updated_at"], FormatTimestamp(Signal.UpdatedAt));
    }

    protected string GetTagsCaption()
        => Tags.Count == 0 ? L["signal_detail_tags_empty"] : string.Format(L["signal_detail_tags_count"], Tags.Count);

    protected string GetCommentsCaption()
        => Comments.Count == 0
            ? L["signal_detail_comments_empty"]
            : string.Format(L["signal_detail_comments_count"], Comments.Count);

    protected string GetStatusText(SignalDetailItem signal)
        => signal.IsActive ? L["signal_detail_status_active"] : L["signal_detail_status_archived"];

    protected void BeginEdit()
    {
        if (!CanEditSignal || Signal is null)
        {
            return;
        }

        SaveErrorMessageKey = null;
        StatusMessageKey = null;
        IsEditing = true;
        Editor = SignalDetailEditorModel.FromSignal(Signal);
    }

    protected void CancelEdit()
    {
        IsEditing = false;
        SaveErrorMessageKey = null;

        if (Signal is not null)
        {
            Editor = SignalDetailEditorModel.FromSignal(Signal);
        }
    }

    protected async Task SaveAsync()
    {
        if (!CanEditSignal || Signal is null)
        {
            SaveErrorMessageKey = "signal_detail_edit_forbidden";
            return;
        }

        if (string.IsNullOrWhiteSpace(Editor.Title))
        {
            SaveErrorMessageKey = "signal_detail_edit_title_required";
            return;
        }

        if (string.IsNullOrWhiteSpace(Editor.Description))
        {
            SaveErrorMessageKey = "signal_detail_edit_description_required";
            return;
        }

        SaveErrorMessageKey = null;
        StatusMessageKey = null;
        IsSaving = true;

        var result = await SignalService.UpdateSignalAsync(new UpdateSignalRequest(
            Signal.Id,
            Editor.Title.Trim(),
            Editor.Description.Trim(),
            Editor.IsActive));

        IsSaving = false;

        if (!result.IsSuccess)
        {
            SaveErrorMessageKey = MapUpdateErrorKey(result.Error?.Code);
            return;
        }

        IsEditing = false;
        await LoadAsync(clearFeedback: false);
        StatusMessageKey = "signal_detail_edit_success";
    }

    private async Task LoadAsync(bool clearFeedback = true)
    {
        IsLoading = true;
        ErrorMessageKey = null;
        SaveErrorMessageKey = null;
        if (clearFeedback)
        {
            StatusMessageKey = null;
        }

        Signal = null;
        Comments = Array.Empty<SignalCommentItem>();
        ActiveTab = SignalDetailTab.Data;
        StateHasChanged();

        var result = await SignalService.GetSignalAsync(SignalId);

        IsLoading = false;

        if (result.IsSuccess)
        {
            var signal = result.Value!;
            Signal = signal;
            if (!IsEditing)
            {
                Editor = SignalDetailEditorModel.FromSignal(signal);
            }

            var commentsResult = await SignalService.GetSignalCommentsAsync(SignalId);
            Comments = commentsResult.IsSuccess
                ? commentsResult.Value ?? Array.Empty<SignalCommentItem>()
                : Array.Empty<SignalCommentItem>();
            return;
        }

        ErrorMessageKey = MapErrorKey(result.Error?.Code);
    }

    private static string MapErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.not_found" => "signal_detail_not_found",
            "signals.timeout" => "signal_detail_timeout",
            _ => "signal_detail_error"
        };
    }

    private static string MapUpdateErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.update_validation" => "signal_detail_edit_validation_error",
            "signals.update_forbidden" => "signal_detail_edit_forbidden",
            "signals.not_found" => "signal_detail_not_found",
            "auth.session_invalid" => "signal_detail_edit_auth_error",
            _ => "signal_detail_edit_error"
        };
    }

    protected enum SignalDetailTab
    {
        Data,
        Map
    }

    protected sealed class SignalDetailEditorModel
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public static SignalDetailEditorModel FromSignal(SignalDetailItem signal)
        {
            return new SignalDetailEditorModel
            {
                Title = signal.Title,
                Description = signal.Description,
                IsActive = signal.IsActive
            };
        }
    }
}
