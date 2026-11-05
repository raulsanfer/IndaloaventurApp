namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Represents a material registered in the club inventory.
/// </summary>
/// <param name="Id">The material identifier.</param>
/// <param name="Description">The material description.</param>
/// <param name="BrandModel">The optional brand or model.</param>
/// <param name="Quantity">The number of units registered.</param>
/// <param name="CategoryId">The selected category identifier.</param>
/// <param name="CategoryName">The selected category display name.</param>
/// <param name="ParentCategoryId">The optional parent category identifier.</param>
/// <param name="ParentCategoryName">The optional parent category display name.</param>
/// <param name="StatusId">The selected status identifier.</param>
/// <param name="StatusName">The selected status display name.</param>
/// <param name="AcquisitionDate">The optional acquisition date.</param>
/// <param name="AcquisitionOrDonation">The acquisition or donation description.</param>
/// <param name="Notes">The optional notes.</param>
/// <param name="SerialNumber">The optional serial number.</param>
public sealed record InventoryMaterial(
    int Id,
    string Description,
    string? BrandModel,
    int Quantity,
    int CategoryId,
    string CategoryName,
    int? ParentCategoryId,
    string? ParentCategoryName,
    int StatusId,
    string StatusName,
    DateOnly? AcquisitionDate,
    string AcquisitionOrDonation,
    string? Notes,
    string? SerialNumber);
