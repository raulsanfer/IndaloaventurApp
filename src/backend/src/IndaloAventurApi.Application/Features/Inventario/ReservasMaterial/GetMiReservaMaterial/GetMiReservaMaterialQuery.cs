using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.GetMiReservaMaterial;

public sealed record GetMiReservaMaterialQuery(
    Guid AuthenticatedUserId,
    bool IsMember,
    Guid ReservaId) : IQuery<ReservaMaterialDto>;

public sealed class GetMiReservaMaterialQueryValidator : AbstractValidator<GetMiReservaMaterialQuery>
{
    public GetMiReservaMaterialQueryValidator()
    {
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.ReservaId).NotEmpty();
    }
}
