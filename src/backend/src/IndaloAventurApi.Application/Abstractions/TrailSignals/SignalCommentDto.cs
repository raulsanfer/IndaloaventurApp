namespace IndaloAventurApi.Application.Abstractions.TrailSignals;

public sealed record SignalCommentDto(
    Guid Id,
    Guid SignalId,
    Guid UserId,
    DateTime FechaComentario,
    string Texto);
