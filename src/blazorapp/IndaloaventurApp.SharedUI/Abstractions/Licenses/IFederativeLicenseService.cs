namespace IndaloaventurApp.SharedUI.Abstractions.Licenses;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;

public interface IFederativeLicenseService
{
    Task<ServiceResult<IReadOnlyList<FederativeLicenseRequest>>> GetMyFederativeLicensesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<IReadOnlyList<FederativeLicenseRate>>> GetAvailableRatesAsync(int temporada, bool mediaTemporada = false, CancellationToken cancellationToken = default);
    Task<ServiceResult<FederativeLicenseRequest>> CreateFederativeLicenseRequestAsync(CreateFederativeLicenseRequest request, CancellationToken cancellationToken = default);
}
