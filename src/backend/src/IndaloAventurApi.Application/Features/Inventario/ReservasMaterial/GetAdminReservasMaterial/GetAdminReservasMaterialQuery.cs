using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservasMaterial;

public sealed record GetAdminReservasMaterialQuery(bool CanManageInventory) : IQuery<IReadOnlyCollection<ReservaMaterialDto>>;
