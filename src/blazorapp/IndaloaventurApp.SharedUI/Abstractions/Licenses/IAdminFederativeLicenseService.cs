namespace IndaloaventurApp.SharedUI.Abstractions.Licenses;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;

public interface IAdminFederativeLicenseService
{
    Task<ServiceResult<IReadOnlyList<AdminFederativeLicenseRequest>>> GetFederativeLicensesAsync(
        AdminFederativeLicenseQuery query,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<AdminFederativeLicenseRequest>> UpdateFederativeLicenseStatusAsync(
        UpdateAdminFederativeLicenseStatusRequest request,
        CancellationToken cancellationToken = default);
}
