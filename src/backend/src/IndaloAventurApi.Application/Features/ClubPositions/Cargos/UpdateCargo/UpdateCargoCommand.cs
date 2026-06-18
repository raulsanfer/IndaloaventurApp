using IndaloAventurApi.Application.Abstractions.Cqrs;
using FluentValidation;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.UpdateCargo;

public sealed record UpdateCargoCommand(int Id, string Descripcion) : ICommand<bool>;

public sealed class UpdateCargoCommandValidator : AbstractValidator<UpdateCargoCommand>
{
    public UpdateCargoCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(200);
    }
}
