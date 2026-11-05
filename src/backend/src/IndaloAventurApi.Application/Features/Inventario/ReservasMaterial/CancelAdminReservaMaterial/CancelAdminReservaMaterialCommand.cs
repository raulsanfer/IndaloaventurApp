using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelAdminReservaMaterial;

public sealed record CancelAdminReservaMaterialCommand(
    bool IsAdmin,
    Guid ReservaId,
    string? ObservacionAdministracion) : ICommand<bool>;

public sealed class CancelAdminReservaMaterialCommandValidator : AbstractValidator<CancelAdminReservaMaterialCommand>
{
    public CancelAdminReservaMaterialCommandValidator()
    {
        RuleFor(x => x.ReservaId).NotEmpty();
        RuleFor(x => x.ObservacionAdministracion).MaximumLength(2000);
    }
}
