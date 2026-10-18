using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioCategorias.UpdateInventarioCategoria;

public sealed record UpdateInventarioCategoriaCommand(int Id, string Nombre, int? CategoriaPadreId) : ICommand<bool>;

public sealed class UpdateInventarioCategoriaCommandValidator : AbstractValidator<UpdateInventarioCategoriaCommand>
{
    public UpdateInventarioCategoriaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre no puede superar 120 caracteres.")
            .Must(nombre => nombre is null || !nombre.Any(char.IsControl)).WithMessage("El nombre contiene caracteres no permitidos.");
        RuleFor(x => x.CategoriaPadreId).GreaterThan(0).When(x => x.CategoriaPadreId.HasValue);
    }
}
