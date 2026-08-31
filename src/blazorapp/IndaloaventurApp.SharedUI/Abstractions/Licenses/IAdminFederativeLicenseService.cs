namespace IndaloaventurApp.SharedUI.Abstractions.Licenses;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>
/// Provides administrator operations for federative license requests.
/// </summary>
public interface IAdminFederativeLicenseService
{
    /// <summary>
    /// Gets federative license requests using optional administrator filters.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>> GetFederativeLicensesAsync(
        AdminFederativeLicenseQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of a federative license request.
    /// </summary>
    Task<ServiceResult<AdminFederativeLicenseRequest>> UpdateFederativeLicenseStatusAsync(
        UpdateAdminFederativeLicenseStatusRequest request,
        CancellationToken cancellationToken = default);
}
