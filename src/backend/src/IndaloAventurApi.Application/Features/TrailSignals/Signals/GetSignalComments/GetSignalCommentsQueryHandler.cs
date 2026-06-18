using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalComments;

public sealed class GetSignalCommentsQueryHandler(ISignalRepository signalRepository) : IRequestHandler<GetSignalCommentsQuery, IReadOnlyCollection<SignalCommentDto>>
{
    public async Task<IReadOnlyCollection<SignalCommentDto>> Handle(GetSignalCommentsQuery request, CancellationToken cancellationToken)
    {
        var signal = await signalRepository.GetByIdAsync(request.SignalId, cancellationToken);
        if (signal is null)
        {
            throw new KeyNotFoundException("La senal no existe.");
        }

        var comments = await signalRepository.GetCommentsBySignalIdAsync(request.SignalId, cancellationToken);
        return comments
            .Select(comment => new SignalCommentDto(
                comment.Id,
                comment.SignalId,
                comment.UserId,
                comment.FechaComentario,
                comment.Texto))
            .ToArray();
    }
}
