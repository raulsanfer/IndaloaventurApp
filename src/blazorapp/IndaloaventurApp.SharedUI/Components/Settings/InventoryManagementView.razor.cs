namespace IndaloaventurApp.SharedUI.Components.Settings;

using IndaloaventurApp.SharedUI.Abstractions.Inventory;
using IndaloaventurApp.SharedUI.Abstractions.Session;
using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;
using IndaloaventurApp.SharedUI.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

public partial class InventoryManagementView
{
    [Inject]
    private IInventoryService InventoryService { get; set; } = default!;

    [Inject]
    private IStringLocalizer<SharedTexts> L { get; set; } = default!;

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected IReadOnlyList<InventoryCategory> Categories { get; private set; } = Array.Empty<InventoryCategory>();

    protected IReadOnlyList<InventoryStatus> Statuses { get; private set; } = Array.Empty<InventoryStatus>();

    protected IReadOnlyList<InventoryMaterial> Materials { get; private set; } = Array.Empty<InventoryMaterial>();

    protected InventoryTab ActiveTab { get; private set; } = InventoryTab.Materials;

    protected bool IsLoading { get; private set; } = true;

    protected bool IsSubmitting { get; private set; }

    protected bool IsForbidden { get; private set; }

    protected string? ErrorMessage { get; private set; }

    protected string? LoadErrorMessage { get; private set; }

    protected string? StatusMessage { get; private set; }

    protected CategoryEditorModel CategoryEditor { get; private set; } = new();

    protected StatusEditorModel StatusEditor { get; private set; } = new();

    protected MaterialEditorModel MaterialEditor { get; private set; } = new();

    protected Dictionary<string, List<string>> CategoryErrors { get; } = new(StringComparer.Ordinal);

    protected Dictionary<string, List<string>> StatusErrors { get; } = new(StringComparer.Ordinal);

    protected Dictionary<string, List<string>> MaterialErrors { get; } = new(StringComparer.Ordinal);

    protected int? EditingCategoryId { get; private set; }

    protected int? EditingStatusId { get; private set; }

    protected int? EditingMaterialId { get; private set; }

    protected InventoryCategory? EditingCategory { get; private set; }

    protected DeletionRequest? PendingDeletion { get; private set; }

    protected bool IsEditingCategory => EditingCategoryId.HasValue;

    protected bool IsEditingStatus => EditingStatusId.HasValue;

    protected bool IsEditingMaterial => EditingMaterialId.HasValue;

    protected bool EditingCategoryHasChildren => EditingCategory?.Subcategories.Count > 0;

    protected IReadOnlyList<InventoryCategory> AvailableParentCategories => Categories
        .Where(category => category.Id != EditingCategoryId)
        .ToArray();

    protected IReadOnlyList<CategoryOption> MaterialCategoryOptions => Categories
        .SelectMany(category => new[]
        {
            new CategoryOption(category.Id, category.Name),
        }.Concat(category.Subcategories.Select(subcategory => new CategoryOption(subcategory.Id, $"{category.Name} > {subcategory.Name}"))))
        .ToArray();

    protected override async Task OnInitializedAsync()
    {
        await LoadAllAsync();
    }

    protected void SelectTab(InventoryTab tab)
    {
        ActiveTab = tab;
        ClearFeedback();
    }

    protected async Task ReloadAsync()
    {
        if (IsForbidden)
        {
            return;
        }

        ClearFeedback();
        await LoadAllAsync();
    }

    protected void BeginCreateCategory()
    {
        if (IsForbidden)
        {
            return;
        }

        EditingCategoryId = null;
        EditingCategory = null;
        CategoryEditor = new CategoryEditorModel();
        CategoryErrors.Clear();
        ClearFeedback();
    }

    protected void BeginEditCategory(InventoryCategory category)
    {
        if (IsForbidden)
        {
            return;
        }

        EditingCategoryId = category.Id;
        EditingCategory = category;
        CategoryEditor = new CategoryEditorModel
        {
            Name = category.Name,
            ParentCategoryId = category.Subcategories.Count > 0 ? null : category.ParentCategoryId
        };
        CategoryErrors.Clear();
        ClearFeedback();
    }

    protected void BeginCreateStatus()
    {
        if (IsForbidden)
        {
            return;
        }

        EditingStatusId = null;
        StatusEditor = new StatusEditorModel();
        StatusErrors.Clear();
        ClearFeedback();
    }

    protected void BeginEditStatus(InventoryStatus status)
    {
        if (IsForbidden)
        {
            return;
        }

        EditingStatusId = status.Id;
        StatusEditor = new StatusEditorModel { Name = status.Name };
        StatusErrors.Clear();
        ClearFeedback();
    }

    protected void BeginCreateMaterial()
    {
        if (IsForbidden)
        {
            return;
        }

        EditingMaterialId = null;
        MaterialEditor = new MaterialEditorModel();
        MaterialErrors.Clear();
        ClearFeedback();
    }

    protected void BeginEditMaterial(InventoryMaterial material)
    {
        if (IsForbidden)
        {
            return;
        }

        EditingMaterialId = material.Id;
        MaterialEditor = new MaterialEditorModel
        {
            Description = material.Description,
            BrandModel = material.BrandModel,
            Quantity = material.Quantity,
            CategoryId = material.CategoryId,
            StatusId = material.StatusId,
            AcquisitionDate = material.AcquisitionDate,
            AcquisitionOrDonation = material.AcquisitionOrDonation,
            Notes = material.Notes,
            SerialNumber = material.SerialNumber
        };
        MaterialErrors.Clear();
        ClearFeedback();
    }

    protected async Task SaveCategoryAsync(EditContext _)
    {
        if (IsForbidden)
        {
            return;
        }

        ClearFeedback();
        CategoryErrors.Clear();

        if (!ValidateCategory())
        {
            return;
        }

        var parentId = EditingCategoryHasChildren ? null : CategoryEditor.ParentCategoryId;
        var name = CategoryEditor.Name!.Trim();
        var wasEditing = IsEditingCategory;
        IsSubmitting = true;

        var result = wasEditing
            ? await InventoryService.UpdateCategoryAsync(new UpdateInventoryCategoryRequest(EditingCategoryId!.Value, name, parentId))
            : ToBooleanResult(await InventoryService.CreateCategoryAsync(new CreateInventoryCategoryRequest(name, parentId)));

        IsSubmitting = false;
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return;
            }

            ApplyServiceError(result.Error, CategoryErrors, MapCategoryProperty);
            return;
        }

        if (await LoadCategoriesAsync())
        {
            EditingCategoryId = null;
            EditingCategory = null;
            CategoryEditor = new CategoryEditorModel();
            StatusMessage = L[wasEditing ? "inventory_category_updated" : "inventory_category_created"].Value;
        }
    }

    protected async Task SaveStatusAsync(EditContext _)
    {
        if (IsForbidden)
        {
            return;
        }

        ClearFeedback();
        StatusErrors.Clear();

        if (!ValidateStatus())
        {
            return;
        }

        var name = StatusEditor.Name!.Trim();
        var wasEditing = IsEditingStatus;
        IsSubmitting = true;
        var result = wasEditing
            ? await InventoryService.UpdateStatusAsync(new UpdateInventoryStatusRequest(EditingStatusId!.Value, name))
            : ToBooleanResult(await InventoryService.CreateStatusAsync(new CreateInventoryStatusRequest(name)));

        IsSubmitting = false;
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return;
            }

            ApplyServiceError(result.Error, StatusErrors, MapStatusProperty);
            return;
        }

        if (await LoadStatusesAsync())
        {
            EditingStatusId = null;
            StatusEditor = new StatusEditorModel();
            StatusMessage = L[wasEditing ? "inventory_status_updated" : "inventory_status_created"].Value;
        }
    }

    protected async Task SaveMaterialAsync(EditContext _)
    {
        if (IsForbidden)
        {
            return;
        }

        ClearFeedback();
        MaterialErrors.Clear();

        if (!ValidateMaterial())
        {
            return;
        }

        var request = new SaveInventoryMaterialRequest(
            MaterialEditor.Description!.Trim(),
            NormalizeOptional(MaterialEditor.BrandModel),
            MaterialEditor.Quantity,
            MaterialEditor.CategoryId!.Value,
            MaterialEditor.StatusId!.Value,
            MaterialEditor.AcquisitionDate,
            MaterialEditor.AcquisitionOrDonation!.Trim(),
            NormalizeNotes(MaterialEditor.Notes),
            NormalizeOptional(MaterialEditor.SerialNumber));

        var wasEditing = IsEditingMaterial;
        IsSubmitting = true;
        var result = wasEditing
            ? await InventoryService.UpdateMaterialAsync(new UpdateInventoryMaterialRequest(EditingMaterialId!.Value, request))
            : ToBooleanResult(await InventoryService.CreateMaterialAsync(request));

        IsSubmitting = false;
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return;
            }

            ApplyServiceError(result.Error, MaterialErrors, MapMaterialProperty);
            return;
        }

        if (await LoadMaterialsAsync())
        {
            EditingMaterialId = null;
            MaterialEditor = new MaterialEditorModel();
            StatusMessage = L[wasEditing ? "inventory_material_updated" : "inventory_material_created"].Value;
        }
    }

    protected void RequestDeletion(PendingDeleteKind kind, int id, string name, bool hasKnownDependents)
    {
        if (IsForbidden)
        {
            return;
        }

        ClearFeedback();
        PendingDeletion = new DeletionRequest(kind, id, name, hasKnownDependents);
    }

    protected void CancelDeletion()
    {
        PendingDeletion = null;
    }

    protected async Task ConfirmDeletionAsync()
    {
        if (PendingDeletion is null || IsForbidden)
        {
            return;
        }

        ClearFeedback();
        IsSubmitting = true;
        var deletion = PendingDeletion;
        var result = deletion.Kind switch
        {
            PendingDeleteKind.Category => await InventoryService.DeleteCategoryAsync(deletion.Id),
            PendingDeleteKind.Status => await InventoryService.DeleteStatusAsync(deletion.Id),
            PendingDeleteKind.Material => await InventoryService.DeleteMaterialAsync(deletion.Id),
            _ => ServiceResult<bool>.Failure(new ServiceError("inventory.unavailable", L["inventory_operation_error"].Value))
        };

        IsSubmitting = false;
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return;
            }

            ApplyServiceError(result.Error, null, static propertyName => propertyName);
            return;
        }

        PendingDeletion = null;
        var reloaded = deletion.Kind switch
        {
            PendingDeleteKind.Category => await LoadCategoriesAsync(),
            PendingDeleteKind.Status => await LoadStatusesAsync(),
            PendingDeleteKind.Material => await LoadMaterialsAsync(),
            _ => false
        };

        if (!reloaded)
        {
            return;
        }

        if (deletion.Kind == PendingDeleteKind.Category && EditingCategoryId == deletion.Id)
        {
            BeginCreateCategory();
        }
        else if (deletion.Kind == PendingDeleteKind.Status && EditingStatusId == deletion.Id)
        {
            BeginCreateStatus();
        }
        else if (deletion.Kind == PendingDeleteKind.Material && EditingMaterialId == deletion.Id)
        {
            BeginCreateMaterial();
        }

        StatusMessage = L[deletion.Kind switch
        {
            PendingDeleteKind.Category => "inventory_category_deleted",
            PendingDeleteKind.Status => "inventory_status_deleted",
            _ => "inventory_material_deleted"
        }].Value;
    }

    protected bool IsCategoryUsedByMaterial(int categoryId) => Materials.Any(material => material.CategoryId == categoryId);

    protected bool IsStatusUsedByMaterial(int statusId) => Materials.Any(material => material.StatusId == statusId);

    protected string GetMaterialCategoryLabel(InventoryMaterial material)
        => string.IsNullOrWhiteSpace(material.ParentCategoryName)
            ? material.CategoryName
            : $"{material.ParentCategoryName} > {material.CategoryName}";

    protected static IEnumerable<string> GetFieldErrors(IReadOnlyDictionary<string, List<string>> errors, string propertyName)
        => errors.TryGetValue(propertyName, out var messages) ? messages : Array.Empty<string>();

    private async Task LoadAllAsync()
    {
        IsLoading = true;
        var categoriesTask = InventoryService.GetCategoriesAsync();
        var statusesTask = InventoryService.GetStatusesAsync();
        var materialsTask = InventoryService.GetMaterialsAsync();
        await Task.WhenAll(categoriesTask, statusesTask, materialsTask);

        var categoriesResult = await categoriesTask;
        var statusesResult = await statusesTask;
        var materialsResult = await materialsTask;

        var loadError = categoriesResult.Error ?? statusesResult.Error ?? materialsResult.Error;
        if (loadError is not null)
        {
            Categories = categoriesResult.IsSuccess ? categoriesResult.Value ?? Array.Empty<InventoryCategory>() : Array.Empty<InventoryCategory>();
            Statuses = statusesResult.IsSuccess ? statusesResult.Value ?? Array.Empty<InventoryStatus>() : Array.Empty<InventoryStatus>();
            Materials = materialsResult.IsSuccess ? materialsResult.Value ?? Array.Empty<InventoryMaterial>() : Array.Empty<InventoryMaterial>();
            if (await HandleAuthenticationOrAuthorizationFailureAsync(loadError))
            {
                IsLoading = false;
                return;
            }

            LoadErrorMessage = loadError.Message;
            ErrorMessage = loadError.Message;
        }
        else
        {
            Categories = categoriesResult.Value ?? Array.Empty<InventoryCategory>();
            Statuses = statusesResult.Value ?? Array.Empty<InventoryStatus>();
            Materials = materialsResult.Value ?? Array.Empty<InventoryMaterial>();
            LoadErrorMessage = null;
        }

        IsLoading = false;
    }

    private async Task<bool> LoadCategoriesAsync()
    {
        var result = await InventoryService.GetCategoriesAsync();
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return false;
            }

            ApplyLoadError(result.Error);
            return false;
        }

        Categories = result.Value ?? Array.Empty<InventoryCategory>();
        return true;
    }

    private async Task<bool> LoadStatusesAsync()
    {
        var result = await InventoryService.GetStatusesAsync();
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return false;
            }

            ApplyLoadError(result.Error);
            return false;
        }

        Statuses = result.Value ?? Array.Empty<InventoryStatus>();
        return true;
    }

    private async Task<bool> LoadMaterialsAsync()
    {
        var result = await InventoryService.GetMaterialsAsync();
        if (!result.IsSuccess)
        {
            if (await HandleAuthenticationOrAuthorizationFailureAsync(result.Error))
            {
                return false;
            }

            ApplyLoadError(result.Error);
            return false;
        }

        Materials = result.Value ?? Array.Empty<InventoryMaterial>();
        return true;
    }

    private bool ValidateCategory()
    {
        CategoryEditor.Name = NormalizeRequired(CategoryEditor.Name);
        ValidateText(CategoryErrors, nameof(CategoryEditor.Name), CategoryEditor.Name, 120, true, false);

        if (CategoryEditor.ParentCategoryId.HasValue && !Categories.Any(category => category.Id == CategoryEditor.ParentCategoryId.Value))
        {
            AddError(CategoryErrors, nameof(CategoryEditor.ParentCategoryId), L["inventory_invalid_parent_category"].Value);
        }

        var parentId = EditingCategoryHasChildren ? null : CategoryEditor.ParentCategoryId;
        if (!CategoryErrors.ContainsKey(nameof(CategoryEditor.Name)) && HasDuplicateCategoryName(CategoryEditor.Name, parentId))
        {
            AddError(CategoryErrors, nameof(CategoryEditor.Name), L["inventory_duplicate_category_name"].Value);
        }

        return CategoryErrors.Count == 0;
    }

    private bool ValidateStatus()
    {
        StatusEditor.Name = NormalizeRequired(StatusEditor.Name);
        ValidateText(StatusErrors, nameof(StatusEditor.Name), StatusEditor.Name, 120, true, false);

        if (!StatusErrors.ContainsKey(nameof(StatusEditor.Name)) && Statuses.Any(status =>
                status.Id != EditingStatusId &&
                string.Equals(status.Name.Trim(), StatusEditor.Name, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(StatusErrors, nameof(StatusEditor.Name), L["inventory_duplicate_status_name"].Value);
        }

        return StatusErrors.Count == 0;
    }

    private bool ValidateMaterial()
    {
        MaterialEditor.Description = NormalizeRequired(MaterialEditor.Description);
        MaterialEditor.AcquisitionOrDonation = NormalizeRequired(MaterialEditor.AcquisitionOrDonation);
        MaterialEditor.BrandModel = NormalizeOptional(MaterialEditor.BrandModel);
        MaterialEditor.Notes = NormalizeNotes(MaterialEditor.Notes);
        MaterialEditor.SerialNumber = NormalizeOptional(MaterialEditor.SerialNumber);

        ValidateText(MaterialErrors, nameof(MaterialEditor.Description), MaterialEditor.Description, 250, true, false);
        ValidateText(MaterialErrors, nameof(MaterialEditor.BrandModel), MaterialEditor.BrandModel, 200, false, false);
        ValidateText(MaterialErrors, nameof(MaterialEditor.AcquisitionOrDonation), MaterialEditor.AcquisitionOrDonation, 250, true, false);
        ValidateText(MaterialErrors, nameof(MaterialEditor.Notes), MaterialEditor.Notes, 2000, false, true);
        ValidateText(MaterialErrors, nameof(MaterialEditor.SerialNumber), MaterialEditor.SerialNumber, 120, false, false);

        if (MaterialEditor.Quantity <= 0)
        {
            AddError(MaterialErrors, nameof(MaterialEditor.Quantity), L["inventory_quantity_invalid"].Value);
        }

        if (!MaterialEditor.CategoryId.HasValue || !MaterialCategoryOptions.Any(category => category.Id == MaterialEditor.CategoryId.Value))
        {
            AddError(MaterialErrors, nameof(MaterialEditor.CategoryId), L["inventory_category_invalid"].Value);
        }

        if (!MaterialEditor.StatusId.HasValue || !Statuses.Any(status => status.Id == MaterialEditor.StatusId.Value))
        {
            AddError(MaterialErrors, nameof(MaterialEditor.StatusId), L["inventory_status_invalid"].Value);
        }

        if (MaterialEditor.AcquisitionDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            AddError(MaterialErrors, nameof(MaterialEditor.AcquisitionDate), L["inventory_acquisition_date_invalid"].Value);
        }

        if (!string.IsNullOrWhiteSpace(MaterialEditor.SerialNumber) && MaterialEditor.Quantity != 1)
        {
            AddError(MaterialErrors, nameof(MaterialEditor.SerialNumber), L["inventory_serial_number_quantity_invalid"].Value);
        }

        if (!string.IsNullOrWhiteSpace(MaterialEditor.SerialNumber) && Materials.Any(material =>
                material.Id != EditingMaterialId &&
                string.Equals(material.SerialNumber?.Trim(), MaterialEditor.SerialNumber, StringComparison.OrdinalIgnoreCase)))
        {
            AddError(MaterialErrors, nameof(MaterialEditor.SerialNumber), L["inventory_duplicate_serial_number"].Value);
        }

        return MaterialErrors.Count == 0;
    }

    private void ValidateText(
        Dictionary<string, List<string>> errors,
        string propertyName,
        string? value,
        int maximumLength,
        bool required,
        bool allowNewLines)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (required)
            {
                AddError(errors, propertyName, L["inventory_required_field"].Value);
            }

            return;
        }

        if (value.Length > maximumLength)
        {
            AddError(errors, propertyName, string.Format(L["inventory_maximum_length"], maximumLength));
        }

        if (value.Any(character => char.IsControl(character) && (!allowNewLines || character != '\n')))
        {
            AddError(errors, propertyName, L["inventory_invalid_characters"].Value);
        }
    }

    private bool HasDuplicateCategoryName(string name, int? parentId)
        => GetAllCategories().Any(category =>
            category.Id != EditingCategoryId &&
            category.ParentCategoryId == parentId &&
            string.Equals(category.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));

    private IEnumerable<InventoryCategory> GetAllCategories()
        => Categories.SelectMany(category => new[] { category }.Concat(category.Subcategories));

    private void ApplyServiceError(
        ServiceError? error,
        Dictionary<string, List<string>>? fieldErrors,
        Func<string, string> mapProperty)
    {
        ErrorMessage = error?.Message ?? L["inventory_operation_error"].Value;
        if (fieldErrors is null || error?.ValidationErrors is null)
        {
            return;
        }

        foreach (var validationError in error.ValidationErrors)
        {
            AddError(fieldErrors, mapProperty(validationError.PropertyName), validationError.ErrorMessage);
        }
    }

    private void ApplyLoadError(ServiceError? error)
    {
        ErrorMessage = error?.Message ?? L["inventory_load_error"].Value;
        LoadErrorMessage = ErrorMessage;
    }

    private async Task<bool> HandleAuthenticationOrAuthorizationFailureAsync(ServiceError? error)
    {
        if (error?.Code == "auth.session_invalid")
        {
            Categories = Array.Empty<InventoryCategory>();
            Statuses = Array.Empty<InventoryStatus>();
            Materials = Array.Empty<InventoryMaterial>();
            PendingDeletion = null;

            if (ServiceProvider.GetService(typeof(ISessionService)) is ISessionService sessionService)
            {
                await sessionService.SignOutAsync();
            }

            NavigationManager.NavigateTo("/", true);
            return true;
        }

        if (error?.Code == "inventory.forbidden")
        {
            PendingDeletion = null;
            IsForbidden = true;
            ErrorMessage = L["inventory_forbidden"].Value;
            LoadErrorMessage = ErrorMessage;
            return true;
        }

        return false;
    }

    private void ClearFeedback()
    {
        ErrorMessage = null;
        LoadErrorMessage = null;
        StatusMessage = null;
    }

    private static void AddError(Dictionary<string, List<string>> errors, string propertyName, string message)
    {
        if (!errors.TryGetValue(propertyName, out var messages))
        {
            messages = new List<string>();
            errors[propertyName] = messages;
        }

        messages.Add(message);
    }

    private static ServiceResult<bool> ToBooleanResult(ServiceResult<int> result)
        => result.IsSuccess
            ? ServiceResult<bool>.Success(true)
            : ServiceResult<bool>.Failure(result.Error!);

    private static string MapCategoryProperty(string propertyName)
        => propertyName.Equals("categoriaPadreId", StringComparison.OrdinalIgnoreCase) ? nameof(CategoryEditorModel.ParentCategoryId) : nameof(CategoryEditorModel.Name);

    private static string MapStatusProperty(string _) => nameof(StatusEditorModel.Name);

    private static string MapMaterialProperty(string propertyName)
        => propertyName.ToLowerInvariant() switch
        {
            "descripcion" => nameof(MaterialEditorModel.Description),
            "marcamodelo" => nameof(MaterialEditorModel.BrandModel),
            "cantidad" => nameof(MaterialEditorModel.Quantity),
            "categoriaid" => nameof(MaterialEditorModel.CategoryId),
            "estadoid" => nameof(MaterialEditorModel.StatusId),
            "fechaadquisicion" => nameof(MaterialEditorModel.AcquisitionDate),
            "adquiridodonado" => nameof(MaterialEditorModel.AcquisitionOrDonation),
            "observaciones" => nameof(MaterialEditorModel.Notes),
            "numeroserie" => nameof(MaterialEditorModel.SerialNumber),
            _ => nameof(MaterialEditorModel.Description)
        };

    private static string NormalizeRequired(string? value) => value?.Trim() ?? string.Empty;

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeNotes(string? value)
    {
        var normalized = NormalizeOptional(value)?.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    protected enum InventoryTab
    {
        Materials,
        Categories,
        Statuses
    }

    protected enum PendingDeleteKind
    {
        Category,
        Status,
        Material
    }

    protected sealed record CategoryOption(int Id, string Label);

    protected sealed record DeletionRequest(PendingDeleteKind Kind, int Id, string Name, bool HasKnownDependents);

    protected sealed class CategoryEditorModel
    {
        public string? Name { get; set; }

        public int? ParentCategoryId { get; set; }
    }

    protected sealed class StatusEditorModel
    {
        public string? Name { get; set; }
    }

    protected sealed class MaterialEditorModel
    {
        public string? Description { get; set; }

        public string? BrandModel { get; set; }

        public int Quantity { get; set; }

        public int? CategoryId { get; set; }

        public int? StatusId { get; set; }

        public DateOnly? AcquisitionDate { get; set; }

        public string? AcquisitionOrDonation { get; set; }

        public string? Notes { get; set; }

        public string? SerialNumber { get; set; }
    }
}
