using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.TrailSignals;

namespace IndaloAventurApi.Application.Features.TrailSignals.SignalTypes.GetAllSignalTypes;

public sealed record GetAllSignalTypesQuery : IQuery<IReadOnlyCollection<SignalTypeDto>>;