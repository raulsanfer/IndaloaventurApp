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
    private const string DefaultCommentErrorKey = "signal_detail_comment_error";

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

    protected string? ImagesErrorMessageKey { get; private set; }

    protected string? CommentsErrorMessageKey { get; private set; }

    protected string? CommentStatusMessageKey { get; private set; }

    protected string? CommentErrorMessageKey { get; private set; }

    protected SignalDetailTab ActiveTab { get; private set; } = SignalDetailTab.Data;

    protected bool IsEditing { get; private set; }

    protected bool IsSaving { get; private set; }

    protected bool IsCommentModalOpen { get; private set; }

    protected bool IsCreatingComment { get; private set; }

    protected bool IsLoadingImages { get; private set; }

    protected SignalDetailEditorModel Editor { get; private set; } = new();

    protected SignalCommentEditorModel CommentEditor { get; private set; } = new();

    protected IReadOnlyList<SignalCommentItem> Comments { get; private set; } = Array.Empty<SignalCommentItem>();

    protected SignalImagesItem? Images { get; private set; }

    protected IReadOnlyList<int> ImageSlots { get; } = [1, 2];

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

    protected bool CanCommentSignal =>
        Signal is not null &&
        SessionService.CurrentSession?.CanCommentSignals() == true;

    protected bool CanSubmitComment =>
        CanCommentSignal &&
        !IsCreatingComment &&
        !string.IsNullOrWhiteSpace(CommentEditor.Text);

    protected bool HasAnyImages =>
        !string.IsNullOrWhiteSpace(Images?.Photo1Url) ||
        !string.IsNullOrWhiteSpace(Images?.Photo2Url);

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

    protected string? GetImageUrl(int slot)
        => slot == 1 ? Images?.Photo1Url : Images?.Photo2Url;

    protected string GetImageLabel(int slot)
        => slot == 1 ? L["signal_create_photo_1"] : L["signal_create_photo_2"];

    protected string GetImageAlt(int slot)
        => string.Format(L["signal_detail_photo_alt"], GetImageLabel(slot), Signal?.Title ?? L["signal_detail_title"].Value);

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

    protected void OpenCommentModal()
    {
        if (!CanCommentSignal)
        {
            return;
        }

        CommentStatusMessageKey = null;
        CommentErrorMessageKey = null;
        CommentEditor = new SignalCommentEditorModel();
        IsCommentModalOpen = true;
    }

    protected void CloseCommentModal()
    {
        if (IsCreatingComment)
        {
            return;
        }

        IsCommentModalOpen = false;
        CommentErrorMessageKey = null;
        CommentEditor = new SignalCommentEditorModel();
    }

    protected async Task SubmitCommentAsync()
    {
        if (!CanCommentSignal)
        {
            CommentErrorMessageKey = "signal_detail_comment_forbidden";
            return;
        }

        if (string.IsNullOrWhiteSpace(CommentEditor.Text))
        {
            CommentErrorMessageKey = "signal_detail_comment_text_required";
            return;
        }

        IsCreatingComment = true;
        CommentErrorMessageKey = null;
        CommentStatusMessageKey = null;

        var result = await SignalService.CreateSignalCommentAsync(new CreateSignalCommentRequest(
            SignalId,
            CommentEditor.Text.Trim()));

        if (!result.IsSuccess)
        {
            IsCreatingComment = false;
            CommentErrorMessageKey = MapCreateCommentErrorKey(result.Error?.Code);
            return;
        }

        var commentsReloaded = await LoadCommentsAsync();
        IsCreatingComment = false;

        if (!commentsReloaded)
        {
            CommentErrorMessageKey = CommentsErrorMessageKey ?? DefaultCommentErrorKey;
            return;
        }

        IsCommentModalOpen = false;
        CommentEditor = new SignalCommentEditorModel();
        CommentStatusMessageKey = "signal_detail_comment_success";
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
        IsLoadingImages = false;
        ErrorMessageKey = null;
        ImagesErrorMessageKey = null;
        CommentsErrorMessageKey = null;
        SaveErrorMessageKey = null;
        CommentErrorMessageKey = null;
        if (clearFeedback)
        {
            StatusMessageKey = null;
            CommentStatusMessageKey = null;
        }

        Signal = null;
        Images = null;
        Comments = Array.Empty<SignalCommentItem>();
        ActiveTab = SignalDetailTab.Data;
        IsCommentModalOpen = false;
        StateHasChanged();

        var result = await SignalService.GetSignalAsync(SignalId);

        if (result.IsSuccess)
        {
            var signal = result.Value!;
            Signal = signal;
            if (!IsEditing)
            {
                Editor = SignalDetailEditorModel.FromSignal(signal);
            }

            IsLoading = false;
            IsLoadingImages = true;
            StateHasChanged();

            var commentsTask = LoadCommentsAsync();
            var imagesTask = SignalService.GetSignalImagesAsync(SignalId);

            await commentsTask;

            var imagesResult = await imagesTask;
            if (imagesResult.IsSuccess)
            {
                Images = imagesResult.Value ?? new SignalImagesItem(SignalId, null, null);
            }
            else
            {
                ImagesErrorMessageKey = MapImagesErrorKey(imagesResult.Error?.Code);
            }

            IsLoadingImages = false;
            return;
        }

        IsLoading = false;
        ErrorMessageKey = MapErrorKey(result.Error?.Code);
    }

    private async Task<bool> LoadCommentsAsync()
    {
        CommentsErrorMessageKey = null;

        var commentsResult = await SignalService.GetSignalCommentsAsync(SignalId);
        if (!commentsResult.IsSuccess)
        {
            Comments = Array.Empty<SignalCommentItem>();
            CommentsErrorMessageKey = MapCommentsErrorKey(commentsResult.Error?.Code);
            return false;
        }

        Comments = commentsResult.Value ?? Array.Empty<SignalCommentItem>();
        return true;
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

    private static string MapCreateCommentErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.comments_create_validation" => "signal_detail_comment_validation_error",
            "signals.comments_create_forbidden" => "signal_detail_comment_forbidden",
            "signals.not_found" => "signal_detail_not_found",
            "auth.session_invalid" => "signal_detail_comment_auth_error",
            _ => DefaultCommentErrorKey
        };
    }

    private static string MapCommentsErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.timeout" => "signal_detail_comments_timeout",
            _ => "signal_detail_comments_error"
        };
    }

    private static string MapImagesErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.timeout" => "signal_detail_images_timeout",
            _ => "signal_detail_images_error"
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

    protected sealed class SignalCommentEditorModel
    {
        public string Text { get; set; } = string.Empty;
    }
}
