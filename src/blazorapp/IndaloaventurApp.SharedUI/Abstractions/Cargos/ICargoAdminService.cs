namespace IndaloaventurApp.SharedUI.Abstractions.Cargos;

using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;

/// <summary>
/// Provides administrator operations for club cargo definitions.
/// </summary>
public interface ICargoAdminService
{
    /// <summary>
    /// Gets all cargos available for member files.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<CargoItem>>> GetCargosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new cargo.
    /// </summary>
    Task<ServiceResult<CargoItem>> CreateCargoAsync(CreateCargoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cargo.
    /// </summary>
    Task<ServiceResult<CargoItem>> UpdateCargoAsync(UpdateCargoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing cargo by identifier.
    /// </summary>
    Task<ServiceResult<bool>> DeleteCargoAsync(int id, CancellationToken cancellationToken = default);
}
