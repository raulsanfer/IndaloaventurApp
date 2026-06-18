using IndaloAventurApi.Application.Abstractions.Cqrs;
using FluentValidation;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.DeleteCargo;

public sealed record DeleteCargoCommand(int Id) : ICommand<bool>;

public sealed class DeleteCargoCommandValidator : AbstractValidator<DeleteCargoCommand>
{
    public DeleteCargoCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
