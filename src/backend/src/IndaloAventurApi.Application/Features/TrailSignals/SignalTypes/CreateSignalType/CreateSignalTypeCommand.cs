using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.CreateSignalType;

public sealed record CreateSignalTypeCommand(string Nombre, string Icono) : ICommand<int>;

public sealed class CreateSignalTypeCommandValidator : AbstractValidator<CreateSignalTypeCommand>
{
    public CreateSignalTypeCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty();
        RuleFor(x => x.Icono).NotEmpty();
    }
}
