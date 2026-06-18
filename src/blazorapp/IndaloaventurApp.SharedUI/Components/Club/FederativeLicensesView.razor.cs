namespace IndaloaventurApp.SharedUI.Components.Club;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Licenses;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

public partial class FederativeLicensesView
{
    private const string DefaultRequestErrorKey = "mi_club_federative_licenses_request_error";
    private const string EmptyRatesMessageKey = "mi_club_federative_licenses_request_rates_empty";
    private const string InvalidCatalogMessageKey = "mi_club_federative_licenses_request_catalog_invalid";
    private const string FullSeasonValue = "false";
    private const string HalfSeasonValue = "true";

    [Inject]
    private IFederativeLicenseService FederativeLicenseService { get; set; } = default!;

    [Inject]
    private ISessionService SessionService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    protected IReadOnlyList<FederativeLicenseSeasonGroup> SeasonGroups { get; private set; } = Array.Empty<FederativeLicenseSeasonGroup>();

    protected IReadOnlyList<FederativeLicenseRate> AvailableRates { get; private set; } = Array.Empty<FederativeLicenseRate>();

    protected bool IsLoading { get; private set; } = true;

    protected bool IsRatesLoading { get; private set; }

    protected bool IsSubmittingRequest { get; private set; }

    protected bool IsRequestModalOpen { get; private set; }

    protected bool IsNonOperational => SessionService.CurrentSession?.CanAccessOwnFederativeLicenses() != true;

    protected bool HasRateCatalogEmptyState { get; private set; }

    protected string? ErrorMessageKey { get; private set; }

    protected string? RequestMessageKey { get; private set; }

    protected string? RequestMessageText { get; private set; }

    protected bool IsRequestMessageError { get; private set; }

    protected bool SelectedMediaTemporada { get; private set; }

    protected int? SelectedTemporada { get; private set; }

    protected string? SelectedLicencia { get; private set; }

    protected string? SelectedCategoria { get; private set; }

    protected string SelectedModalityValue => SelectedMediaTemporada ? HalfSeasonValue : FullSeasonValue;

    protected IReadOnlyList<FederativeLicenseSelectionOption> ModalityOptions =>
        new[]
        {
            new FederativeLicenseSelectionOption(FullSeasonValue, L["mi_club_federative_licenses_request_modality_full"]),
            new FederativeLicenseSelectionOption(HalfSeasonValue, L["mi_club_federative_licenses_request_modality_half"])
        };

    protected IReadOnlyList<FederativeLicenseSelectionOption> SeasonOptions =>
        new[]
        {
            new FederativeLicenseSelectionOption(CurrentYear.ToString(CultureInfo.InvariantCulture), CurrentYear.ToString(CultureInfo.InvariantCulture)),
            new FederativeLicenseSelectionOption((CurrentYear + 1).ToString(CultureInfo.InvariantCulture), (CurrentYear + 1).ToString(CultureInfo.InvariantCulture))
        };

    protected IReadOnlyList<FederativeLicenseSelectionOption> LicenseOptions =>
        AvailableRates
            .Select(static rate => rate.Licencia)
            .Where(static licencia => !string.IsNullOrWhiteSpace(licencia))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(static licencia => licencia, StringComparer.CurrentCultureIgnoreCase)
            .Select(licencia => new FederativeLicenseSelectionOption(licencia, licencia))
            .ToArray();

    protected IReadOnlyList<FederativeLicenseSelectionOption> CategoryOptions =>
        AvailableRates
            .Where(rate => string.Equals(rate.Licencia, SelectedLicencia, StringComparison.CurrentCultureIgnoreCase))
            .Select(static rate => rate.Categoria)
            .Where(static categoria => !string.IsNullOrWhiteSpace(categoria))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(static categoria => categoria, StringComparer.CurrentCultureIgnoreCase)
            .Select(categoria => new FederativeLicenseSelectionOption(categoria, categoria))
            .ToArray();

    protected FederativeLicenseRate? SelectedRate =>
        SelectedRateMatches.Count == 1
            ? SelectedRateMatches[0]
            : null;

    protected bool CanConfirmRequest =>
        !IsNonOperational &&
        !IsRatesLoading &&
        !IsSubmittingRequest &&
        SelectedRate is not null &&
        !HasRateCatalogEmptyState;

    protected override async Task OnInitializedAsync()
    {
        if (IsNonOperational)
        {
            IsLoading = false;
            return;
        }

        await LoadMyLicensesAsync();
    }

    protected void OpenRequestModal()
    {
        if (IsNonOperational)
        {
            return;
        }

        ResetRequestState();
        IsRequestModalOpen = true;
    }

    protected void CloseRequestModal()
    {
        if (IsSubmittingRequest)
        {
            return;
        }

        IsRequestModalOpen = false;
        ResetRequestState();
    }

    protected async Task OnSeasonChangedAsync(ChangeEventArgs args)
    {
        ResetRateSelection();

        if (!TryParseSelectedSeason(args.Value, out var temporada))
        {
            SelectedTemporada = null;
            return;
        }

        SelectedTemporada = temporada;
        await LoadRatesForSeasonAsync(temporada);
    }

    protected async Task OnModalityChangedAsync(ChangeEventArgs args)
    {
        if (!TryParseSelectedModality(args.Value, out var mediaTemporada))
        {
            mediaTemporada = false;
        }

        SelectedMediaTemporada = mediaTemporada;
        ResetRateSelection();

        if (SelectedTemporada is int temporada)
        {
            await LoadRatesForSeasonAsync(temporada);
        }
    }

    protected void OnLicenseChanged(ChangeEventArgs args)
    {
        SelectedLicencia = GetNormalizedValue(args.Value);
        SelectedCategoria = null;
        UpdateAmbiguousSelectionMessage();
    }

    protected void OnCategoryChanged(ChangeEventArgs args)
    {
        SelectedCategoria = GetNormalizedValue(args.Value);
        UpdateAmbiguousSelectionMessage();
    }

    protected async Task SubmitRequestAsync()
    {
        if (!CanConfirmRequest || SelectedTemporada is null || SelectedRate is null)
        {
            return;
        }

        IsSubmittingRequest = true;
        ClearRequestMessages();

        var result = await FederativeLicenseService.CreateFederativeLicenseRequestAsync(
            new CreateFederativeLicenseRequest(SelectedTemporada.Value, SelectedRate.Id));

        IsSubmittingRequest = false;

        if (!result.IsSuccess)
        {
            SetRequestError(result.Error);
            return;
        }

        IsRequestModalOpen = false;
        ResetRequestState();
        await LoadMyLicensesAsync();
    }

    protected string GetSeasonLabel(int temporada)
    {
        var localizedTemplate = L["mi_club_federative_licenses_season"].Value;

        if (localizedTemplate.Contains("{0}", StringComparison.Ordinal))
        {
            return string.Format(localizedTemplate, temporada);
        }

        return $"Temporada {temporada}";
    }

    protected string GetRequestSeasonLabel(FederativeLicenseRequest request)
    {
        var localizedTemplate = L["mi_club_federative_licenses_season_with_modality"].Value;
        var seasonLabel = GetSeasonLabel(request.Temporada);
        var modalityLabel = GetModalityLabel(request.MediaTemporada);

        if (localizedTemplate.Contains("{0}", StringComparison.Ordinal) &&
            localizedTemplate.Contains("{1}", StringComparison.Ordinal))
        {
            return string.Format(localizedTemplate, seasonLabel, modalityLabel);
        }

        return $"{seasonLabel} · {modalityLabel}";
    }

    protected string FormatPrice(decimal price)
    {
        return price.ToString("C", CultureInfo.CurrentCulture);
    }

    protected string ResolveRequestMessage()
    {
        if (!string.IsNullOrWhiteSpace(RequestMessageKey))
        {
            return L[RequestMessageKey];
        }

        return RequestMessageText ?? string.Empty;
    }

    private int CurrentYear => DateTime.UtcNow.Year;

    private IReadOnlyList<FederativeLicenseRate> SelectedRateMatches =>
        AvailableRates
            .Where(rate =>
                rate.Temporada == SelectedTemporada &&
                rate.MediaTemporada == SelectedMediaTemporada &&
                string.Equals(rate.Licencia, SelectedLicencia, StringComparison.CurrentCultureIgnoreCase) &&
                string.Equals(rate.Categoria, SelectedCategoria, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();

    private async Task LoadMyLicensesAsync()
    {
        IsLoading = true;
        ErrorMessageKey = null;

        var result = await FederativeLicenseService.GetMyFederativeLicensesAsync();

        IsLoading = false;

        if (!result.IsSuccess)
        {
            ErrorMessageKey = result.Error?.Code switch
            {
                "licenses.not_member" => "mi_club_federative_licenses_not_available",
                _ => "mi_club_federative_licenses_error"
            };
            SeasonGroups = Array.Empty<FederativeLicenseSeasonGroup>();
            return;
        }

        SeasonGroups = (result.Value ?? Array.Empty<FederativeLicenseRequest>())
            .GroupBy(static request => request.Temporada)
            .OrderByDescending(static group => group.Key)
            .Select(group => new FederativeLicenseSeasonGroup(
                group.Key,
                group
                    .OrderBy(request => request.Licencia, StringComparer.CurrentCultureIgnoreCase)
                    .ThenBy(request => request.MediaTemporada)
                    .ToArray()))
            .ToArray();
    }

    private async Task LoadRatesForSeasonAsync(int temporada)
    {
        IsRatesLoading = true;

        var result = await FederativeLicenseService.GetAvailableRatesAsync(temporada, SelectedMediaTemporada);

        IsRatesLoading = false;

        if (!result.IsSuccess)
        {
            SetRequestError(result.Error, DefaultRequestErrorKey);
            return;
        }

        AvailableRates = (result.Value ?? Array.Empty<FederativeLicenseRate>())
            .Where(rate => rate.Temporada == temporada && rate.MediaTemporada == SelectedMediaTemporada)
            .ToArray();
        HasRateCatalogEmptyState = AvailableRates.Count == 0;

        if (HasRateCatalogEmptyState)
        {
            SetRequestMessage(EmptyRatesMessageKey, isError: false);
            return;
        }

        UpdateAmbiguousSelectionMessage();
    }

    private void UpdateAmbiguousSelectionMessage()
    {
        if (HasAmbiguousSelection())
        {
            SetRequestMessage(InvalidCatalogMessageKey, isError: true);
            return;
        }

        if (!HasRateCatalogEmptyState)
        {
            ClearRequestMessages();
        }
    }

    private bool HasAmbiguousSelection()
    {
        return SelectedTemporada is not null &&
            !string.IsNullOrWhiteSpace(SelectedLicencia) &&
            !string.IsNullOrWhiteSpace(SelectedCategoria) &&
            SelectedRateMatches.Count > 1;
    }

    private void ResetRequestState()
    {
        SelectedMediaTemporada = false;
        SelectedTemporada = null;
        ResetRateSelection();
        IsRatesLoading = false;
        IsSubmittingRequest = false;
    }

    private void ResetRateSelection()
    {
        ClearRequestMessages();
        SelectedLicencia = null;
        SelectedCategoria = null;
        AvailableRates = Array.Empty<FederativeLicenseRate>();
        HasRateCatalogEmptyState = false;
    }

    private void ClearRequestMessages()
    {
        RequestMessageKey = null;
        RequestMessageText = null;
        IsRequestMessageError = false;
    }

    private void SetRequestMessage(string key, bool isError)
    {
        RequestMessageKey = key;
        RequestMessageText = null;
        IsRequestMessageError = isError;
    }

    private void SetRequestError(ServiceError? error, string fallbackKey = DefaultRequestErrorKey)
    {
        if (!string.IsNullOrWhiteSpace(error?.Message))
        {
            RequestMessageText = error.Message;
            RequestMessageKey = null;
        }
        else
        {
            RequestMessageKey = fallbackKey;
            RequestMessageText = null;
        }

        IsRequestMessageError = true;
    }

    private string GetModalityLabel(bool mediaTemporada)
    {
        var key = mediaTemporada
            ? "mi_club_federative_licenses_request_modality_half"
            : "mi_club_federative_licenses_request_modality_full";

        var localizedValue = L[key].Value;
        if (!string.Equals(localizedValue, key, StringComparison.Ordinal))
        {
            return localizedValue;
        }

        return mediaTemporada ? "Media Temporada" : "Temporada Completa";
    }

    private static bool TryParseSelectedSeason(object? value, out int temporada)
    {
        var rawValue = GetNormalizedValue(value);
        return int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out temporada);
    }

    private static bool TryParseSelectedModality(object? value, out bool mediaTemporada)
    {
        var rawValue = GetNormalizedValue(value);
        return bool.TryParse(rawValue, out mediaTemporada);
    }

    private static string? GetNormalizedValue(object? value)
    {
        var stringValue = value?.ToString()?.Trim();
        return string.IsNullOrWhiteSpace(stringValue) ? null : stringValue;
    }
}
