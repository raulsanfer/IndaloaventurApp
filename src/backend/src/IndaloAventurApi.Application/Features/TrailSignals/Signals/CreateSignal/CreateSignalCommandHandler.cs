using IndaloAventurApi.Application.Abstractions.TrailSignals;
using IndaloAventurApi.Domain.TrailSignals;
using MediatR;

namespace IndaloAventurApi.Application.Features.TrailSignals.Signals.CreateSignal;

public sealed class CreateSignalCommandHandler(
    ISignalRepository signalRepository,
    ISignalTypeRepository signalTypeRepository) : IRequestHandler<CreateSignalCommand, Guid>
{
    public async Task<Guid> Handle(CreateSignalCommand request, CancellationToken cancellationToken)
    {
        if (!await signalTypeRepository.ExistsAsync(request.Tipo, cancellationToken))
        {
            throw new InvalidOperationException("El tipo de senal indicado no existe.");
        }

        var signal = Signal.Crear(
            request.Latitud,
            request.Longitud,
            request.Titulo,
            request.Descripcion,
            request.Foto1,
            request.Foto2,
            request.Activo,
            request.UserIdAlta,
            request.Tipo,
            request.Tags);

        await signalRepository.AddAsync(signal, cancellationToken);
        await signalRepository.SaveChangesAsync(cancellationToken);
        return signal.Id;
    }
}
