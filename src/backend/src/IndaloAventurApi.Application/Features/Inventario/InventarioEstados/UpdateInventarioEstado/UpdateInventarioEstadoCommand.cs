using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.UpdateInventarioEstado;

public sealed record UpdateInventarioEstadoCommand(int Id, string Nombre) : ICommand<bool>;

public sealed class UpdateInventarioEstadoCommandValidator : AbstractValidator<UpdateInventarioEstadoCommand>
{
    public UpdateInventarioEstadoCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre no puede superar 120 caracteres.")
            .Must(nombre => nombre is null || !nombre.Any(char.IsControl)).WithMessage("El nombre contiene caracteres no permitidos.");
    }
}
