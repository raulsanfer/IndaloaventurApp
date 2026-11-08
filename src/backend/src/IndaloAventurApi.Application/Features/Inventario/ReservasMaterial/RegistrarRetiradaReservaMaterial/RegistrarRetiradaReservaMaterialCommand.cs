using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.RegistrarRetiradaReservaMaterial;

public sealed record RegistrarRetiradaReservaMaterialCommand(
    bool CanManageInventory,
    Guid ReservaId,
    DateOnly FechaRetirada) : ICommand<bool>;

public sealed class RegistrarRetiradaReservaMaterialCommandValidator : AbstractValidator<RegistrarRetiradaReservaMaterialCommand>
{
    public RegistrarRetiradaReservaMaterialCommandValidator() => RuleFor(x => x.ReservaId).NotEmpty();
}
