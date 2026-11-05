using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMisReservasMaterial;

public sealed record GetMisReservasMaterialQuery(Guid AuthenticatedUserId, bool IsMember)
    : IQuery<IReadOnlyCollection<ReservaMaterialDto>>;
