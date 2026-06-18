namespace IndaloaventurApp.SharedUI.Components.Signals;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class SignalHomeView : IDisposable
{
    [Inject]
    private ISignalService SignalService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<SignalCategoryItem> Categories { get; private set; } = Array.Empty<SignalCategoryItem>();

    protected IReadOnlyList<SignalCardItem> Signals { get; private set; } = Array.Empty<SignalCardItem>();

    protected bool IsLoading { get; private set; } = true;

    protected string? ErrorMessageKey { get; private set; }

    protected string SearchText { get; private set; } = string.Empty;

    protected int? SelectedCategoryId { get; private set; }

    private CancellationTokenSource? searchDebounceCts;

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    protected async Task OnSearchInputAsync(ChangeEventArgs args)
    {
        SearchText = args.Value?.ToString() ?? string.Empty;
        await DebounceLoadAsync();
    }

    protected async Task SelectCategoryAsync(int? categoryId)
    {
        if (SelectedCategoryId == categoryId)
        {
            return;
        }

        SelectedCategoryId = categoryId;
        await LoadAsync();
    }

    protected string FormatTimestamp(DateTime timestamp)
    {
        var localDate = timestamp.ToLocalTime().Date;
        var today = DateTime.Now.Date;

        if (localDate == today)
        {
            return string.Format(L["signal_home_date_today"], timestamp.ToLocalTime().ToString("HH:mm", CultureInfo.GetCultureInfo("es-ES")));
        }

        if (localDate == today.AddDays(-1))
        {
            return L["signal_home_date_yesterday"];
        }

        return timestamp.ToLocalTime().ToString("dd MMM", CultureInfo.GetCultureInfo("es-ES"));
    }

    protected string GetMetaIcon(SignalCardItem signal)
    {
        if (!string.IsNullOrWhiteSpace(signal.Tags))
        {
            return "bi-tag-fill";
        }

        if (signal.Latitude != 0 || signal.Longitude != 0)
        {
            return "bi-geo-alt-fill";
        }

        return signal.IsActive ? "bi-exclamation-triangle-fill" : "bi-archive-fill";
    }

    protected string GetMetaText(SignalCardItem signal)
    {
        return string.IsNullOrWhiteSpace(signal.MetaLabel)
            ? L["signal_home_meta_fallback"]
            : signal.MetaLabel;
    }

    private async Task DebounceLoadAsync()
    {
        searchDebounceCts?.Cancel();
        searchDebounceCts?.Dispose();
        searchDebounceCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(300, searchDebounceCts.Token);
            await LoadAsync(searchDebounceCts.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsLoading = true;
        ErrorMessageKey = null;
        StateHasChanged();

        var result = await SignalService.GetSignalHomeDataAsync(
            new SignalListQuery(SearchText, SelectedCategoryId, OnlyActive: true),
            cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        IsLoading = false;

        if (result.IsSuccess)
        {
            Categories = result.Value?.Categories ?? Array.Empty<SignalCategoryItem>();
            Signals = result.Value?.Signals ?? Array.Empty<SignalCardItem>();
            return;
        }

        ErrorMessageKey = MapErrorKey(result.Error?.Code);
        Categories = Array.Empty<SignalCategoryItem>();
        Signals = Array.Empty<SignalCardItem>();
    }

    private static string MapErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.timeout" => "signal_home_timeout",
            "auth.session_invalid" => "signal_home_error",
            _ => "signal_home_error"
        };
    }

    public void Dispose()
    {
        searchDebounceCts?.Cancel();
        searchDebounceCts?.Dispose();
    }
}
