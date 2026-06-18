using IndaloAventurApi.Application.Abstractions.Cqrs;
using IndaloAventurApi.Application.Abstractions.TrailSignals;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.SearchSignals;

public sealed record SearchSignalsQuery(string? Tags, bool? Activo, string? Descripcion, int? Tipo) : IQuery<IReadOnlyCollection<SignalDto>>;

