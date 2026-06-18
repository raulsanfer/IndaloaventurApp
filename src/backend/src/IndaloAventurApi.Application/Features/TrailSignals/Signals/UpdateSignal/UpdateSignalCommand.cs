using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.UpdateSignal;

public sealed record UpdateSignalCommand(
    Guid Id,
    string Titulo,
    string Descripcion,
    bool Activo,
    Guid UserIdModificacion) : ICommand<bool>;

public sealed class UpdateSignalCommandValidator : AbstractValidator<UpdateSignalCommand>
{
    public const int MaxBytesPorFoto = 2 * 1024 * 1024;

    public UpdateSignalCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Titulo).NotEmpty();
        RuleFor(x => x.Descripcion).NotEmpty();
        RuleFor(x => x.UserIdModificacion).NotEmpty();
    }
}
