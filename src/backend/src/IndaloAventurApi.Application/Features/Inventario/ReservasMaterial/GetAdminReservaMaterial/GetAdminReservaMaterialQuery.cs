using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetAdminReservaMaterial;

public sealed record GetAdminReservaMaterialQuery(bool IsAdmin, Guid ReservaId) : IQuery<ReservaMaterialDto>;

public sealed class GetAdminReservaMaterialQueryValidator : AbstractValidator<GetAdminReservaMaterialQuery>
{
    public GetAdminReservaMaterialQueryValidator() => RuleFor(x => x.ReservaId).NotEmpty();
}
