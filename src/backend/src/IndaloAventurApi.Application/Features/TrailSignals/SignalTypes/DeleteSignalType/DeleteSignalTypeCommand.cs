using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.DeleteSignalType;

public sealed record DeleteSignalTypeCommand(int Id) : ICommand<bool>;

public sealed class DeleteSignalTypeCommandValidator : AbstractValidator<DeleteSignalTypeCommand>
{
    public DeleteSignalTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

