using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.Inventario.ReservasMaterial.CancelMiReservaMaterial;

public sealed record CancelMiReservaMaterialCommand(
    Guid AuthenticatedUserId,
    bool IsMember,
    Guid ReservaId) : ICommand<bool>;

public sealed class CancelMiReservaMaterialCommandValidator : AbstractValidator<CancelMiReservaMaterialCommand>
{
    public CancelMiReservaMaterialCommandValidator()
    {
        RuleFor(x => x.AuthenticatedUserId).NotEmpty();
        RuleFor(x => x.ReservaId).NotEmpty();
    }
}
