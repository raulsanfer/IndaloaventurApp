using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CreateMiReservaMaterial;

public sealed record CreateMiReservaMaterialCommand(
    Guid AuthenticatedUserId,
    bool IsMember,
    int InventarioMaterialId,
    int CantidadSolicitada,
    DateOnly FechaRetiradaPrevista,
    DateOnly FechaDevolucionEstimada,
    string? Observaciones) : ICommand<ReservaMaterialDto>;

public sealed class CreateMiReservaMaterialCommandValidator : AbstractValidator<CreateMiReservaMaterialCommand>
{
    public CreateMiReservaMaterialCommandValidator()
    {
        ReservaMaterialCommandRules.Apply(
            this,
            x => x.AuthenticatedUserId,
            x => x.InventarioMaterialId,
            x => x.CantidadSolicitada,
            x => x.FechaRetiradaPrevista,
            x => x.FechaDevolucionEstimada,
            x => x.Observaciones);
    }
}
