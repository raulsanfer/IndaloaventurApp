using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarDevolucionAdministracion;

public sealed record ConfirmarDevolucionAdministracionCommand(
    bool IsAdmin,
    Guid ReservaId,
    DateOnly FechaDevolucionEfectiva) : ICommand<bool>;

public sealed class ConfirmarDevolucionAdministracionCommandValidator : AbstractValidator<ConfirmarDevolucionAdministracionCommand>
{
    public ConfirmarDevolucionAdministracionCommandValidator() => RuleFor(x => x.ReservaId).NotEmpty();
}
