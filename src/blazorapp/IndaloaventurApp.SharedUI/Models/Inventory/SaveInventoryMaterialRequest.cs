namespace IndaloaventurApp.SharedUI.Models.Inventory;

/// <summary>
/// Contains the complete editable values of an inventory material.
/// </summary>
/// <param name="Description">The material description.</param>
/// <param name="BrandModel">The optional brand or model.</param>
/// <param name="Quantity">The number of units registered.</param>
/// <param name="CategoryId">The selected category identifier.</param>
/// <param name="StatusId">The selected status identifier.</param>
/// <param name="AcquisitionDate">The optional acquisition date.</param>
/// <param name="AcquisitionOrDonation">The acquisition or donation description.</param>
/// <param name="Notes">The optional notes.</param>
/// <param name="SerialNumber">The optional serial number.</param>
public sealed record SaveInventoryMaterialRequest(
    string Description,
    string? BrandModel,
    int Quantity,
    int CategoryId,
    int StatusId,
    DateOnly? AcquisitionDate,
    string AcquisitionOrDonation,
    string? Notes,
    string? SerialNumber);
