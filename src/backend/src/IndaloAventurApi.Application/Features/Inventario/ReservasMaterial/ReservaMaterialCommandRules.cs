using FluentValidation;
using IndaloAventurApi.Domain.Inventario;
using System.Linq.Expressions;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial;

internal static class ReservaMaterialCommandRules
{
    public static void Apply<T>(
        AbstractValidator<T> validator,
        Expression<Func<T, Guid>> userId,
        Expression<Func<T, int>> materialId,
        Expression<Func<T, int>> cantidad,
        Expression<Func<T, DateOnly>> retiradaPrevista,
        Expression<Func<T, DateOnly>> devolucionEstimada,
        Expression<Func<T, string?>> observaciones)
    {
        validator.RuleFor(userId).NotEmpty().WithMessage("El usuario autenticado es obligatorio.");
        validator.RuleFor(materialId).GreaterThan(0).WithMessage("El material de inventario es obligatorio.");
        validator.RuleFor(cantidad).GreaterThan(0).WithMessage("La cantidad solicitada debe ser mayor que cero.");
        validator.RuleFor(devolucionEstimada)
            .GreaterThanOrEqualTo(retiradaPrevista)
            .WithMessage("La fecha estimada de devolucion no puede ser anterior a la retirada prevista.");
        validator.RuleFor(observaciones)
            .MaximumLength(InventarioMaterialTexto.ObservacionesMaxLength)
            .WithMessage($"Las observaciones no pueden superar {InventarioMaterialTexto.ObservacionesMaxLength} caracteres.")
            .Must(value => value is null || value.All(character => !char.IsControl(character) || character == '\n' || character == '\r'))
            .WithMessage("Las observaciones contienen caracteres no permitidos.");
    }
}
