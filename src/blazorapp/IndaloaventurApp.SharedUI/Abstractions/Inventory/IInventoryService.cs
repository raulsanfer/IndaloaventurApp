namespace IndaloaventurApp.SharedUI.Abstractions.Inventory;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Provides administrator operations for the club material inventory and its catalogs.
/// </summary>
public interface IInventoryService
{
    /// <summary>Gets the complete inventory category tree.</summary>
    Task<ServiceResult<IReadOnlyList<InventoryCategory>>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates an inventory category.</summary>
    Task<ServiceResult<int>> CreateCategoryAsync(CreateInventoryCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an inventory category.</summary>
    Task<ServiceResult<bool>> UpdateCategoryAsync(UpdateInventoryCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an inventory category.</summary>
    Task<ServiceResult<bool>> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets all inventory statuses.</summary>
    Task<ServiceResult<IReadOnlyList<InventoryStatus>>> GetStatusesAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates an inventory status.</summary>
    Task<ServiceResult<int>> CreateStatusAsync(CreateInventoryStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an inventory status.</summary>
    Task<ServiceResult<bool>> UpdateStatusAsync(UpdateInventoryStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an inventory status.</summary>
    Task<ServiceResult<bool>> DeleteStatusAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets all inventory materials.</summary>
    Task<ServiceResult<IReadOnlyList<InventoryMaterial>>> GetMaterialsAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates an inventory material.</summary>
    Task<ServiceResult<int>> CreateMaterialAsync(SaveInventoryMaterialRequest request, CancellationToken cancellationToken = default);

    /// <summary>Updates an inventory material.</summary>
    Task<ServiceResult<bool>> UpdateMaterialAsync(UpdateInventoryMaterialRequest request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an inventory material.</summary>
    Task<ServiceResult<bool>> DeleteMaterialAsync(int id, CancellationToken cancellationToken = default);
}
