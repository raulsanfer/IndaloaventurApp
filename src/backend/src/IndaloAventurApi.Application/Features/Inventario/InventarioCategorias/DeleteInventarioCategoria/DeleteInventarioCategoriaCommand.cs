using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.DeleteInventarioCategoria;

public sealed record DeleteInventarioCategoriaCommand(int Id) : ICommand<bool>;

public sealed class DeleteInventarioCategoriaCommandValidator : AbstractValidator<DeleteInventarioCategoriaCommand>
{
    public DeleteInventarioCategoriaCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
