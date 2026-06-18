using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.TrailSignals;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalImages;

public sealed record GetSignalImagesQuery(Guid SignalId) : IQuery<SignalImagesDto>;