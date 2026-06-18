using IndaloAventurApi.Application.Abstractions.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.GetSignalById;

public sealed class GetSignalByIdQueryHandler(ISignalRepository signalRepository) : IRequestHandler<GetSignalByIdQuery, SignalDto>
{
    public async Task<SignalDto> Handle(GetSignalByIdQuery request, CancellationToken cancellationToken)
    {
        var signal = await signalRepository.GetByIdAsync(request.SignalId, cancellationToken);
        if (signal is null)
        {
            throw new KeyNotFoundException("La senal no existe.");
        }

        return new SignalDto(
            signal.Id,
            signal.Latitud,
            signal.Longitud,
            signal.Titulo,
            signal.Descripcion,
            signal.Activo,
            signal.UserIdAlta,
            signal.FechaAlta,
            signal.FechaModificacion,
            signal.UserIdModificacion,
            signal.Tipo,
            signal.Tags);
    }
}
