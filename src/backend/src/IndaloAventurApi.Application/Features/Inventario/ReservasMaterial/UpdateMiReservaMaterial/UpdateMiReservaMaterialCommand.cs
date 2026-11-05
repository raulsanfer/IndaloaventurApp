using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.Inventario.ReservasMaterial;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.UpdateMiReservaMaterial;

public sealed record UpdateMiReservaMaterialCommand(
    Guid AuthenticatedUserId,
    bool IsMember,
    Guid ReservaId,
    int InventarioMaterialId,
    int CantidadSolicitada,
    DateOnly FechaRetiradaPrevista,
    DateOnly FechaDevolucionEstimada,
    string? Observaciones) : ICommand<ReservaMaterialDto>;

public sealed class UpdateMiReservaMaterialCommandValidator : AbstractValidator<UpdateMiReservaMaterialCommand>
{
    public UpdateMiReservaMaterialCommandValidator()
    {
        RuleFor(x => x.ReservaId).NotEmpty().WithMessage("La reserva de material es obligatoria.");
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
