using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.TrailSignals;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalComments;

public sealed record GetSignalCommentsQuery(Guid SignalId) : IQuery<IReadOnlyCollection<SignalCommentDto>>;
