using IndaloAventurApi.Application.Abstractions.Cqrs;
using FluentValidation;

namespace IndaloAventurApi.Application.Features.ClubPositions.Cargos.CreateCargo;

public sealed record CreateCargoCommand(string Descripcion) : ICommand<int>;

public sealed class CreateCargoCommandValidator : AbstractValidator<CreateCargoCommand>
{
    public CreateCargoCommandValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(200);
    }
}
