namespace IndaloaventurApp.SharedUI.Components.Signals;

using System.Globalization;
using IndaloaventurApp.SharedUI.Abstractions.Signals;
using IndaloaventurApp.SharedUI.Models.Signals;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

public partial class SignalCreateView
{
    private const long MaxPhotoBytes = 2 * 1024 * 1024;
    private const int MaxPhotoDimension = 1600;
    private const int MinPhotoDimension = 640;

    [Inject]
    private ISignalService SignalService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    protected IReadOnlyList<SignalCategoryItem> Categories { get; private set; } = Array.Empty<SignalCategoryItem>();

    protected SignalCreateDraft Draft { get; } = new();

    protected SignalCreateStep CurrentStep { get; private set; } = SignalCreateStep.Type;

    protected bool IsLoadingCategories { get; private set; } = true;

    protected bool IsLocating { get; private set; }

    protected bool IsSaving { get; private set; }

    protected bool IsCompleted { get; private set; }

    protected bool IsProcessingPhoto { get; private set; }

    protected string? CategoryErrorMessageKey { get; private set; }

    protected string? TypeErrorMessageKey { get; private set; }

    protected string? DetailsErrorMessageKey { get; private set; }

    protected string? PhotoErrorMessageKey { get; private set; }

    protected string? LocationStatusMessageKey { get; private set; }

    protected string? SaveErrorMessageKey { get; private set; }

    protected IReadOnlyList<SignalCreateStep> OrderedSteps { get; } =
    [
        SignalCreateStep.Type,
        SignalCreateStep.Details,
        SignalCreateStep.Photos,
        SignalCreateStep.Review
    ];

    protected IReadOnlyList<int> PhotoSlots { get; } = [1, 2];

    protected bool HasLocation => Draft.Latitude.HasValue && Draft.Longitude.HasValue;

    protected SignalCategoryItem? SelectedCategory => Categories.FirstOrDefault(item => item.Id == Draft.TypeId);

    protected string LocationText =>
        HasLocation
            ? string.Format(
                CultureInfo.GetCultureInfo("es-ES"),
                L["signal_create_location_value"],
                Draft.Latitude!.Value,
                Draft.Longitude!.Value)
            : string.Empty;

    protected IReadOnlyList<string> ParsedTags =>
        Draft.Tags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

    protected string GetStepClass(SignalCreateStep step)
    {
        return (int)step <= (int)CurrentStep ? "step-primary" : string.Empty;
    }

    protected string GetStepLabel(SignalCreateStep step)
    {
        return step switch
        {
            SignalCreateStep.Type => L["signal_create_step_type"],
            SignalCreateStep.Details => L["signal_create_step_details"],
            SignalCreateStep.Photos => L["signal_create_step_photos"],
            SignalCreateStep.Review => L["signal_create_step_review"],
            _ => string.Empty
        };
    }

    protected void SelectType(SignalCategoryItem category)
    {
        Draft.TypeId = category.Id;
        TypeErrorMessageKey = null;
        SaveErrorMessageKey = null;
        CurrentStep = SignalCreateStep.Details;
    }

    protected void GoBack()
    {
        SaveErrorMessageKey = null;

        if ((int)CurrentStep > (int)SignalCreateStep.Type)
        {
            CurrentStep = (SignalCreateStep)((int)CurrentStep - 1);
        }
    }

    protected void GoToPhotos()
    {
        if (!ValidateDetails())
        {
            return;
        }

        CurrentStep = SignalCreateStep.Photos;
    }

    protected void GoToReview()
    {
        if (!ValidatePhotos())
        {
            return;
        }

        CurrentStep = SignalCreateStep.Review;
    }

    protected async Task RequestCurrentLocationAsync()
    {
        LocationStatusMessageKey = null;
        IsLocating = true;

        try
        {
            var coordinates = await JSRuntime.InvokeAsync<SignalCoordinates>("indaloSignals.getCurrentPosition");
            Draft.Latitude = coordinates.Latitude;
            Draft.Longitude = coordinates.Longitude;
        }
        catch (JSException)
        {
            LocationStatusMessageKey = "signal_create_location_error";
        }
        finally
        {
            IsLocating = false;
        }
    }

    protected void ClearLocation()
    {
        Draft.Latitude = null;
        Draft.Longitude = null;
        LocationStatusMessageKey = null;
    }

    protected async Task OpenPhotoPickerAsync(int slot, PhotoInputMode mode)
    {
        await JSRuntime.InvokeVoidAsync("indaloSignals.triggerFileInput", GetPhotoInputId(slot, mode));
    }

    protected SignalPhotoDraft? GetPhoto(int slot)
    {
        return slot == 1 ? Draft.Photo1 : Draft.Photo2;
    }

    protected string GetPhotoLabel(int slot)
    {
        return slot == 1 ? L["signal_create_photo_1"] : L["signal_create_photo_2"];
    }

    protected string GetPhotoCameraButtonLabel(int slot)
    {
        return slot == 1 ? L["signal_create_photo_1_camera_button"] : L["signal_create_photo_2_camera_button"];
    }

    protected string GetPhotoGalleryButtonLabel(int slot)
    {
        return slot == 1 ? L["signal_create_photo_1_gallery_button"] : L["signal_create_photo_2_gallery_button"];
    }

    protected string GetPhotoCaption(int slot)
    {
        return slot == 1 ? L["signal_create_photo_1_hint"] : L["signal_create_photo_2_hint"];
    }

    protected bool IsPhotoRequired(int slot)
    {
        return slot == 1;
    }

    protected void RemovePhoto2()
    {
        Draft.Photo2 = null;
        PhotoErrorMessageKey = null;
        SaveErrorMessageKey = null;
    }

    protected async Task SaveAsync()
    {
        SaveErrorMessageKey = null;

        if (!ValidateAllSteps())
        {
            return;
        }

        IsSaving = true;

        var request = new SignalCreateRequest(
            Draft.Latitude ?? 0,
            Draft.Longitude ?? 0,
            Draft.Title.Trim(),
            Draft.Description.Trim(),
            Draft.Photo1!.Content,
            SignalImageCodec.NormalizeOptionalPhotoContent(Draft.Photo2),
            IsActive: true,
            Draft.TypeId!.Value,
            string.Join(",", ParsedTags));

        var result = await SignalService.CreateSignalAsync(request);
        IsSaving = false;

        if (result.IsSuccess)
        {
            IsCompleted = true;
            return;
        }

        SaveErrorMessageKey = MapCreateErrorKey(result.Error?.Code);
    }

    private async Task LoadCategoriesAsync()
    {
        IsLoadingCategories = true;
        CategoryErrorMessageKey = null;

        var result = await SignalService.GetSignalCategoriesAsync();
        IsLoadingCategories = false;

        if (result.IsSuccess)
        {
            Categories = result.Value ?? Array.Empty<SignalCategoryItem>();
            return;
        }

        Categories = Array.Empty<SignalCategoryItem>();
        CategoryErrorMessageKey = "signal_home_error";
    }

    private bool ValidateAllSteps()
    {
        if (!ValidateType())
        {
            CurrentStep = SignalCreateStep.Type;
            return false;
        }

        if (!ValidateDetails())
        {
            CurrentStep = SignalCreateStep.Details;
            return false;
        }

        if (!ValidatePhotos())
        {
            CurrentStep = SignalCreateStep.Photos;
            return false;
        }

        return true;
    }

    private bool ValidateType()
    {
        if (Draft.TypeId.HasValue)
        {
            TypeErrorMessageKey = null;
            return true;
        }

        TypeErrorMessageKey = "signal_create_type_required";
        return false;
    }

    private bool ValidateDetails()
    {
        TypeErrorMessageKey = null;
        SaveErrorMessageKey = null;

        if (string.IsNullOrWhiteSpace(Draft.Title))
        {
            DetailsErrorMessageKey = "signal_create_title_required";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Draft.Description))
        {
            DetailsErrorMessageKey = "signal_create_description_required";
            return false;
        }

        if (ParsedTags.Count == 0)
        {
            DetailsErrorMessageKey = "signal_create_tags_required";
            return false;
        }

        DetailsErrorMessageKey = null;
        return true;
    }

    private bool ValidatePhotos()
    {
        if (Draft.Photo1 is null)
        {
            PhotoErrorMessageKey = "signal_create_photos_required";
            return false;
        }

        PhotoErrorMessageKey = null;
        return true;
    }

    protected Task OnPhotoSelectedAsync(int slot, PhotoInputMode mode, InputFileChangeEventArgs args)
    {
        return LoadPhotoAsync(slot, mode, args);
    }

    private static string GetPhotoInputId(int slot, PhotoInputMode mode)
    {
        return $"signal-photo-{slot}-{mode.ToString().ToLowerInvariant()}-input";
    }

    private async Task LoadPhotoAsync(int slot, PhotoInputMode mode, InputFileChangeEventArgs args)
    {
        PhotoErrorMessageKey = null;
        SaveErrorMessageKey = null;

        if (args.File is null)
        {
            return;
        }

        IsProcessingPhoto = true;

        try
        {
            if (string.IsNullOrWhiteSpace(args.File.ContentType) || !args.File.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                PhotoErrorMessageKey = "signal_create_photo_invalid_type";
                return;
            }

            var processedPhoto = await NormalizePhotoAsync(args.File);
            if (processedPhoto is null)
            {
                PhotoErrorMessageKey = "signal_create_photo_too_large";
                return;
            }

            var photo = SignalImageCodec.CreatePhotoDraft(
                processedPhoto.Content,
                processedPhoto.ContentType,
                processedPhoto.FileName);

            if (slot == 1)
            {
                Draft.Photo1 = photo;
            }
            else
            {
                Draft.Photo2 = photo;
            }
        }
        catch (IOException)
        {
            PhotoErrorMessageKey = "signal_create_photo_processing_error";
        }
        catch (InvalidOperationException)
        {
            PhotoErrorMessageKey = "signal_create_photo_processing_error";
        }
        finally
        {
            IsProcessingPhoto = false;
        }
    }

    private static async Task<NormalizedPhotoResult?> NormalizePhotoAsync(IBrowserFile file)
    {
        if (file.Size <= MaxPhotoBytes)
        {
            return await ReadBrowserFileAsync(file, file.ContentType, file.Name);
        }

        for (var dimension = MaxPhotoDimension; dimension >= MinPhotoDimension; dimension = (int)Math.Floor(dimension * 0.8))
        {
            var resized = await file.RequestImageFileAsync("image/jpeg", dimension, dimension);
            if (resized.Size > MaxPhotoBytes)
            {
                continue;
            }

            var result = await ReadBrowserFileAsync(resized, "image/jpeg", BuildNormalizedFileName(file.Name));
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    private static async Task<NormalizedPhotoResult?> ReadBrowserFileAsync(IBrowserFile file, string contentType, string fileName)
    {
        await using var stream = file.OpenReadStream(MaxPhotoBytes);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();

        return bytes.Length > MaxPhotoBytes
            ? null
            : new NormalizedPhotoResult(bytes, string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType, fileName);
    }

    private static string BuildNormalizedFileName(string originalFileName)
    {
        var extensionIndex = originalFileName.LastIndexOf('.');
        return extensionIndex < 0
            ? $"{originalFileName}.jpg"
            : $"{originalFileName[..extensionIndex]}.jpg";
    }

    private static string MapCreateErrorKey(string? errorCode)
    {
        return errorCode switch
        {
            "signals.create_validation" => "signal_create_save_validation_error",
            "auth.session_invalid" => "signal_create_save_auth_error",
            "signals.timeout" => "signal_create_save_timeout",
            _ => "signal_create_save_error"
        };
    }

    protected enum SignalCreateStep
    {
        Type = 1,
        Details = 2,
        Photos = 3,
        Review = 4
    }

    protected enum PhotoInputMode
    {
        Camera,
        Gallery
    }

    private sealed record SignalCoordinates(float Latitude, float Longitude);
    private sealed record NormalizedPhotoResult(byte[] Content, string ContentType, string FileName);
}
