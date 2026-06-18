using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignalComment;

public sealed class CreateSignalCommentCommandHandler(ISignalRepository signalRepository) : IRequestHandler<CreateSignalCommentCommand, Guid>
{
    public async Task<Guid> Handle(CreateSignalCommentCommand request, CancellationToken cancellationToken)
    {
        var signal = await signalRepository.GetByIdAsync(request.SignalId, cancellationToken);
        if (signal is null)
        {
            throw new KeyNotFoundException("La senal no existe.");
        }

        var comentario = signal.AnadirComentario(request.Texto, request.UserId);
        await signalRepository.SaveChangesAsync(cancellationToken);
        return comentario.Id;
    }
}
