using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.TrailSignals;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalById;

public sealed record GetSignalByIdQuery(Guid SignalId) : IQuery<SignalDto>;
