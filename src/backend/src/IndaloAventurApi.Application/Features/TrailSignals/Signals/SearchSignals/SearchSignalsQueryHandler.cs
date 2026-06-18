using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.SearchSignals;

public sealed class SearchSignalsQueryHandler(ISignalRepository repository) : IRequestHandler<SearchSignalsQuery, IReadOnlyCollection<SignalDto>>
{
    public async Task<IReadOnlyCollection<SignalDto>> Handle(SearchSignalsQuery request, CancellationToken cancellationToken)
    {
        var signals = await repository.SearchAsync(request.Tags, request.Activo, request.Descripcion, request.Tipo, cancellationToken);
        return signals
            .Select(s => new SignalDto(
                s.Id,
                s.Latitud,
                s.Longitud,
                s.Titulo,
                s.Descripcion,
                s.Activo,
                s.UserIdAlta,
                s.FechaAlta,
                s.FechaModificacion,
                s.UserIdModificacion,
                s.Tipo,
                s.Tags))
            .ToArray();
    }
}
