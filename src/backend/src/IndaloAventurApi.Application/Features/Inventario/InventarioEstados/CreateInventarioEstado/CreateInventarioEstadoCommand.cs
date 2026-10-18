using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.InventarioEstados.CreateInventarioEstado;

public sealed record CreateInventarioEstadoCommand(string Nombre) : ICommand<int>;

public sealed class CreateInventarioEstadoCommandValidator : AbstractValidator<CreateInventarioEstadoCommand>
{
    public CreateInventarioEstadoCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre no puede superar 120 caracteres.")
            .Must(nombre => nombre is null || !nombre.Any(char.IsControl)).WithMessage("El nombre contiene caracteres no permitidos.");
    }
}
