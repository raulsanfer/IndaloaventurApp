using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.ConfirmarMiDevolucionMaterial;

public sealed record ConfirmarMiDevolucionMaterialCommand(
    Guid AuthenticatedUserId,
    bool IsMember,
    Guid ReservaId) : ICommand<bool>;

public sealed class ConfirmarMiDevolucionMaterialCommandValidator : AbstractValidator<ConfirmarMiDevolucionMaterialCommand>
{
    public ConfirmarMiDevolucionMaterialCommandValidator()
    {
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.ReservaId).NotEmpty();
    }
}
