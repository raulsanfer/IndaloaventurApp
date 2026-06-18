namespace IndaloaventurApp.SharedUI.Abstractions.Cargos;

using IndaloaventurApp.SharedUI.Models.Cargos;
using IndaloaventurApp.SharedUI.Models.Common;

public interface ICargoAdminService
{
    Task<ServiceResult<IReadOnlyList<CargoItem>>> GetCargosAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<CargoItem>> CreateCargoAsync(CreateCargoRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<CargoItem>> UpdateCargoAsync(UpdateCargoRequest request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeleteCargoAsync(int id, CancellationToken cancellationToken = default);
}
