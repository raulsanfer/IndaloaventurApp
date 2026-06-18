using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.UpdateSignalType;

public sealed record UpdateSignalTypeCommand(int Id, string Nombre, string Icono) : ICommand<bool>;

public sealed class UpdateSignalTypeCommandValidator : AbstractValidator<UpdateSignalTypeCommand>
{
    public UpdateSignalTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty();
        RuleFor(x => x.Icono).NotEmpty();
    }
}

