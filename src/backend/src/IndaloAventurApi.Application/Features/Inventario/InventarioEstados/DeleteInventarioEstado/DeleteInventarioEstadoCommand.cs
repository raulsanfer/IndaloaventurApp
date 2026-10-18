using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.DeleteInventarioEstado;

public sealed record DeleteInventarioEstadoCommand(int Id) : ICommand<bool>;

public sealed class DeleteInventarioEstadoCommandValidator : AbstractValidator<DeleteInventarioEstadoCommand>
{
    public DeleteInventarioEstadoCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
