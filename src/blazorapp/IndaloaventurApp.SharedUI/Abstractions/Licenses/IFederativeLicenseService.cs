namespace IndaloaventurApp.SharedUI.Abstractions.Licenses;

using IndaloaventurApp.SharedUI.Models.Common;
using IndaloaventurApp.SharedUI.Models.Licenses;

/// <summary>
/// Provides member-facing federative license operations.
/// </summary>
public interface IFederativeLicenseService
{
    /// <summary>
    /// Gets federative license requests for the current member.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<FederativeLicenseRequest>>> GetMyFederativeLicensesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the authenticated read-only type catalog in API order.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<FederativeLicenseType>>> GetLicenseTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available license rates for the selected season and catalog type.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<FederativeLicenseRate>>> GetAvailableRatesAsync(int temporada, int tipoLicenciaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a federative license request for the current member.
    /// </summary>
    Task<ServiceResult<FederativeLicenseRequest>> CreateFederativeLicenseRequestAsync(CreateFederativeLicenseRequest request, CancellationToken cancellationToken = default);
}
