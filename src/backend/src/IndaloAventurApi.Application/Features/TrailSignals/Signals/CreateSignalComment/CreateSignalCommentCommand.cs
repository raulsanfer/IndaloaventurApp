using FluentValidation;
using IndaloAventurApi.Application.Abstractions.Cqrs;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignalComment;

public sealed record CreateSignalCommentCommand(Guid SignalId, Guid UserId, string Texto) : ICommand<Guid>;

public sealed class CreateSignalCommentCommandValidator : AbstractValidator<CreateSignalCommentCommand>
{
    public const int MaxTextoLength = 2000;

    public CreateSignalCommentCommandValidator()
    {
        RuleFor(x => x.SignalId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Texto).NotEmpty().MaximumLength(MaxTextoLength);
    }
}
